using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Canducci.Zip;
namespace WebAppSitePackages.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ZipCodeInfo zipcodeInfo = ZipCodeLoad.Find("19200000");
            ViewBag.ZipCodeInfo = zipcodeInfo;
            return View();
        }
    }
}