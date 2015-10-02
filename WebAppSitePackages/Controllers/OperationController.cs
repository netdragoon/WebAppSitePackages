using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Canducci.Zip;
using Canducci.QuoteDolar;
namespace WebAppSitePackages.Controllers
{
    [RoutePrefix("operation")]
    public class OperationController : Controller
    {
        #region ZipCodeOperation
        [Route("zipcode")]
        [HttpPost()]
        public JsonResult zipcode(string cep)
        {

            ZipCodeInfo data = CacheGet(cep);

            if (data == null)
            {

                data = ZipCodeLoad.Find(cep);
                
                CacheSave(cep, data);

            }

            return Json(data, JsonRequestBehavior.DenyGet);

        }

        private ZipCodeInfo CacheGet(string cep)
        {

            return (HttpRuntime.Cache.Get(cep) as ZipCodeInfo);

        }
        private void CacheSave<T>(string key, T value)
        {

            HttpRuntime.Cache.Insert(key, value, null, DateTime.Now.AddHours(3), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.NotRemovable, null);

        }
        #endregion ZipCodeOperation

        #region CotacaoDolarOperation
        [Route("cotacao")]
        [HttpPost()]
        public JsonResult CotacaoDolar()
        {
            DolarInfo dolarInfo = null;
            using (Dolar dolar = new Dolar())
            {
                dolarInfo = dolar.DolarInfo();
            }
            RatesInfo data = dolarInfo.RatesInfo.GetRatesInfo(RatesInfoType.USDBRL);
            
            return Json(data, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region AddressOperation
        [HttpPost]
        public JsonResult Address(string uf, string city, string address)
        {
            try
            {

                ZipCodeUF zipcodeUf = (ZipCodeUF)Enum.Parse(typeof(ZipCodeUF), uf, true);

                ZipCodeInfo[] data = ZipCodeLoad.Address(zipcodeUf, city, address);

                return Json(data, JsonRequestBehavior.DenyGet);

            }
            catch
            {
                return Json(new string[] { }, JsonRequestBehavior.DenyGet);
            }            

        }
        #endregion AddressOperation
    }
}