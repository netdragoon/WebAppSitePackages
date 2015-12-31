using System;
using System.Web;
using System.Web.Mvc;
using Canducci.Zip;
using Canducci.QuoteDolar;
using Canducci.Forecast;
using Canducci.Forecast.Interfaces;
using Canducci.Gravatar;
using Canducci.Gravatar.Validation;
using Canducci.ShortUrl;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Canducci.YoutubeThumbnail;
using System.Collections.Generic;

namespace WebAppSitePackages.Controllers
{
    [RoutePrefix("operation")]
    public class OperationController : Controller
    {
        #region ZipCodeOperation
        [Route("zipcode")]
        [HttpPost()]
        public async Task<JsonResult> zipcode(string cep)
        {

            ZipCodeInfo data = CacheGet(cep);
            
            if (data == null)
            {

                ZipCodeLoad zipCodeLoad = new ZipCodeLoad();

                data = await zipCodeLoad.FindAsync(cep);
                
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
        public async Task<JsonResult> CotacaoDolar()
        {
            DolarInfo dolarInfo = null;
            using (Dolar dolar = new Dolar())
            {
                dolarInfo = await dolar.DolarInfoAsync();
            }

            RatesInfo data = dolarInfo.RatesInfo.GetRatesInfoUSDBRL();
            
            return Json(data, JsonRequestBehavior.DenyGet);

        }
        #endregion

        #region AddressOperation
        [HttpPost]
        public async Task<JsonResult> Address(string uf, string city, string address)
        {
            try
            {
                ZipCodeLoad zipCodeLoad = new ZipCodeLoad();

                ZipCodeUf zipcodeUf = (ZipCodeUf)Enum.Parse(typeof(ZipCodeUf), uf, true);

                ZipCodeInfo[] data = await zipCodeLoad.AddressAsync(zipcodeUf, city, address);

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

        #region ShortUrlOperation
        [HttpPost]
        [Route("shorturl")]
        public async Task<JsonResult> ShortUrl(string Url)
        {
            string servePath = Server.MapPath("~");
            string nameJson = string.Format("/json/{0}.json", renderHash(Url));
            string path = string.Format("{0}/{1}", servePath, nameJson);

            try
            {                
                string Content = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : null;
                if (Content == null)
                {

                    IsGd isGD = new IsGd(Url);

                    ShortUrlClient Send = new ShortUrlClient(isGD);

                    ShortUrlReceive Receive = await Send.ReceiveAsync();

                    System.IO.File.WriteAllText(path, Receive.ToJson());

                    return Json(new { data = Receive, error = false }, JsonRequestBehavior.DenyGet);

                }

                Dictionary<object, object> _data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<object, object>>(Content);
                dynamic _result = new { ShortUrl = _data["url"], LongUrl = _data["longurl"], Keyword = _data["keyword"] };
                return Json(new { data = _result, error = false }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = true, data = ' ', exception = ex.Message });
            }
        }
        private async Task<string> RenderHash(string Value)
        {
            return await Task.FromResult(renderHash(Value));
        }
        private string renderHash(string Value)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] valueToArray = md5.ComputeHash(Encoding.UTF8.GetBytes(Value));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < valueToArray.Length; i++)
                {
                    sBuilder.Append(valueToArray[i].ToString("x2"));
                }
                md5.Dispose();
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        #endregion

        #region ThumbnailOp
        [HttpPost]
        [Route("Thumbnail")]
        public async Task<JsonResult> Thumbnail(string Url)
        {            
            try
            {                

                Thumbnail thumb = new Thumbnail(Url);

                string code = thumb.ThumbnailPicture0.Code;
                if (!string.IsNullOrEmpty(code))
                {
                    ThumbnailResult rpic0 = await thumb.ThumbnailPicture0.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rpic1 = await thumb.ThumbnailPicture1.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rpic2 = await thumb.ThumbnailPicture2.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rpic3 = await thumb.ThumbnailPicture3.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rdef = await thumb.ThumbnailPictureDefault.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rhg = await thumb.ThumbnailPictureHightQuality.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rmx = await thumb.ThumbnailPictureMaxResolution.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rmq = await thumb.ThumbnailPictureMediumQuality.SaveAsAsync("thumb/", Server.MapPath("~"));
                    ThumbnailResult rst = await thumb.ThumbnailPictureStandard.SaveAsAsync("thumb/", Server.MapPath("~"));

                    string pic0 = thumb.ThumbnailPicture0.PathWeb;
                    string pic1 = thumb.ThumbnailPicture1.PathWeb;
                    string pic2 = thumb.ThumbnailPicture2.PathWeb;
                    string pic3 = thumb.ThumbnailPicture3.PathWeb;
                    string def = thumb.ThumbnailPictureDefault.PathWeb;
                    string hg = thumb.ThumbnailPictureHightQuality.PathWeb;
                    string mx = thumb.ThumbnailPictureMaxResolution.PathWeb;
                    string mq = thumb.ThumbnailPictureMediumQuality.PathWeb;
                    string st = thumb.ThumbnailPictureStandard.PathWeb;
                    string embed = thumb.VideoEmbed();
                    string share = thumb.VideoShare;

                    var datas = new
                    {
                        pic0 = new { picture = pic0, status = rpic0.Exception == null },
                        pic1 = new { picture = pic1, status = rpic1.Exception == null },
                        pic2 = new { picture = pic2, status = rpic2.Exception == null },
                        pic3 = new { picture = pic3, status = rpic3.Exception == null },
                        def = new { picture = def, status = rdef.Exception == null },
                        hg = new { picture = hg, status = rhg.Exception == null },
                        mx = new { picture = mx, status = rmx.Exception == null },
                        mq = new { picture = mq, status = rmq.Exception == null },
                        st = new { picture = st, status = rst.Exception == null },
                        embed = embed,
                        share = share,
                        code = code
                    };

                    return Json(new { data = datas, error = false });
                }
                else
                {
                    return Json(new { data = ' ', error = true });
                }             
            }
            catch (Exception ex)
            {
                return Json(new { data = ' ', error = true, exception = ex });
            }
        }
        #endregion
    }
}