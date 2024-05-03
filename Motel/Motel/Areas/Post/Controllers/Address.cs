using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Motel.Utility.Address;
using Motel.Utility.Database;

namespace Motel.Areas.Post.Controllers
{
    public class Address : Controller
    {
        private readonly AddressApi _addressApi;

        public Address(IOptions<DatabaseSettings> databaseSettings)
        {
            _addressApi = new AddressApi(databaseSettings);
        }

        [HttpGet]
        public async Task<JsonResult> GetDistricts(string apiId)
        {
            if (!string.IsNullOrEmpty(apiId))
            {
                var districts = await _addressApi.GetDistrictsOfCity(apiId);

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
        public async Task<ActionResult> GetAwards(string apiId, string district)
        {
            if (!string.IsNullOrEmpty(apiId))
            {
                var awards = await _addressApi.GetAwards(apiId, district);

                List<string> stringOfWards = new List<string>();

                stringOfWards.Add(" ");

                foreach (var award in awards)
                {
                    stringOfWards.Add(award.Name);
                }

                stringOfWards = stringOfWards.Distinct().ToList();

                return Json(stringOfWards);
            }

            return null;
        }
    }
}
