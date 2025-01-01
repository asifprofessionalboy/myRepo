using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace Ecommerce_Project.Controllers
{
    public class HomeController : Controller
    {

        private readonly EStoreContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(EStoreContext context, IWebHostEnvironment webHostEnvironment)
        {

            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var ProList = context.Products.ToList();
            context.SaveChanges();
            return View(ProList);

        }

        public IActionResult Wishlist()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Checkout(string[] cartItemNames, decimal[] cartItemPrices, int[] cartItemQuantities)
        //{
        //    // Save cart item details to the database



        //    var user =  HttpContext.Session.GetInt32("UserId");
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Home"); // Redirect to login if user is not logged in
        //    }
        //    var orderDate = DateTime.Now;


        //    for (int i = 0; i < cartItemNames.Length; i++)
        //    {
        //        var order = new OrderTbl
        //        {
        //            // Set other properties such as UserId, OrderDate, etc.

        //            UserId = user,
        //            ProductName = cartItemNames[i],

        //            TotalAmount = cartItemPrices[i],
        //            Quantity = cartItemQuantities[i]
        //        };

        //        // Save the order to the database using your context
        //        context.OrderTbls.Add(order);
        //    }


        //    context.SaveChanges();
        //    ViewBag.checkoutmsg = "Your order has been placed successfully";

        //    // Redirect or return appropriate response
        //    return View();
        //}


        [HttpPost]
        public IActionResult Checkout(string[] cartItemNames, decimal[] cartItemPrices, int[] cartItemQuantities)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var orderDate = DateTime.Now;

            if (cartItemNames != null && cartItemPrices != null && cartItemQuantities != null
                && cartItemNames.Length == cartItemPrices.Length && cartItemPrices.Length == cartItemQuantities.Length)
            {
                for (int i = 0; i < cartItemNames.Length; i++)
                {
                    var order = new OrderTbl
                    {
                        UserId = user.Value,
                        ProductName = cartItemNames[i],
                        TotalAmount = cartItemPrices[i] * cartItemQuantities[i],
                        Quantity = cartItemQuantities[i],
                        OrderDate = orderDate
                    };

                    context.OrderTbls.Add(order);
                }

                context.SaveChanges();

                // Clear the cart after checkout
              //  HttpContext.Session.Remove("Cart");

                ViewBag.checkoutmsg = "Your order has been placed successfully";
            }
            else
            {
                ViewBag.checkoutmsg = "Error processing your order. Please try again.";
            }

            return View();
        }


        public IActionResult ClearCart()
        {
            // Clear the session cart
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index", "Home");
        }













        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserTbl user)
        {
            var data = context.UserTbls.FirstOrDefault(x => x.Mobile == user.Mobile && x.Password == user.Password);
            if (data != null)
            {
                HttpContext.Session.SetInt32("UserId", data.Uid);

                if (data.IsAdmin)
                {
                    HttpContext.Session.SetString("MySession", data.Mobile);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    HttpContext.Session.SetString("MySession", data.Mobile);
                    return RedirectToAction("Checkout", "Home");
                }
            }
            else
            {
                ViewBag.LogMsg = "Login Failed..!";
                return View();
            }
        }


        public IActionResult Register()
        {

            return View();
        }


    

        [HttpPost]
        public async Task<IActionResult> Register(UserTbl user)
        {
            if(ModelState.IsValid)
            {
                await context.UserTbls.AddAsync(user);
                await context.SaveChangesAsync();
                 ViewBag.RegMsg = "Register successfully";
                 //return RedirectToAction("Login");
            }
            return View();
        }



        public IActionResult UserList()
        {

            if (HttpContext.Session.GetString("MySession") != null)
            {
                var UserList = context.UserTbls.ToList();

                return View(UserList);
            }
            else
            {
                return RedirectToAction("Login");
            }
         
        }

      
        public IActionResult EditUser(int id)
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var edituser = context.UserTbls.Find(id);
            return View(edituser);
			}
			else
			{
				return RedirectToAction("Login");
			}

		}

        [HttpPost]
        public IActionResult EditUser(int id , UserTbl user)
        {
            context.UserTbls.Update(user);
            context.SaveChanges();
            return RedirectToAction("UserList");
        }




        public IActionResult Dashboard()
        {
            

       

            if (HttpContext.Session.GetInt32("UserId") != null)
            {
				var Datacount = context.Products.ToList();
				ViewBag.productcount = Datacount.Count();

				var Datacount2 = context.UserTbls.ToList();
				ViewBag.usercount = Datacount2.Count();


				var Datacount3 = context.OrderTbls.ToList();
				ViewBag.ordercount = Datacount3.Count();


				//var DataSum = (from Data in context.MaterialTbls select Data.Price).Sum();
				//ViewBag.Sdata = DataSum;
				return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }

    
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                HttpContext.Session.Remove("UserId");
                return RedirectToAction("Login");
            }
                return View();
        }



        public IActionResult AddProduct()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                        List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                        ViewBag.data = Status;
                        return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            Product s = new Product
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.Products.Add(s);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }



        public IActionResult ProductList()
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{

				var ProList =  context.Products.ToList();
            context.SaveChanges();
            return View(ProList);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

        public IActionResult DeleteProduct(int id)
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{

				var proId =  context.Products.Find(id);
            return View(proId);
		}
			else
			{
				return RedirectToAction("Login");
	}
}

        [HttpPost]
        public IActionResult DeleteProduct(Product pro, int id)
        {
            context.Products.Remove(pro);
            context.SaveChanges();
           // ViewBag.Prodltmsg = "One Product Removed.. ";
            return RedirectToAction("ProductList");
          
        }

        public IActionResult EditProduct(int id)
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var editId = context.Products.Find(id);

            return View(editId);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

        [HttpPost]
        public IActionResult EditProduct(Product pro,int id)
        {
            context.Products.Update(pro);
            context.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public IActionResult DetailProduct(int id)
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var proDetail = context.Products.FirstOrDefault(x => x.Pid == id);
            return View(proDetail);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}


        public IActionResult EditOption(int id)
        {
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var editOption = context.Options.Find(id);
               return  View(editOption);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

        [HttpPost]
        public IActionResult EditOption(Option opt, int id)
        {
            context.Options.Update(opt);
            context.SaveChanges();
            ViewBag.opmsg = "Updated..";
           // return RedirectToAction("EditOption");
           return View();
        }

        public IActionResult OptionList()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View(context.Options.ToList());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public async Task<IActionResult> Orderlist()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var OrderList = context.OrderTbls.ToList();
                return View(OrderList);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpGet]
        public IActionResult SingleProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SingleProduct(int pid, string title, int price, int qty, string pic, int status)
        {
            var productViewModel = new ProductViewModel
            {
                Pid = pid,
                Title = title,
                Price = price,
                Qty = qty,
                Pic = pic,
                Status = status
            };

            return View(productViewModel);
        }



        public IActionResult AddReview()
        {
            return View();
        }

      // Action method to handle form submission and insert new review
      [HttpPost]
        public IActionResult AddReview(Review rev)
        {
            if (ModelState.IsValid)
            {
                 context.Reviews.AddAsync(rev);
                 context.SaveChangesAsync();
                ViewBag.RegMsg = "Submitted successfully";
                //return RedirectToAction("Login");
                
            }
            return View();
        }


        public IActionResult InOrganic()
        {
            
            return View(context.InOrganicProducts.ToList());
        }

        public IActionResult Organic()
        {
            return View(context.OrganicProducts.ToList());
        }

        //----------------------MensWear---------------
        public IActionResult AddMensWear()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddMensWear(MensWearViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            MensWearProduct m = new MensWearProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.MensWearProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }

    

        public IActionResult MensWear()
        {
            return View(context.MensWearProducts.ToList());
        }

        //----------------------End---------------






        //----------------------WoMensWear---------------
        public IActionResult AddWoMensWear()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddWoMensWear(WoMensWearViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            WmensWearProduct W = new WmensWearProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.WmensWearProducts.Add(W);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }

      


        public IActionResult WomensWear()
        {
            return View(context.WmensWearProducts.ToList());
        }

        //----------------------End---------------






        //----------------------MensCos---------------
        public IActionResult AddMensCos()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddMensCos(MensCosViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            MensCosmoticsProduct c = new MensCosmoticsProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.MensCosmoticsProducts.Add(c);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }

       


        public IActionResult MensCosmotics()
        {
            return View(context.MensCosmoticsProducts.ToList());
        }

        //----------------------End---------------







        //----------------------WomensCos---------------





        public IActionResult AddWomensCos()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddWomensCos(WomensCosViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            WoMensCosmoticsProduct m = new WoMensCosmoticsProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.WoMensCosmoticsProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }

    




        public IActionResult WomensCosmotics()
        {
            return View(context.WoMensCosmoticsProducts.ToList());
        }

        //----------------------End---------------











        //----------------------start---------------
        public IActionResult AddHome()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddHome(HomeViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            HomeProduct m = new HomeProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.HomeProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }





        public IActionResult HomeAppliances()
        {
            return View(context.HomeProducts.ToList());
        }


        //----------------------End---------------









        public IActionResult AddMob()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddMob(MobViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            MobProduct m = new MobProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.MobProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }





        public IActionResult MobLap()
        {
            return View(context.MobProducts.ToList());
        }








        public IActionResult AddOrganic()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddOrganic(MobViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            OrganicProduct m = new OrganicProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.OrganicProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }




        public IActionResult AddInOrganic()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<SelectListItem> Status = new List<SelectListItem>()
                    {
                        new SelectListItem {Value="1", Text="Active"},
                        new SelectListItem {Value="2", Text="Deactive"}
                    };

                ViewBag.data = Status;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult AddIInOrganic(MobViewModel pvm)
        {
            String filename = "";
            if (pvm.Photo != null)
            {
                String uploadflder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + pvm.Photo.FileName;
                String filepath = Path.Combine(uploadflder, filename);
                pvm.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            InOrganicProduct m = new InOrganicProduct
            {
                Title = pvm.Title,
                Price = pvm.Price,
                Qty = pvm.Qty,
                Status = pvm.Status,
                Pic = filename
            };
            context.InOrganicProducts.Add(m);
            context.SaveChanges();
            ViewBag.Promsg = "Product Added... ";
            return View();
        }



        public IActionResult Trending()
        {
            return View(context.Products.ToList());
        }


        public IActionResult Latest()
        {
            return View(context.Products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}