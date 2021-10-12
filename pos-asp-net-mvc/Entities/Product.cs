using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Entities
{
    [Table("Products")]
    public class Product
    {

        public Product()
        {
            CreatedAt = DateTime.Now;
            DateExpired = DateTime.Now.Date;
            Image = "Image/no-image.png";
            PriceProfit = 0;
            PricePurchase = 0;
            PriceSale = 0;
            Stock = 0;
        }

        [Key]
        public int Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Image { get; set; }

        [DisplayName("Brand")]
        public int? BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        [DisplayName("Type")]
        public int? TypeId { get; set; }
        public virtual Type Type { get; set; }

        [DisplayName("Supplier")]
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [DefaultValue(0)]
        public int Stock { get; set; }

        [DefaultValue(0)]
        public float PricePurchase { get; set; }

        [DefaultValue(0)]
        public float PriceSale { get; set; }

        [DefaultValue(0)]
        public decimal PriceProfit { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateExpired { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}