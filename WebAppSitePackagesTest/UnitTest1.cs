using Microsoft.VisualStudio.TestTools.UnitTesting;
using Canducci.Forecast;
using Canducci.Zip;
using Canducci.QuoteDolar;
namespace WebAppSitePackagesTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ZipCodeNotError()
        {
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo zip = zipLoad.Find("19200000");
            Assert.AreEqual(true, !zip.Erro);
        }

        [TestMethod]
        public void ZipCodeError()
        {
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo zip = zipLoad.Find("19200000");
            Assert.AreEqual(false, zip.Erro);
        }

        [TestMethod]
        public void ForecastNotError()
        {
            CityForecast fs = new CityForecast();
            Cities cities = fs.Cities("Pirapozinho");
            Assert.AreEqual(1, cities.Count);            
        }

        [TestMethod]
        public void ForecastError()
        {
            CityForecast fs = new CityForecast();
            Cities cities = fs.Cities("Presidente");
            Assert.AreNotEqual(1, cities.Count);
        }


        [TestMethod]
        public void QuoteDolarNotError()
        {
            Dolar d = new Dolar();
            DolarInfo info = d.DolarInfo();
            Assert.IsInstanceOfType(info, typeof(DolarInfo));
        }

        [TestMethod]
        public void QuoteDolarError()
        {            
            Assert.IsNotInstanceOfType("", typeof(DolarInfo));
        }
    }
}
