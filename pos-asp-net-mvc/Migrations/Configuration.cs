namespace pos_asp_net_mvc.Migrations
{
    using Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    class RoleNames
    {
        public const string ROLE_ADMINISTRATOR = "Administrator";
        public const string ROLE_CONTRIBUTOR = "Contributor";
        public const string ROLE_READER = "Reader";
    }

    internal sealed class Configuration : DbMigrationsConfiguration<pos_asp_net_mvc.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(pos_asp_net_mvc.Models.ApplicationDbContext context)
        {
            try
            {
                int max = 100;

                SeedUser("admin@admin.com", context);
                for (int i = 1; i < max; i++)
                {
                    String Email = Faker.Internet.Email();
                    SeedUser(Email, context);
                }

                for (int i = 1; i <= max; i++)
                {
                    Brand b = new Brand();
                    b.Name = Faker.Lorem.Sentence(3);
                    b.Description = Faker.Lorem.Paragraph(10);
                    b.CreatedAt = DateTime.Now;
                    b.UpdatedAt = DateTime.Now;
                    context.Brands.Add(b);
                    context.SaveChanges();
                }

                for (int i = 1; i <= max; i++)
                {
                    Category c = new Category();
                    c.Name = Faker.Lorem.Sentence(3);
                    c.Description = Faker.Lorem.Paragraph(10);
                    c.CreatedAt = DateTime.Now;
                    c.UpdatedAt = DateTime.Now;
                    context.Categories.Add(c);
                    context.SaveChanges();
                }

                for (int i = 1; i <= max; i++)
                {
                    pos_asp_net_mvc.Entities.Type t = new pos_asp_net_mvc.Entities.Type();
                    t.Name = Faker.Lorem.Sentence(3);
                    t.Description = Faker.Lorem.Paragraph(10);
                    t.CreatedAt = DateTime.Now;
                    t.UpdatedAt = DateTime.Now;
                    context.Types.Add(t);
                    context.SaveChanges();
                }

                for (int i = 1; i <= max; i++)
                {
                    Customer c = new Customer();
                    c.Name = Faker.Name.FullName();
                    c.Phone = Faker.Phone.Number();
                    c.Email = Faker.Internet.Email();
                    c.Website = Faker.Internet.DomainName();
                    c.Address = Faker.Address.StreetAddress();
                    c.CreatedAt = DateTime.Now;
                    c.UpdatedAt = DateTime.Now;
                    context.Customers.Add(c);
                    context.SaveChanges();
                }

                for (int i = 1; i <= max; i++)
                {
                    Supplier s = new Supplier();
                    s.Name = Faker.Name.FullName();
                    s.Phone = Faker.Phone.Number();
                    s.Email = Faker.Internet.Email();
                    s.Website = Faker.Internet.DomainName();
                    s.Address = Faker.Address.StreetAddress();
                    s.CreatedAt = DateTime.Now;
                    s.UpdatedAt = DateTime.Now;
                    context.Suppliers.Add(s);
                    context.SaveChanges();
                }

                for (int i = 1; i <= max; i++)
                {
                    Supplier Supplier = context.Suppliers.OrderBy(x => Guid.NewGuid()).First();
                    pos_asp_net_mvc.Entities.Type Type = context.Types.OrderBy(x => Guid.NewGuid()).First();
                    Brand Brand = context.Brands.OrderBy(x => Guid.NewGuid()).First();
                    List<Category> Categories = context.Categories.OrderBy(x => Guid.NewGuid()).Take(10).ToList();

                    double PricePurchase = RandomNumber(1000, 9999);
                    double PriceProfit = RandomNumber(10, 100);
                    double PriceSale = PricePurchase + ((PricePurchase * PriceProfit) / 100);

                    Product p = new Product();
                    p.Sku = "PR" + Faker.RandomNumber.Next() + "" + DateTime.Now.Ticks;
                    p.Name = Faker.Address.StreetName();
                    p.Image = "Image/no-image.png";
                    p.BrandId = Brand.Id;
                    p.TypeId = Type.Id;
                    p.SupplierId = Supplier.Id;
                    p.Stock = 0;
                    p.PricePurchase = float.Parse(PricePurchase.ToString());
                    p.PriceProfit = decimal.Parse(PriceProfit.ToString());
                    p.PriceSale = float.Parse(PriceSale.ToString());
                    p.DateExpired = DateTime.Now;
                    p.Description = Faker.Lorem.Paragraph(10);
                    p.Notes = Faker.Lorem.Paragraph(10);
                    p.Categories = Categories;
                    p.CreatedAt = DateTime.Now;
                    p.UpdatedAt = DateTime.Now;
                    context.Products.Add(p);
                    context.SaveChanges();
                }

               
            }
            catch(Exception e)
            {
                
            }

        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private void SeedUser(String Email, pos_asp_net_mvc.Models.ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
            {
                var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
            }
            if (!roleManager.RoleExists(RoleNames.ROLE_CONTRIBUTOR))
            {
                var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_CONTRIBUTOR));
            }
            if (!roleManager.RoleExists(RoleNames.ROLE_READER))
            {
                var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_READER));
            }

            string password = "password";

            ApplicationUser user = userManager.FindByName(Email);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true
                };
                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, RoleNames.ROLE_ADMINISTRATOR);
                }
            }
        }

    }
}
