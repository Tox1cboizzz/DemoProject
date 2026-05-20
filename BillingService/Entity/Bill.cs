using System.ComponentModel.DataAnnotations;

namespace BillingService.Entity
{
    public class Bill
    {
        public int Id { get; set; }

        public string CustomerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "ko dc de trong Amount")]
        [Range(1,999999, ErrorMessage = "phai lon hon 0")]
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;

        public int UserId { get; set; }
    }
} 