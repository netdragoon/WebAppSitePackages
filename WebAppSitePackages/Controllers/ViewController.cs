using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        private IEnumerable<KeyValuePair<object, object>> Itens()
        {
            Array itens = Enum.GetValues(typeof(ZipCodeUF));
            IEnumerator collection = itens.GetEnumerator();
            while (collection.MoveNext())
            {                
                yield return new KeyValuePair<object, object>(collection.Current, collection.Current);
            }
        }
    }
}