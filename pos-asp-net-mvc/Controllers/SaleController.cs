using pos_asp_net_mvc.Entities;
using pos_asp_net_mvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace pos_asp_net_mvc.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        const int TRANSACTION_TYPE = 1;
        const int TRANSACTION_UNPAID = 0;
        const int TRANSACTION_PAID = 1;
        const String TRANSACTION_CODE = "SLS";

        const String PAGE_NAME = "Purchase";
        const String MESSAGE_SAVED = "Record created successfully";
        const String MESSAGE_UPDATED = "Record updated successfully";
        const String MESSAGE_DELETED = "Record deleted successfully";

        [HttpGet]
        public ActionResult DataTable()
        {
            var draw = Request.Params["draw"] == null ? "0" : Request.Params["draw"];
            var start = Request.Params["start"] == null ? "0" : Request.Params["start"];
            var rowperpage = Request.Params["length"] == null ? "10" : Request.Params["length"];
            var columnIndex = Request.Params["order[0][column]"] == null ? "0" : Request.Params["order[0][column]"];
            var columnName = Request.Params["columns[" + columnIndex + "][name]"] == null ? "0" : Request.Params["columns[" + columnIndex + "][name]"];
            var columnSortOrder = Request.Params["order[0][dir]"] == null ? "desc" : Request.Params["order[0][dir]"];
            var searchValue = Request.Params["search[value]"] == null ? "" : Request.Params["search[value]"];

            var actions = "";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Details", "Purchase", new { id = 0 }) + "' class='btn btn-success btn-sm btn-show'><i class='fa fa-search'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Edit", "Purchase", new { id = 0 }) + "' class='btn btn-info btn-sm btn-edit'><i class='fa fa-edit'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Delete", "Purchase", new { id = 0 }) + "' class='btn btn-danger btn-sm btn-delete'><i class='fa fa-trash'></i></a>";


            var getData = (from items in db.Transactions
            select new
            {
                id = items.Id,
                transaction_invoice_date = SqlFunctions.DatePart("yyyy", items.InvoiceDate) + "-" + SqlFunctions.DatePart("MM", items.InvoiceDate) + "-" + SqlFunctions.DatePart("dd", items.InvoiceDate),
                transaction_invoice_number = items.InvoiceNumber,
                customer_name = items.Customer.Name,
                transaction_grandtotal = items.GrandTotal,
                transaction_status = items.Status,
                key_id = items.Id,
                type_of = items.TypeOf,
                action = actions
            }
            ).Where(x => x.type_of == TRANSACTION_TYPE);
            var totalRecord = db.Transactions.Where(x => x.TypeOf == TRANSACTION_TYPE).Count();
            var getFilter = getData;

            if (!String.IsNullOrEmpty(searchValue) && searchValue.Length > 0)
            {
                getFilter = getData.Where(
                    x => x.transaction_invoice_date.Contains(searchValue) ||
                    x.transaction_invoice_number.Contains(searchValue) ||
                    x.customer_name.Contains(searchValue)
               );
            }

            var totalRecordFiltered = getFilter.Count();
            var getDataOrder = getFilter;

            if (columnSortOrder.Equals("desc"))
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.transaction_invoice_date); break;
                    case 1: getDataOrder = getFilter.OrderByDescending(x => x.transaction_invoice_number); break;
                    case 2: getDataOrder = getFilter.OrderByDescending(x => x.customer_name); break;
                    case 3: getDataOrder = getFilter.OrderByDescending(x => x.transaction_grandtotal); break;
                    case 4: getDataOrder = getFilter.OrderByDescending(x => x.transaction_status); break;
                    default: getDataOrder = getFilter.OrderByDescending(x => x.key_id); break;
                }
            }
            else
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderBy(x => x.transaction_invoice_date); break;
                    case 1: getDataOrder = getFilter.OrderBy(x => x.transaction_invoice_number); break;
                    case 2: getDataOrder = getFilter.OrderBy(x => x.customer_name); break;
                    case 3: getDataOrder = getFilter.OrderBy(x => x.transaction_grandtotal); break;
                    case 4: getDataOrder = getFilter.OrderBy(x => x.transaction_status); break;
                    default: getDataOrder = getFilter.OrderBy(x => x.key_id); break;
                }
            }

            var aaData = getDataOrder.Skip(int.Parse(start)).Take(int.Parse(rowperpage)).ToList();

            return Json(new { draw = draw, iTotalRecords = totalRecord, iTotalDisplayRecords = totalRecordFiltered, aaData = aaData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            try
            {
                var dateIndex = DateTime.Now.Date.ToString("yyyyMMdd");
                var invoiceNumber = TRANSACTION_CODE + "." + dateIndex + ".00001";
                var dateNow = DateTime.Now.Date;
                var lastTransactions = db.Transactions.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) == DbFunctions.TruncateTime(dateNow) && x.TypeOf == TRANSACTION_TYPE).OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastTransactions != null)
                {
                    var tempInvoice = lastTransactions.InvoiceNumber;
                    var arrInvoice = tempInvoice.Split('.');
                    var lastIndex = arrInvoice.Last();
                    var nextVal = int.Parse(lastIndex) + 1;
                    var nextValStr = nextVal.ToString();
                    int digit = 5;
                    int i_number = nextValStr.Length;
                    for (int i = digit; i > i_number; i--)
                    {
                        nextValStr = "0" + nextValStr;
                    }
                    invoiceNumber = TRANSACTION_CODE + "." + dateIndex + "." + nextValStr;
                }

                var identityName = User.Identity.Name;
                var currentUser = db.Users.Where(x => x.Email.Equals(identityName) || x.UserName.Equals(identityName)).FirstOrDefault();
                var user_id = currentUser.Id;

                Transaction tr = new Transaction();
                tr.TypeOf = TRANSACTION_TYPE;
                tr.Status = TRANSACTION_UNPAID;
                tr.InvoiceNumber = invoiceNumber;
                tr.InvoiceDate = DateTime.Now.Date;
                tr.UserId = user_id;
                tr.TotalItems = 0;
                tr.SubTotal = 0;
                tr.Discount = 0;
                tr.Tax = 0;
                tr.GrandTotal = 0;
                tr.Cash = 0;
                tr.Change = 0;
                tr.Notes = "";
                tr.CreatedAt = DateTime.Now;
                tr.UpdatedAt = DateTime.Now;

                db.Transactions.Add(tr);
                await db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_SAVED;
                return RedirectToAction("Edit", new { id = tr.Id });
            }
            catch (DbEntityValidationException e)
            {

                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction model = await db.Transactions.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Transaction model)
        {

            int totalItems = 0;

            var _product = Request.Params["product_id[]"];
            var _price = Request.Params["price[]"];
            var _qty = Request.Params["qty[]"];
            var _total = Request.Params["total[]"];

            if (_product != null && _price != null && _qty != null && _total != null)
            {
                var products = _product.Split(',');
                var price = _price.Split(',');
                var qty = _qty.Split(',');
                var total = _total.Split(',');

                for (int i = 0; i < products.Length; i++)
                {
                    if (products[i] != null && price[i] != null && qty[i] != null && total[i] != null)
                    {
                        var dt = new TransactionDetail();
                        dt.TransactionId = model.Id;
                        dt.ProductId = int.Parse(products[i]);
                        dt.Price = float.Parse(price[i]);
                        dt.Qty = int.Parse(qty[i]);
                        dt.Total = float.Parse(total[i]);
                        dt.UpdatedAt = DateTime.Now;
                        db.TransactionDetails.Add(dt);

                        var prd_id = int.Parse(products[i]);
                        var prd = db.Products.Where(x => x.Id == prd_id).FirstOrDefault();
                        prd.Stock = prd.Stock - int.Parse(qty[i]);
                        db.Entry(prd).State = EntityState.Modified;

                        totalItems = totalItems + int.Parse(qty[i]);

                    }
                }

            }


            var tr = db.Transactions.Where(x => x.Id == model.Id).FirstOrDefault();
            tr.CustomerId = model.CustomerId;
            tr.UpdatedAt = DateTime.Now;
            tr.Status = TRANSACTION_PAID;
            tr.GrandTotal = model.GrandTotal;
            tr.TotalItems = totalItems;
            tr.UpdatedAt = DateTime.Now;
            db.Entry(tr).State = EntityState.Modified;
            await db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_UPDATED;
            return RedirectToAction("Details", new { id = model.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction model = await db.Transactions.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Details = db.TransactionDetails.Where(x => x.TransactionId == id).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Print(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction model = await db.Transactions.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Details = db.TransactionDetails.Where(x => x.TransactionId == id).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction model = await db.Transactions.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            db.Transactions.Remove(model);
            await db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_DELETED;
            return RedirectToAction("Index");
        }

    }
}