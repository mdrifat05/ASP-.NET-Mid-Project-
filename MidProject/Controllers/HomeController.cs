using MidProject.Auth;
using MidProject.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MidProject.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RegistrationView()
        {
            return View();
        }
        //admin registration
        //[NGOAccess]
        public ActionResult AdminRegistration()
        {
            return View();
        }
        [NGOAccess]
        [HttpPost]  
        public ActionResult AdminRegistration(Admin admin, EF.Models.Login login)
        {
            if (ModelState.IsValid)
            {
                NGOEntities db = new NGOEntities();
                var user = new EF.Models.Login { Name =login.Name, Email = login.Email, Password = login.Password, Role = "admin" };
                db.Logins.Add(user);
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(admin);

        }
        //Restaurant Registration
        public ActionResult RestaurantRegistration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RestaurantRegistration(Restaurant restaurant, EF.Models.Login login)
        {
            if (ModelState.IsValid)
            {
                NGOEntities db = new NGOEntities();
                var userR = new EF.Models.Login { Name = login.Name, Email = login.Email, Password = login.Password, Role = "restaurant" };
                db.Logins.Add(userR);
                db.Restaurants.Add(restaurant);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(restaurant);
        }

        //Employee Registration
        public ActionResult EmployeeRegistration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EmployeeRegistration(Employee employee, EF.Models.Login login)
        {
            if (ModelState.IsValid)
            {
                NGOEntities db = new NGOEntities();
                var userE = new EF.Models.Login { Name = login.Name, Email = login.Email, Password = login.Password, Role = "employee" };
                db.Logins.Add(userE);
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(employee);
        }
        public ActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(EF.Models.Login login)
        {
            if (ModelState.IsValid)
            {
                NGOEntities db = new NGOEntities();
                var user = (from u in db.Logins
                            where u.Email.Equals(login.Email)
                            && u.Password.Equals(login.Password)
                            select u).SingleOrDefault();
                if (user != null)
                {
                    Session["User-ID"] = user.ID;
                    Session["User-Name"] = user.Name;
                    Session["User"] = user;

                    var retUrl = Request["ReturnUrl"];

                    // Use switch statement to check user role and redirect accordingly
                    switch (user.Role)
                    {
                        case "admin":
                            return RedirectToAction("CollectRequests", "NGO");
                        case "restaurant":
                            return RedirectToAction("CollectRequests", "Restaurant");
                        case "employee":
                            return RedirectToAction("Index", "Employee");
                        default:
                            return RedirectToAction("Login", "Home");
                    }
                }
            }
            TempData["Msg"] = "Email Password Invalid!";
            return View(login);
        }

        //public ActionResult Login(EF.Models.Login login)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        NGOEntities db = new NGOEntities();
        //        var user = (from u in db.Logins 
        //                    where u.Email.Equals(login.Email) 
        //                    && u.Password.Equals(login.Password) 
        //                    select u).SingleOrDefault();
        //        if (user != null)
        //        {
        //            Session["User-ID"] = user.ID;
        //            Session["User-Name"] = user.Name;
        //            Session["User"] = user;

        //            var retUrl = Request["ReturnUrl"];
        //            if (retUrl != null)
        //            {
        //                return Redirect(retUrl);
        //            }
        //            return RedirectToAction("CollectRequests", "Restaurant");
        //        }
        //    }
        //    TempData["Msg"] = "Email Password Invalid!";
        //    return View(login);
        //}
        public ActionResult Logout()
        {
            // Clear the session
            Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login", "Home");
        }
    }
}