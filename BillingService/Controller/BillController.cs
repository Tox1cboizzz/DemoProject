using BillingService.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BillingService.DTOs;
using BillingService.Messaging;

namespace BillingService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RabbitMQProducer _producer;
        public BillController(AppDbContext context, IHttpClientFactory httpClientFactory, RabbitMQProducer producer)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _producer = producer;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bills = await _context.Bills.ToListAsync();
            return Ok(bills);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var bill = await _context.Bills
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (bill == null) return NotFound();
            return Ok(bill);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Bill bill)
        {
            var client = _httpClientFactory.CreateClient("UserService");
            var response = await client.GetAsync($"/api/User/{bill.UserId}");

            if (!response.IsSuccessStatusCode)
                return BadRequest($"UserId{bill.UserId} ko ton tai");

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            bill.CustomerCode = user!.Username;

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return Ok(bill);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Bill bill)
        {
            var existingBill = await _context.Bills.FindAsync(id);
            if (existingBill == null) return NotFound();

            existingBill.CustomerCode = bill.CustomerCode;
            existingBill.Amount = bill.Amount;
            existingBill.IsPaid = bill.IsPaid;
            existingBill.UserId = bill.UserId;

            await _context.SaveChangesAsync();
            return Ok(existingBill);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return NotFound();

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
            return Ok("xoa thanh cong");
        }

        [HttpPut("pay/{id}")]
        public async Task<IActionResult> Pay(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return NotFound();
            bill.IsPaid = true;
            await _context.SaveChangesAsync();
            await _producer.SendMessage("bill-paid", new
            {
                BillId = bill.Id,
                CusstomerCode = bill.CustomerCode,
                Amount = bill.Amount,
                UserId = bill.UserId
            });
            return Ok("thanh toan thanh cong");
        }
    }
}

