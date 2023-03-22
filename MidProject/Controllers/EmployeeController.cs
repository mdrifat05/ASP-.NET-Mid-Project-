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
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User-ID"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var EmployeeCollectRequest = db.CollectRequests.Where(x => x.Employee.LoginID == userID && x.Status.Equals("Accepted")).ToList();
                    return View(EmployeeCollectRequest);
                }
                else
                {
                    return RedirectToAction("Login", "Home"); // Redirect to login page if user ID is not available or invalid
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return View("Error", new HandleErrorInfo(ex, "Login", "Home"));
            }
        }
        public ActionResult EmployeeDetails(int id)
        {
            var db = new NGOEntities();
            var employee = (from cr in db.CollectRequests
                            join e in db.Employees on cr.EmployeeID equals e.ID
                            where cr.ID == id
                            select e).FirstOrDefault();
            int userID = (int)Session["User-ID"];
            var Emp = db.Employees.Where(x => x.LoginID == userID).SingleOrDefault();
            Emp.CompletionDate = DateTime.Now;
            // get the request with the specified ID
            var request = db.CollectRequests.FirstOrDefault(r => r.ID == id);
            request.Status = "Completed";
            db.SaveChanges();
            return View(employee);

        }
        public ActionResult CompleteCollectRequest()
        {
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User-ID"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var EmployeeCollectRequest = db.CollectRequests.Where(x => x.Employee.LoginID == userID && x.Status.Equals("Completed")).ToList();
                    return View(EmployeeCollectRequest);
                }
                else
                {
                    return RedirectToAction("Login", "Home"); // Redirect to login page if user ID is not available or invalid
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return View("Error", new HandleErrorInfo(ex, "Login", "Home"));
            }
        }
        public ActionResult CompletionDate(int id)
        {
            var db = new NGOEntities();
            var employee = (from cr in db.CollectRequests
                            join e in db.Employees on cr.EmployeeID equals e.ID
                            where cr.ID == id
                            select e).FirstOrDefault();
            return View(employee);
        }
        // GET: Employee
        [EmployeeAccess]
        public ActionResult EmpolyeeProfile()
        {
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var profile = db.Employees.Where(x => x.LoginID.Equals(userID)).SingleOrDefault();
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
                return View("Error", new HandleErrorInfo(ex, "EmpolyeeProfile", "Employee"));
            }
        }
    }
}