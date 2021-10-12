using pos_asp_net_mvc.Entities;
using pos_asp_net_mvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace pos_asp_net_mvc.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        const String PAGE_NAME = "Product";
        const String MESSAGE_SAVED = "Record created successfully";
        const String MESSAGE_UPDATED = "Record updated successfully";
        const String MESSAGE_DELETED = "Record deleted successfully";


        [HttpGet]
        public ActionResult Select2()
        {
            var search = Request.Params["q"] == null ? "" : Request.Params["q"];
            var type = Request.Params["type"] == null ? "0" : Request.Params["type"];
            if(type == "0")
            {
                var getData = (from items in db.Products
                    select new
                    {
                        id = items.Id,
                        sku = items.Sku,
                        text = items.Name,
                        stock = items.Stock,
                        price = items.PricePurchase
                    }
                );
                getData = getData.Where(x => x.stock == 0);
                if (!String.IsNullOrEmpty(search) && search.Length > 0)
                {
                    getData = getData.Where(x => x.sku.Contains(search) || x.text.Contains(search));
                }
                getData = getData.OrderBy(x => x.text);
                var result = getData.Skip(0).Take(10).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var getData = (from items in db.Products
                    select new
                    {
                        id = items.Id,
                        sku = items.Sku,
                        text = items.Name,
                        stock = items.Stock,
                        price = items.PriceSale
                    }
                 );
                getData = getData.Where(x => x.stock > 0);
                if (!String.IsNullOrEmpty(search) && search.Length > 0)
                {
                    getData = getData.Where(x => x.sku.Contains(search) || x.text.Contains(search));
                }
                getData = getData.OrderBy(x => x.text);
                var result = getData.Skip(0).Take(10).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
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
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Details", "Product", new { id = 0 }) + "' class='btn btn-success btn-sm btn-show'><i class='fa fa-search'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Edit", "Product", new { id = 0 }) + "' class='btn btn-info btn-sm btn-edit'><i class='fa fa-edit'></i></a>";
            actions += "&nbsp;<a href='javascript:void(0);' data-route='" + Url.Action("Delete", "Product", new { id = 0 }) + "' class='btn btn-danger btn-sm btn-delete'><i class='fa fa-trash'></i></a>";


            var getData = (from items in db.Products
                           select new
                           {
                               id = items.Id,
                               product_sku = items.Sku,
                               product_name = items.Name,
                               product_stock = items.Stock,
                               key_id = items.Id,
                               action = actions
                           }
            );
            var totalRecord = db.Products.Count();
            var getFilter = getData;

            if (!String.IsNullOrEmpty(searchValue) && searchValue.Length > 0)
            {
                getFilter = getData.Where(x => x.product_name.Contains(searchValue) || x.product_sku.Contains(searchValue) || x.product_stock.ToString().Contains(searchValue));
            }

            var totalRecordFiltered = getFilter.Count();
            var getDataOrder = getFilter;

            if (columnSortOrder.Equals("desc"))
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderByDescending(x => x.product_sku); break;
                    case 1: getDataOrder = getFilter.OrderByDescending(x => x.product_name); break;
                    case 2: getDataOrder = getFilter.OrderByDescending(x => x.product_stock); break;
                    default: getDataOrder = getFilter.OrderByDescending(x => x.key_id); break;
                }
            }
            else
            {
                switch (int.Parse(columnIndex))
                {
                    case 0: getDataOrder = getFilter.OrderBy(x => x.product_sku); break;
                    case 1: getDataOrder = getFilter.OrderBy(x => x.product_name); break;
                    case 2: getDataOrder = getFilter.OrderBy(x => x.product_stock); break;
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
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id, Sku, Name, Image, BrandId, SupplierId, TypeId, Stock, PricePurchase, PriceSale, PriceProfit, DateExpired, Description, Notes")] Product model,
            int[] CategoryId = null,
            HttpPostedFileBase upload = null
        )
        {
            if (ModelState.IsValid)
            {
                var Product = new Product
                {
                    Sku = model.Sku,
                    Name = model.Name,
                    BrandId = model.BrandId,
                    SupplierId = model.SupplierId,
                    TypeId = model.TypeId,
                    Stock = model.Stock,
                    PricePurchase = model.PricePurchase,
                    PriceSale = model.PriceSale,
                    PriceProfit = model.PriceProfit,
                    DateExpired = model.DateExpired,
                    Description = model.Description,
                    Notes = model.Notes
                };

                if (upload != null && upload.ContentLength > 0)
                {
                    this.createDir("Uploads");
                    string extension = Path.GetExtension(upload.FileName).ToLower();
                    string fileName = createFileName() + "" + extension;
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    upload.SaveAs(path);
                    Product.Image = "Uploads/"+fileName;
                }
                else
                {
                    Product.Image = "Image/no-image.png";
                }

                db.Products.Add(Product);
                if (CategoryId != null)
                {
                    foreach (var id in CategoryId)
                    {
                        var category = db.Categories.Find(id);
                        category.Product.Add(Product);

                    }

                }
                await db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_SAVED;
                return RedirectToAction("Details", new { id = Product.Id });
            }

            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product model = await db.Products.FindAsync(id);
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
            Product model = await db.Products.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Brand = new SelectList(db.Brands, "Id", "Name", model.Brand.Id);
            ViewBag.Supplier = new SelectList(db.Suppliers, "Id", "Name", model.Supplier.Id);
            ViewBag.Type = new SelectList(db.Types, "Id", "Name", model.Type.Id);
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id, Sku, Name, Image, BrandId, SupplierId, TypeId, Stock, PricePurchase, PriceSale, PriceProfit, DateExpired, Description, Notes")] Product model,
            int[] CategoryId = null,
            HttpPostedFileBase upload = null
        )
        {
            if (ModelState.IsValid)
            {
                var Product = db.Products.Find(model.Id);
                Product.Sku = model.Sku;
                Product.Name = model.Name;
                Product.BrandId = model.BrandId;
                Product.SupplierId = model.SupplierId;
                Product.TypeId = model.TypeId;
                Product.Stock = model.Stock;
                Product.PricePurchase = model.PricePurchase;
                Product.PriceSale = model.PriceSale;
                Product.PriceProfit = model.PriceProfit;
                Product.DateExpired = model.DateExpired;
                Product.Description = model.Description;
                Product.Notes = model.Notes;

                if (upload != null && upload.ContentLength > 0)
                {
                    this.createDir("Uploads");
                    if (!String.IsNullOrWhiteSpace(Product.Image) && !Product.Image.Equals("Image/no-image.png"))
                    {
                        string pathExists = Path.Combine(Server.MapPath("~/"), Product.Image);
                        if (System.IO.File.Exists(pathExists))
                        {
                            System.IO.File.Delete(pathExists);
                        }
                    }

                    string extension = Path.GetExtension(upload.FileName).ToLower();
                    string fileName = createFileName() + "" + extension;
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    upload.SaveAs(path);
                    Product.Image = "Uploads/"+fileName;
                }

                var currentSelected = db.Products.Include("Categories")
                   .Single(x => x.Id == model.Id);
                currentSelected.Categories.Clear();

                if (CategoryId != null)
                {
                    foreach (var id in CategoryId)
                    {
                        var category = db.Categories.Find(id);
                        category.Product.Add(Product);
                    }

                }

                await db.SaveChangesAsync();
                TempData["message_success"] = MESSAGE_UPDATED;
                return RedirectToAction("Details", new { id = Product.Id });
            }

            ViewBag.Brand = new SelectList(db.Brands, "Id", "Name", model.Brand.Id);
            ViewBag.Supplier = new SelectList(db.Suppliers, "Id", "Name", model.Supplier.Id);
            ViewBag.Type = new SelectList(db.Types, "Id", "Name", model.Type.Id);
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product model = await db.Products.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrWhiteSpace(model.Image))
            {
                string path = Path.Combine(Server.MapPath("~/"), model.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            db.Products.Remove(model);
            await db.SaveChangesAsync();
            TempData["message_success"] = MESSAGE_DELETED;
            return RedirectToAction("Index");
        }

        private string createFileName()
        {
            return Guid.NewGuid().ToString();
        }

        private void createDir(string foldername)
        {
            string path = Path.Combine(Server.MapPath("~/"), foldername);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


        }


    }
}