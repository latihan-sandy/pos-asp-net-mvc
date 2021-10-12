using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Entities
{
    [Table("TransactionDetails")]
    public class TransactionDetail
    {
        public TransactionDetail()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [DisplayName("Transaction")]
        public int? TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }

        [DefaultValue(0)]
        public float Price { get; set; }

        [DefaultValue(0)]
        public int Qty { get; set; }

        [DefaultValue(0)]
        public float Total { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}