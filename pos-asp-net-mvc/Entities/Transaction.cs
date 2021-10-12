using pos_asp_net_mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Entities
{
    [Table("Transactions")]
    public class Transaction
    {
        public Transaction()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public int TypeOf { get; set; }

        public int Status { get; set; }

        public string InvoiceNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        [DisplayName("Supplier")]
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [DisplayName("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [DisplayName("User")]
        [Required(AllowEmptyStrings = true)]
        public String UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DefaultValue(0)]
        public int TotalItems { get; set; }

        [DefaultValue(0)]
        public float SubTotal { get; set; }

        [DefaultValue(0)]
        public float Discount { get; set; }

        [DefaultValue(0)]
        public float Tax { get; set; }

        [DefaultValue(0)]
        public float GrandTotal { get; set; }

        [DefaultValue(0)]
        public float Cash { get; set; }

        [DefaultValue(0)]
        public float Change { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }

    }
}