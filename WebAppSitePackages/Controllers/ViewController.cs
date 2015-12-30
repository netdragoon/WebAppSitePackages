using System.Web.Mvc;
using Canducci.Zip;
namespace WebAppSitePackages.Controllers
{
    [RoutePrefix("view")]
    public class ViewController : Controller
    {
        
        [Route("home")]
        public ActionResult Home()
        {
            return View();
        }

        [Route("zipcode")]
        public ActionResult ZipCode()
        {
            return View();
        }

        [Route("address")]
        public ActionResult Address()
        {                        
            ViewBag.uf = new SelectList(ZipCodeLoad.UfToList(), "Value", "Key", "SP");
            return View();
        }

        [Route("cotacao")]
        public ActionResult Cotacao()
        {
            return View();
        }

        [Route("forecast")]
        public ActionResult Forecast()
        {
            return View();
        }        

        [Route("gravatar")]
        public ActionResult Gravatar()
        {
            return View();
        }

        [Route("shorturl")]
        public ActionResult ShortUrl()
        {
            return View();
        }

        [Route("thumbnail")]
        public ActionResult Thumbnail()
        {
            return View();
        }
    }
}