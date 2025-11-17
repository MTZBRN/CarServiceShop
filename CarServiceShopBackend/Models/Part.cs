using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models
{
    public class Part
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PartNumber { get; set; }

        // Nettó ár tárolása
        public decimal NetPrice { get; set; }
        public int Quantity { get; set; }
        public int ServiceId { get; set; }

        [NotMapped]
        public decimal VATRate { get; set; } = 0.27m; // ÁFA 27%

        // Bruttó ár kiszámolása nem tárolva az adatbázisban
        [NotMapped]
        public decimal GrossPrice => NetPrice * (1 + VATRate);

        public Service Service { get; set; }
    }
}
