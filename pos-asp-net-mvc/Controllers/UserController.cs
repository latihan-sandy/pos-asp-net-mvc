using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
using System.Web.Helpers;
using System.Web.Mvc;

namespace pos_asp_net_mvc.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        const String PAGE_NAME = "User";
        const String MESSAGE_SAVED = "Record created successfully";
        const String MESSAGE_UPDATED = "Record updated successfully";
        const String MESSAGE_DELETED = "Record deleted successfully";


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


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
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Details", "User", new { id = 0 }) + "' class='btn btn-success btn-sm btn-show'><i class='fa fa-search'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Edit", "User", new { id = 0 }) + "' class='btn btn-info btn-sm btn-edit'><i class='fa fa-edit'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Delete", "User", new { id = 0 }) + "' class='btn btn-danger btn-sm btn-delete'><i class='fa fa-trash'></i></a>";

            var identityName = User.Identity.Name;
            var currentUser = db.Users.Where(x => x.Email.Equals(identityName) || x.UserName.Equals(identityName)).FirstOrDefault();
            var user_id = currentUser.Id;

            var getData = (from items in db.Users
                           select new
                           {
                               id = items.Id,
                               user_username = items.UserName,
                               user_email = items.Email,
                               user_phone = items.PhoneNumber,
                               key_id = items.Id,
                               action = actions
                           }
            ).Where(x => !x.id.Equals(user_id));

            var totalRecord = db.Users.Where(x => !x.Id.Equals(user_id)).Count();
            var getFilter = getData;

            if (!String.IsNullOrEmpty(searchValue) && searchValue.Length > 0)
            {
                getFilter = getData.Where(x => x.user_username.Contains(searchValue) || x.user_email.Contains(searchValue) || x.user_phone.Contains(searchValue));
            }

            var totalRecordFiltered = getFilter.Count();
            var getDataOrder = getFilter;

            if (columnSortOrder.Equals("desc"))
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.user_username); break;
                    case 1: getDataOrder = getFilter.OrderByDescending(x => x.user_email); break;
                    case 2: getDataOrder = getFilter.OrderByDescending(x => x.user_phone); break;
                    default: getDataOrder = getFilter.OrderByDescending(x => x.key_id); break;
                }
            }
            else
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.user_username); break;
                    case 1: getDataOrder = getFilter.OrderByDescending(x => x.user_email); break;
                    case 2: getDataOrder = getFilter.OrderByDescending(x => x.user_phone); break;
                    default: getDataOrder = getFilter.OrderBy(x => x.key_id); break;
                }
            }

            var aaData = getDataOrder.Skip(int.Parse(start)).Take(int.Parse(rowperpage)).ToList();

            return Json(new { draw = draw, iTotalRecords = totalRecord, iTotalDisplayRecords = totalRecordFiltered, aaData = aaData, user_id = user_id }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Roles = db.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if(model.Roles != null)
                    {
                        foreach(var row in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, row);
                        }
                    }

                    TempData["message_success"] = MESSAGE_SAVED;
                    return RedirectToAction("Details", new { id = user.Id });
                }
                AddErrors(result);
            }

            ViewBag.Roles = db.Roles.ToList();
            return View(model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }



        [HttpGet]
        public ActionResult Details(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser model = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }

            var roleUserId = model.Roles.Select(x => x.RoleId).ToArray();
            var roleUser = db.Roles.Where(x => roleUserId.Contains(x.Id)).ToList();
            ViewBag.Roles = roleUser;
            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();

            var roleUserId = user.Roles.Select(x => x.RoleId).ToArray();
            var roleUser = db.Roles.Where(x => roleUserId.Contains(x.Id)).Select(x => x.Name).ToList();

            UserEditViewModel model = new UserEditViewModel();
            model.Id = user.Id;
            model.Username = user.UserName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.Roles = roleUser;
            ViewBag.Roles = db.Roles.ToList();
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);
                var roleUserId = user.Roles.Select(x => x.RoleId).ToArray();
                var roleUser = db.Roles.Where(x => roleUserId.Contains(x.Id)).Select(x => x.Name).ToArray();

                UserManager.RemoveFromRoles(model.Id, roleUser);


                user.UserName = model.Username;
                user.Email = model.Email;

                if (!String.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber;
                }

                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = Crypto.Hash(model.Password);
                }

                var result = await UserManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    if (model.Roles != null)
                    {

                       
                        foreach (var row in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, row);
                        }
                    }

                    TempData["message_success"] = MESSAGE_UPDATED;
                    return RedirectToAction("Details", new { id = user.Id });
                }
                AddErrors(result);

            }

            ViewBag.Roles = db.Roles.ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser model = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            db.Users.Remove(model);
            db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_DELETED;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async new Task<ActionResult> Profile()
        {
            var identityName = User.Identity.Name;
            var currentUser = db.Users.Where(x => x.Email.Equals(identityName) || x.UserName.Equals(identityName)).FirstOrDefault();
            var id = currentUser.Id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id);

            var roleUserId = user.Roles.Select(x => x.RoleId).ToArray();
            var roleUser = db.Roles.Where(x => roleUserId.Contains(x.Id)).Select(x => x.Name).ToList();

            UserEditViewModel model = new UserEditViewModel();
            model.Id = user.Id;
            model.Username = user.UserName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async new Task<ActionResult> Profile(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);
                user.UserName = model.Username;
                user.Email = model.Email;

                if (!String.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber;
                }

                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = Crypto.Hash(model.Password);
                }

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    TempData["message_success"] = "Yout profile has been changed";
                    return RedirectToAction("Profile");
                }
                AddErrors(result);

            }
            return View(model);
        }

    }
}