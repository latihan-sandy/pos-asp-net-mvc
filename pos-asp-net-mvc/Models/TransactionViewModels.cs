using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pos_asp_net_mvc.Models
{
    public class Purchase2
    {
        public string SupplierName { get; set; }
        public int TotalItems { get; set; }
        public float GrandTotal { get; set; }
        public int Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int TypeOf { get; set; }
    }

    public class Purchase3
    {
        public string SupplierName { get; set; }
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public int TotalItems { get; set; }
        public float GrandTotal { get; set; }
        public int Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int TypeOf { get; set; }
    }

    public class Sale2
    {
        public string CustomerName { get; set; }
        public int TotalItems { get; set; }
        public float GrandTotal { get; set; }
        public int Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int TypeOf { get; set; }
    }

    public class Sale3
    {
        public string CustomerName { get; set; }
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public int TotalItems { get; set; }
        public float GrandTotal { get; set; }
        public int Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int TypeOf { get; set; }
    }
}