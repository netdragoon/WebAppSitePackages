using System;
using System.Web;
using System.Web.Mvc;
using Canducci.Zip;
using Canducci.QuoteDolar;
using Canducci.Forecast;
using Canducci.Forecast.Interfaces;
using Canducci.Gravatar;
using Canducci.Gravatar.Validation;
using System.Threading.Tasks;

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

        #region Forecast
        [HttpPost]
        [Route("cities")]
        public async Task<JsonResult> ForecastCities(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    ICities cities = null;

                    using (ICityForecast fc = new CityForecast())
                    {
                        cities = await fc.CitiesAsync(name.WithoutAccents());
                    }
                    return Json(cities, JsonRequestBehavior.DenyGet);
                }
            }
            catch
            {
                return Json(new string[] { }, JsonRequestBehavior.DenyGet);
            }

            return Json(new string[] { }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [Route("prevision")]
        public async Task<JsonResult> ForecastPrevision(int? Id, int? Count = 4)
        {
            try
            {
                IPrevision prev = null;
                if (Id.HasValue)
                {                    
                    using (ICityForecast fc = new CityForecast())
                    {
                        prev = await fc.ForecastAsync(Id.Value, ((Count.HasValue && Count == 7) ? ForecastDay.D7 : ForecastDay.D4));
                    }
                    return Json(prev, JsonRequestBehavior.DenyGet);
                }
            }
            catch
            {
                return Json(new string[] { }, JsonRequestBehavior.DenyGet);
            }

            return Json(new string[] { }, JsonRequestBehavior.DenyGet);
        }
        #endregion Forecast

        #region Gravatar
        [HttpPost]
        [Route("gravatar")]
        public JsonResult Gravatar(string email, int? width = 100)
        {            
            try               
            {            
                if (Assertion.IsEmail(email))
                {
                    IEmail Email = Canducci.Gravatar.Email.Parse(email);

                    IAvatarFolder folder = new AvatarFolder("image/", Server.MapPath("~"));

                    IAvatarConfiguration config =
                        new AvatarConfiguration(email, folder, width.Value);

                    using (IAvatar avatar = new Avatar(config))
                    {
                        if (!avatar.Exists())
                        {
                            avatar.Save();
                        }

                        var model = new { email = email, image = "/" + avatar.WebPath(), width = width };

                        return Json(GravatarJson(false, "Ok", model), JsonRequestBehavior.DenyGet);

                    }
                }
            }
            catch(Exception ex)
            {                
                return Json(GravatarJson(true, ex.Message), JsonRequestBehavior.DenyGet);
            }
            return Json(GravatarJson(false, "E-mail inválid"), JsonRequestBehavior.DenyGet);

        }
        protected dynamic GravatarJson(bool error, string message, object model = null)
        {
            dynamic _return = new { error = error, message = message };
            if (model != null)
            {
                _return = new { error = error, message = message, item = model };
            }
            return _return;
        }
        #endregion Gravatar

    }
}