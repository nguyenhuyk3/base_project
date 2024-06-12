using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Newtonsoft.Json.Linq;
using WebProject.Models.Address;

namespace Motel.Utility.Address
{
    public class GetPlacesUsing_vnappmob
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseConstructor _databaseConstructor;

        public GetPlacesUsing_vnappmob(IOptions<DatabaseSettings> databaseSettings)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("https://vapi.vnappmob.com/api/province/");
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public async Task<List<District>> GetDistricts(string provinceId)
        {
            try
            {
                var cityId = await _databaseConstructor.CityCollection
                                                           .Find(city => city.ProvinceId == provinceId)
                                                           .Project(city => city.Id)
                                                           .FirstOrDefaultAsync();
                var existingDistrict = await _databaseConstructor.DistrictCollection
                                                                      .Find(district => district.CityId == cityId)
                                                                      .AnyAsync();

                if (existingDistrict)
                {
                    var districts = await _databaseConstructor.DistrictCollection
                                                                  .Find(district => district.CityId == cityId)
                                                                  .ToListAsync();

                    return districts;
                }
                else
                {
                    HttpResponseMessage response = await _httpClient.GetAsync($"district/{provinceId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var obj = JObject.Parse(jsonString);
                        var results = obj["results"];

                        foreach (var result in results)
                        {
                            var districtId = result["district_id"].ToString();
                            var districtName = result["district_name"].ToString();

                            District district = new District()
                            {
                                DistrictId = districtId,
                                CityId = cityId,
                                Name = districtName
                            };

                            await _databaseConstructor.DistrictCollection
                                                        .InsertOneAsync(district);
                        }

                        var districts = await _databaseConstructor.DistrictCollection
                                                                    .Find(f => f.CityId == cityId)
                                                                    .ToListAsync();

                        return districts;
                    }
                    else
                    {

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Ward>> GetWards(string districtName)
        {
            var districtId = await _databaseConstructor.DistrictCollection
                                                        .Find(f => f.Name == districtName)
                                                        .Project(f => f.DistrictId)
                                                        .FirstOrDefaultAsync();
            var existingWard = await _databaseConstructor.WardCollection
                                                               .Find(ward => ward.DistrictId == districtId)
                                                               .AnyAsync();

            if (existingWard)
            {
                var wards = await _databaseConstructor.WardCollection
                                                          .Find(ward => ward.DistrictId == districtId)
                                                          .ToListAsync();

                return wards;
            }
            else
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"ward/{districtId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(jsonString);
                    var results = obj["results"];

                    foreach (var result in results)
                    {
                        var wardName = result["ward_name"].ToString();

                        Ward ward = new Ward()
                        {
                            DistrictId = districtId,
                            Name = wardName
                        };

                        await _databaseConstructor.WardCollection
                                                           .InsertOneAsync(ward);
                    }

                    var wards = await _databaseConstructor.WardCollection
                                                            .Find(ward => ward.DistrictId == districtId)
                                                            .ToListAsync();

                    return wards;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
