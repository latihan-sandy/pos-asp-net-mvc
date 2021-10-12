using pos_asp_net_mvc.Entities;
using pos_asp_net_mvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace pos_asp_net_mvc.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        const String PAGE_NAME = "Customer";
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
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Details", "Customer", new { id = 0 }) + "' class='btn btn-success btn-sm btn-show'><i class='fa fa-search'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Edit", "Customer", new { id = 0 }) + "' class='btn btn-info btn-sm btn-edit'><i class='fa fa-edit'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Delete", "Customer", new { id = 0 }) + "' class='btn btn-danger btn-sm btn-delete'><i class='fa fa-trash'></i></a>";


            var getData = (from items in db.Customers
                           select new
                           {
                               id = items.Id,
                               customer_name = items.Name,
                               customer_phone = items.Phone,
                               customer_email = items.Email,
                               customer_website = items.Website,
                               customer_address = items.Address,
                               key_id = items.Id,
                               action = actions
                           }
            );
            var totalRecord = db.Customers.Count();
            var getFilter = getData;

            if (!String.IsNullOrEmpty(searchValue) && searchValue.Length > 0)
            {
                getFilter = getData.Where(
                    x => x.customer_name.Contains(searchValue) || 
                    x.customer_phone.Contains(searchValue) ||
                    x.customer_email.Contains(searchValue) ||
                    x.customer_website.Contains(searchValue) ||
                    x.customer_address.Contains(searchValue)
              );
            }

            var totalRecordFiltered = getFilter.Count();
            var getDataOrder = getFilter;

            if (columnSortOrder.Equals("desc"))
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.customer_name); break;
                    case 1: getDataOrder = getFilter.OrderByDescending(x => x.customer_phone); break;
                    case 2: getDataOrder = getFilter.OrderByDescending(x => x.customer_email); break;
                    default: getDataOrder = getFilter.OrderByDescending(x => x.key_id); break;
                }
            }
            else
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderBy(x => x.customer_name); break;
                    case 1: getDataOrder = getFilter.OrderBy(x => x.customer_phone); break;
                    case 2: getDataOrder = getFilter.OrderBy(x => x.customer_email); break;
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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Phone,Email,Website,Address,CreatedAt,UpdatedAt")] Customer model)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(model);
                await db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_SAVED;
                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer model = await db.Customers.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer model = await db.Customers.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Phone,Email,Website,Address,CreatedAt,UpdatedAt")] Customer model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedAt = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_UPDATED;
                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer model = await db.Customers.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            db.Customers.Remove(model);
            await db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_DELETED;
            return RedirectToAction("Index");
        }


    }
}