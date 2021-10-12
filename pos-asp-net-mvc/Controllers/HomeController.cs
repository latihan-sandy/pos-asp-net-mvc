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
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Dashboard()
        {

            var i_product = db.Products.Count();
            var i_brand = db.Brands.Count();
            var i_supplier = db.Suppliers.Count();
            var i_customer = db.Customers.Count();
            var purchase = PieChart(0);
            var sale = PieChart(1);
            var chartPurchase = BarChart(0);
            var chartSale = BarChart(1);

            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("product", i_product);
            data.Add("brand", i_brand);
            data.Add("supplier", i_supplier);
            data.Add("customer", i_customer);
            data.Add("sale", purchase);
            data.Add("purchase", sale);
            data.Add("chartPurchase", chartPurchase);
            data.Add("chartSale", chartSale);


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<object> PieChart(int type_transaction)
        {
            List<object> result = new List<object>();

            var cultureInfo = CultureInfo.InvariantCulture;
            var _firstDate = new DateTime(DateTime.Now.Year, 1, 1);
            var _lastDate = new DateTime(DateTime.Now.Year, 12, 31);
            var i_firstDate = DateTime.ParseExact(_firstDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", cultureInfo);
            var i_lastDate = DateTime.ParseExact(_lastDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", cultureInfo);

            var getData = (from items in db.TransactionDetails
                select new
                {
                    ProductName = items.Product.Name,
                    GrandTotal = items.Total,
                    Status = items.Transaction.Status,
                    InvoiceDate = items.Transaction.InvoiceDate.Value,
                    TypeOf = items.Transaction.TypeOf
                }
           )
            .Where(x => x.Status == 1 && x.TypeOf == type_transaction && (DbFunctions.TruncateTime(x.InvoiceDate) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate) <= i_lastDate))
            .GroupBy(l => new { l.ProductName, l.InvoiceDate, l.Status })
            .ToList();

            foreach(var row in getData)
            {
                result.Add(new
                {
                    name = row.First().ProductName,
                    total = row.First().GrandTotal
                });
            }

            return result;
        }

        private List<float> BarChart(int type_transaction)
        {
            List<float> result = new List<float>();
            var cultureInfo = CultureInfo.InvariantCulture;

            for (int i = 1; i <= 12; i ++)
            {
                var _firstDate = new DateTime(DateTime.Now.Year, i, 1);
                var _lastDate = _firstDate.AddMonths(1).AddDays(-1);
                var i_firstDate = DateTime.ParseExact(_firstDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", cultureInfo);
                var i_lastDate = DateTime.ParseExact(_lastDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", cultureInfo);
                var transaction = db.Transactions.Where(x => x.Status == 1 && x.TypeOf == type_transaction && (DbFunctions.TruncateTime(x.InvoiceDate.Value) >= i_firstDate && DbFunctions.TruncateTime(x.InvoiceDate.Value) <= i_lastDate))
                    .Sum(x => (float?)x.GrandTotal) ?? 0f;

                result.Add(transaction);

            }

            return result;
        }

    }
}