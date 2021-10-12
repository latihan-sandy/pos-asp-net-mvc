using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Entities
{
    [Table("Suppliers")]
    public class Supplier
    {

        public Supplier()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Website { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}