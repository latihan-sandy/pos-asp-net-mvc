using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Entities
{
    [Table("Categories")]
    public class Category
    {

        public Category()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

       
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}