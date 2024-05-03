using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using WebProject.Models.Address;

namespace Motel.Utility.Address
{
    public class AddressApi
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseConstructor _databaseConstructor;

        public AddressApi(IOptions<DatabaseSettings> databaseSettings)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("https://vnprovinces.pythonanywhere.com/api/provinces/");
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public async Task<List<District>> GetDistrictsOfCity(string apiId)
        {
            try
            {
                // Get the city's Id through the ApiId sent from the client
                // This Id is used to check if any district exists in the city
                var cityId = await _databaseConstructor.CityCollection
                                    .Find(city => city.ApiId == int.Parse(apiId))
                                    .Project(city => city.Id)
                                    .FirstOrDefaultAsync();

                // Check if there is a district then call data from database
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
                    HttpResponseMessage response = await _httpClient.GetAsync(apiId);

                    if (response.IsSuccessStatusCode)
                    {
                        // Returns json data as string
                        string jsonString = await response.Content.ReadAsStringAsync();

                        var obj = JObject.Parse(jsonString);
                        var districts = obj["districts"]
                                            .Select(d => d["full_name"]
                                            .ToString())
                                            .ToList();

                        foreach (var districtName in districts)
                        {
                            District district = new District()
                            {
                                Name = districtName,
                                CityId = cityId
                            };

                            await _databaseConstructor.DistrictCollection.InsertOneAsync(district);
                        }

                        var districtDocuments = await _databaseConstructor.DistrictCollection
                                                        .Find(district => district.CityId == cityId)
                                                        .ToListAsync();

                        return districtDocuments;
                    }
                    else
                    {

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Ward>> GetAwards(string id, string districtName)
        {
            try
            {
                var districtId = await _databaseConstructor.DistrictCollection
                                        .Find(district => district.Name == districtName)
                                        .Project(district => district.Id)
                                        .FirstOrDefaultAsync();

                var existingWard = await _databaseConstructor.WardCollection
                                    .Find(ward => ward.DistrictId == districtId)
                                    .AnyAsync();

                // Check if there is awards then call data from database
                if (existingWard)
                {
                    var wards = await _databaseConstructor.WardCollection
                                        .Find(ward => ward.DistrictId == districtId)
                                        .ToListAsync();

                    return wards;
                }
                else
                {
                    for (int i = 1; i < 50; i++)
                    {
                        HttpResponseMessage response = await _httpClient.GetAsync($"{id}/wards/?page={i}");

                        if (!response.IsSuccessStatusCode)
                        {
                            break;
                        }

                        string jsonString = await response.Content?.ReadAsStringAsync();
                        var obj = JObject.Parse(jsonString);

                        // results[] in response
                        JArray results = (JArray)obj["results"];

                        foreach (JObject result in results)
                        {
                            JObject district = (JObject)result["district"];

                            string districtFullName = district["full_name"].ToString();
                            string wardFullName = result["full_name"].ToString();

                            if (string.Compare(districtName, districtFullName,
                                StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                Ward ward = new Ward()
                                {
                                    Name = wardFullName,
                                    DistrictId = districtId
                                };

                                await _databaseConstructor.WardCollection
                                                            .InsertOneAsync(ward);
                            }
                        }
                    }

                    var wards = await _databaseConstructor.WardCollection
                                                    .Find(ward => ward.DistrictId == districtId)
                                                    .ToListAsync();

                    return wards;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
