using pos_asp_net_mvc.Entities;
using pos_asp_net_mvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pos_asp_net_mvc.Controllers
{
   
    [Authorize]
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            var firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDate = firstDate.AddMonths(1).AddDays(-1);
            ViewBag.FirstDate = firstDate.ToString("yyyy-MM-dd");
            ViewBag.LastDate = lastDate.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public ActionResult Purchase1()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var transaction = db.Transactions.Where(x => x.Status == 1 && x.TypeOf == 0 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate)).ToList();
            ViewBag.Transaction = transaction;
            return View();
           
        }

        [HttpGet]
        public ActionResult Purchase2()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var getData = (from items in db.Transactions
                select new
                {
                    SupplierName = items.Supplier.Name,
                    TotalItems = items.TotalItems,
                    GrandTotal = items.GrandTotal,
                    Status = items.Status,
                    InvoiceDate = items.InvoiceDate.Value,
                    TypeOf = items.TypeOf
                }
           )
            .Where(x => x.Status == 1 && x.TypeOf == 0 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate))
            .GroupBy(l => l.SupplierName)
            .ToList();

            List<Purchase2> result = new List<Purchase2>();
            foreach(var row in getData)
            {
                result.Add(new Models.Purchase2
                {
                    SupplierName = row.First().SupplierName,
                    TotalItems = row.First().TotalItems,
                    GrandTotal = row.First().GrandTotal,
                    Status = row.First().Status,
                    InvoiceDate = row.First().InvoiceDate,
                    TypeOf = row.First().TypeOf
                });
            }

            ViewBag.Transaction = result;
            return View();
        }

        [HttpGet]
        public ActionResult Purchase3()
        {

            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var getData = (from items in db.TransactionDetails
                select new
                {
                    SupplierName = items.Transaction.Supplier.Name,
                    BrandName = items.Product.Brand.Name,
                    TypeName = items.Product.Type.Name,
                    SKU = items.Product.Sku,
                    ProductName = items.Product.Name,
                    TotalItems = items.Qty,
                    GrandTotal = items.Total,
                    Status = items.Transaction.Status,
                    InvoiceDate = items.Transaction.InvoiceDate.Value,
                    TypeOf = items.Transaction.TypeOf
                }
           )
            .Where(x => x.Status == 1 && x.TypeOf == 0 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate))
            .GroupBy(l => new { l.SupplierName, l.BrandName, l.TypeName, l.ProductName, l.InvoiceDate, l.Status })
            .ToList();

            List<Purchase3> result = new List<Purchase3>();
            foreach (var row in getData)
            {
                result.Add(new Models.Purchase3
                {
                    SupplierName = row.First().SupplierName,
                    BrandName = row.First().BrandName,
                    TypeName = row.First().TypeName,
                    SKU = row.First().SKU,
                    ProductName = row.First().ProductName,
                    TotalItems = row.First().TotalItems,
                    GrandTotal = row.First().GrandTotal,
                    Status = row.First().Status,
                    InvoiceDate = row.First().InvoiceDate,
                    TypeOf = row.First().TypeOf,
                });
            }

            ViewBag.Transaction = result;
            return View();
        }


        [HttpGet]
        public ActionResult Sale1()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var transaction = db.Transactions.Where(x => x.Status == 1 && x.TypeOf == 1 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate)).ToList();
            ViewBag.Transaction = transaction;
            return View();

        }

        [HttpGet]
        public ActionResult Sale2()
        {

            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var getData = (from items in db.Transactions
                select new
                {
                    CustomerName = items.Customer.Name,
                    TotalItems = items.TotalItems,
                    GrandTotal = items.GrandTotal,
                    Status = items.Status,
                    InvoiceDate = items.InvoiceDate.Value,
                    TypeOf = items.TypeOf
                }
           )
            .Where(x => x.Status == 1 && x.TypeOf == 1 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate))
            .GroupBy(l => l.CustomerName)
            .ToList();

            List<Sale2> result = new List<Sale2>();
            foreach (var row in getData)
            {
                result.Add(new Models.Sale2
                {
                    CustomerName = row.First().CustomerName,
                    TotalItems = row.First().TotalItems,
                    GrandTotal = row.First().GrandTotal,
                    Status = row.First().Status,
                    InvoiceDate = row.First().InvoiceDate,
                    TypeOf = row.First().TypeOf
                });
            }

            ViewBag.Transaction = result;
            return View();
        }

        [HttpGet]
        public ActionResult Sale3()
        {

            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
            var firstDate = Request.Params["first"] != null ? Request.Params["first"] : _firstDate.ToString("yyyy-MM-dd");
            var lastDate = Request.Params["last"] != null ? Request.Params["last"] : _lastDate.ToString("yyyy-MM-dd");
            var i_firstDate = DateTime.ParseExact(firstDate, "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(lastDate, "yyyy-MM-dd", cultureInfo);

            var getData = (from items in db.TransactionDetails
                select new
                {
                    CustomerName = items.Transaction.Customer.Name,
                    BrandName = items.Product.Brand.Name,
                    TypeName = items.Product.Type.Name,
                    SKU = items.Product.Sku,
                    ProductName = items.Product.Name,
                    TotalItems = items.Qty,
                    GrandTotal = items.Total,
                    Status = items.Transaction.Status,
                    InvoiceDate = items.Transaction.InvoiceDate.Value,
                    TypeOf = items.Transaction.TypeOf
                }
           )
            .Where(x => x.Status == 1 && x.TypeOf == 1 && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate))
            .GroupBy(l => new { l.CustomerName, l.BrandName, l.TypeName, l.ProductName, l.InvoiceDate, l.Status })
            .ToList();

            List<Sale3> result = new List<Sale3>();
            foreach (var row in getData)
            {
                result.Add(new Models.Sale3
                {
                    CustomerName = row.First().CustomerName,
                    BrandName = row.First().BrandName,
                    TypeName = row.First().TypeName,
                    SKU = row.First().SKU,
                    ProductName = row.First().ProductName,
                    TotalItems = row.First().TotalItems,
                    GrandTotal = row.First().GrandTotal,
                    Status = row.First().Status,
                    InvoiceDate = row.First().InvoiceDate,
                    TypeOf = row.First().TypeOf,
                });
            }

            ViewBag.Transaction = result;
            return View();
        }
    }
}