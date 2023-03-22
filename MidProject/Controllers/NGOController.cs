using MidProject.Auth;
using MidProject.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MidProject.Controllers
{
    [Logged]
    public class NGOController : Controller
    {
        // GET: NGO
        public ActionResult Index()
        {
            return View();
        }
        [NGOAccess]
        public ActionResult CollectRequests()
        {
            var db = new NGOEntities();
            var pending = (from cr in db.CollectRequests where cr.Status == "Pending" select cr).ToList();
            return View(pending); 
        }
        [NGOAccess]
        public ActionResult AssignEmployees()
        {
            var db = new NGOEntities();
            var Accepted = (from cr in db.CollectRequests where cr.Status == "Accepted" select cr).ToList();
            return View(Accepted);
        }

        [NGOAccess]
        public ActionResult EmployeeDetails(int id)
        {
            var db = new NGOEntities();
            var employee = (from cr in db.CollectRequests
                            join e in db.Employees on cr.EmployeeID equals e.ID
                            where cr.ID == id
                            select e).FirstOrDefault();
            return View(employee);
        }
        [NGOAccess]
        public ActionResult AssignEmployee(int id)
        {
            var db = new NGOEntities();
            var employees = db.Employees.ToList();
            TempData["id"] = id;
            return View(employees);
        }
        /*public ActionResult AssigningEmployee(int id)
        {

        }*/
        [NGOAccess]
        public ActionResult AssigningEmployee(int id)
        {
            var db = new NGOEntities();
            int requestId = (int)TempData["id"];
            // get the request with the specified ID
            var request = db.CollectRequests.FirstOrDefault(r => r.ID == requestId);

            // get the employee with the specified ID
            var employee = db.Employees.FirstOrDefault(e => e.ID == id);

            if (request != null && employee != null)
            {
                employee.AssignData = DateTime.Now;
                // assign the employee to the request
                request.Employee = employee;

                // update the corresponding foreign key in the CollectRequests table
                request.EmployeeID = id;
                request.Status = "Accepted";

                // save the changes to the database
                db.SaveChanges();

                // return a success message
                return RedirectToAction("CollectRequests", "NGO");
            }

            // if either the request or employee is not found, return an error message
            return Content("Error: Request or Employee not found.");
        }
        [NGOAccess]
        public ActionResult NGOProfile() 
        {
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var profile = db.Admins.Where(x => x.LoginID.Equals(userID)).SingleOrDefault();
                    return View(profile);
                }
                else
                {
                    return RedirectToAction("Login", "Home"); // Redirect to login page if user ID is not available or invalid
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return View("Error", new HandleErrorInfo(ex, "ShowProfile", "NGO"));
            }
        }
        [NGOAccess]
        public ActionResult EditProfile(int id)
        {
            using (var db = new NGOEntities())
            {
                var profile = db.Admins.Find(id);
                if (profile == null)
                {
                    return HttpNotFound();
                }
                return View(profile);
            }
        }
        [HttpPost]
        [NGOAccess]
        public ActionResult EditProfile(Restaurant restaurant)
        {
            var db = new NGOEntities();
            var existingRestaurant = db.Admins.Find(restaurant.ID);
            existingRestaurant.Name = restaurant.Name;
            existingRestaurant.Email = restaurant.Email;
            existingRestaurant.Contact = restaurant.Contact;
            db.SaveChanges();
            return RedirectToAction("NGOProfile", "NGO");
        }
        [NGOAccess]
        public ActionResult RequestHistory() 
        {
            var db = new NGOEntities();
            var history = (from cr in db.CollectRequests where cr.Status == "Completed" select cr).ToList();
            return View(history);
        }
        [NGOAccess]
        public ActionResult Logout()
        {
            // Clear the session
            Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login", "Home");
        }

    }
}