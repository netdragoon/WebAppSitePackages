using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Canducci.Zip;
using System.Collections;

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
            ViewBag.uf = new SelectList(Itens(), "Value", "Key", "SP");
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

        private IEnumerable<KeyValuePair<object, object>> Itens()
        {
            Array itens = Enum.GetValues(typeof(ZipCodeUF));
            IEnumerator collection = itens.GetEnumerator();
            while (collection.MoveNext())
            {                
                yield return new KeyValuePair<object, object>(collection.Current, collection.Current);
            }
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