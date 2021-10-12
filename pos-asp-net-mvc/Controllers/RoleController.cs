using Microsoft.AspNet.Identity.EntityFramework;
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
    public class RoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        const String PAGE_NAME = "Role";
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
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Details", "Role", new { id = 0 }) + "' class='btn btn-success btn-sm btn-show'><i class='fa fa-search'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Edit", "Role", new { id = 0 }) + "' class='btn btn-info btn-sm btn-edit'><i class='fa fa-edit'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Delete", "Role", new { id = 0 }) + "' class='btn btn-danger btn-sm btn-delete'><i class='fa fa-trash'></i></a>";


            var getData = (from items in db.Roles
                           select new
                           {
                               id = items.Id,
                               role_name = items.Name,
                               key_id = items.Id,
                               action = actions
                           }
            );
            var totalRecord = db.Roles.Count();
            var getFilter = getData;

            if (!String.IsNullOrEmpty(searchValue) && searchValue.Length > 0)
            {
                getFilter = getData.Where(x => x.role_name.Contains(searchValue));
            }

            var totalRecordFiltered = getFilter.Count();
            var getDataOrder = getFilter;

            if (columnSortOrder.Equals("desc"))
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.role_name); break;
                    default: getDataOrder = getFilter.OrderByDescending(x => x.key_id); break;
                }
            }
            else
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderBy(x => x.role_name); break;
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
        public ActionResult Create([Bind(Include = "Id,Name")] IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(model);
                db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_SAVED;
                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Details(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole model = db.Roles.Where(x => x.Id.Equals(id)).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole model = db.Roles.Where(x => x.Id.Equals(id)).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_UPDATED;
                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole model = db.Roles.Where(x => x.Id.Equals(id)).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            db.Roles.Remove(model);
            db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_DELETED;
            return RedirectToAction("Index");
        }


    }
}