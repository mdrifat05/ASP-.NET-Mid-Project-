using MidProject.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MidProject.Auth
{
    public class RestaurantAccess : AuthorizeAttribute
    {
       
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var Restaurant = (Login)httpContext.Session["User"];
            if (Restaurant != null && Restaurant.Role.Equals("restaurant")) return true;
            return false;
        }
        
    }
}