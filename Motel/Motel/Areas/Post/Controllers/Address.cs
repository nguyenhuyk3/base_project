using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Address;
using Motel.Utility.Database;

namespace Motel.Areas.Post.Controllers
{
    public class Address : Controller
    {
        private readonly GetDAW _getDAW;
        private readonly GetS _getS;
        private readonly DatabaseConstructor _databaseConstructor;

        public Address(IOptions<DatabaseSettings> databaseSettings)
        {
            _getDAW = new GetDAW(databaseSettings);
            _getS = new GetS(databaseSettings);
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        [HttpGet]
        public async Task<JsonResult> GetDistricts(string apiId)
        {
            if (!string.IsNullOrEmpty(apiId))
            {
                var districts = await _getDAW.GetDistricts(apiId);

                List<string> stringOfDistricts = new List<string>();

                // Add empty string first beacause it will not show the district name first
                stringOfDistricts.Add(" ");

                foreach (var district in districts)
                {
                    stringOfDistricts.Add(district.Name);
                }

                return Json(stringOfDistricts);
            }

            return null;
        }

        // Get awards from db by using apiIdCity and districtName
        [HttpGet]
        public async Task<JsonResult> GetAwards(string apiId, string district)
        {
            if (!string.IsNullOrEmpty(apiId))
            {
                var awards = await _getDAW.GetWards(apiId, district);

                List<string> stringOfWards = new List<string>();

                stringOfWards.Add(" ");

                foreach (var award in awards)
                {
                    stringOfWards.Add(award.Name);
                }

                return Json(stringOfWards);
            }

            return null;
        }

        [HttpGet]
        public async Task<ActionResult> GetStreets(string apiId, string district, string ward)
        {
            var cityName = await _databaseConstructor.CityCollection
                                                        .Find(f => f.ApiId == int.Parse(apiId))
                                                        .Project(f => f.Name)
                                                        .FirstOrDefaultAsync();
            var streets = await _getS.GetStreets(cityName, district, ward);
            var stringOfStreets = new List<string>();

            stringOfStreets.Add(" ");

            foreach (var street in streets)
            {
                stringOfStreets.Add(street.Name);
            }

            return Json(stringOfStreets);
        }
    }
}
