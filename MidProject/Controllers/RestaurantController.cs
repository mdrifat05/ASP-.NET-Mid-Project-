using MidProject.Auth;
using MidProject.EF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MidProject.Controllers
{
    [Logged]
    public class RestaurantController : Controller
    {
        [RestaurantAccess]
        [HttpGet]
        public ActionResult CollectRequests()
        {
            return View();
        }
        //
        [RestaurantAccess]
        [HttpPost]
        public ActionResult CollectRequests(CollectRequest collectRequest, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var db = new NGOEntities();
                // Set the restaurant ID for the Collect Request
                collectRequest.LoginID = Int32.Parse(Session["User-ID"].ToString());
                collectRequest.RestaurantName = Session["User-Name"].ToString();

                // Save the file to the server
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                    file.SaveAs(filePath);
                    // Set the file name in the Collect Request object
                    collectRequest.Image = fileName;
                }

                // Add the Collect Request to the database
                db.CollectRequests.Add(collectRequest);
                db.SaveChanges();

                return RedirectToAction("RequestHistory", "Restaurant");
            }

            return View(collectRequest);
        }

        //[HttpPost]
        //public ActionResult CollectRequests(CollectRequest collectRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var db = new NGOEntities();
        //        // Set the restaurant ID for the Collect Request
        //        collectRequest.RestaurantID = Int32.Parse(Session["User-ID"].ToString());
        //        collectRequest.RestaurantName = Session["User-Name"].ToString();
        //        //collectRequest.RestaurantName= "KFC";
        //        // Add the Collect Request to the database
        //        db.CollectRequests.Add(collectRequest);
        //        db.SaveChanges();

        //        return RedirectToAction("Index", "Restaurant");
        //    }
        //    return View(collectRequest);
        //}
        [RestaurantAccess]
        public ActionResult RequestHistory()
        {
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User-ID"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var collectRequests = db.CollectRequests.Where(x => x.LoginID == userID).ToList();
                    return View(collectRequests);
                }
                else
                {
                    return RedirectToAction("Login", "Home"); // Redirect to login page if user ID is not available or invalid
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return View("Error", new HandleErrorInfo(ex, "RequestHistory", "Restaurant"));
            }
        }
        [RestaurantAccess]
        public ActionResult DeleteRequest(int id)
        {
            using (var db = new NGOEntities())
            {
                var request = db.CollectRequests.Find(id);
                if (request == null)
                {
                    return HttpNotFound();
                }
                db.CollectRequests.Remove(request);
                db.SaveChanges();
                return RedirectToAction("RequestHistory");
            }
        }
        [RestaurantAccess]
        public ActionResult ShowProfile()
        {
            try
            {
                var db = new NGOEntities();
                int userID;
                if (Session["User"] != null && int.TryParse(Session["User-ID"].ToString(), out userID))
                {
                    var profile = db.Restaurants.Where(x => x.LoginID.Equals(userID)).SingleOrDefault();
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
                return View("Error", new HandleErrorInfo(ex, "ShowProfile", "Restaurant"));
            }

        }
        [RestaurantAccess]
        public ActionResult EditProfile(int id)
        {
            using (var db = new NGOEntities())
            {
                var profile = db.Restaurants.Find(id);
                if (profile == null)
                {
                    return HttpNotFound();
                }
                return View(profile);
            }
        }
        [HttpPost]
        [RestaurantAccess]
        public ActionResult EditProfile(Restaurant restaurant)
        {
            var db = new NGOEntities();
            var existingRestaurant = db.Restaurants.Find(restaurant.ID);
            existingRestaurant.Name= restaurant.Name;
            existingRestaurant.Email = restaurant.Email;
            existingRestaurant.Contact= restaurant.Contact;
            db.SaveChanges();
            return RedirectToAction("ShowProfile","Restaurant");
        }
        [RestaurantAccess]
        public ActionResult Logout() 
        {
            // Clear the session
            Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login", "Home");
        }
    }
}