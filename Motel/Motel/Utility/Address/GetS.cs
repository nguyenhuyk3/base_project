using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.Utility.Database;
using Newtonsoft.Json.Linq;
using System.Linq;
using WebProject.Models.Address;

namespace Motel.Utility.Address
{
    public class GetS
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseConstructor _databaseConstructor;

        public GetS(IOptions<DatabaseSettings> databaseSettings)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("http://overpass-api.de/api/");
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public async Task<string> GetStreetsFromApi(string cityName, string districtName, string wardName)
        {
            var query = $@"
                            [out:json];
                            area[name=""{cityName}""]->.city;
                            area[name=""{districtName}""]->.district;
                            area[name=""{wardName}""]->.ward;
                            (
                                way(area.ward)[highway];
                            );
                            out body;
                            ";
            var requestContent = new StringContent($"data={Uri.EscapeDataString(query)}");

            // Thiết lập headers
            requestContent.Headers.ContentType = new System
                                                        .Net
                                                        .Http
                                                        .Headers
                                                        .MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // Gửi yêu cầu POST
            var response = await _httpClient.PostAsync("interpreter", requestContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content?.ReadAsStringAsync();

            return responseContent;
        }

        public async Task<List<Street>> GetStreets(string cityName, string districtName, string wardName)
        {
            try
            {
                var wardId = await _databaseConstructor.WardCollection
                                                        .Find(f => f.Name == wardName)
                                                        .Project(f => f.Id)
                                                        .FirstOrDefaultAsync();
                var existingStreet = await _databaseConstructor.StreetCollection
                                                                .Find(f => f.AwardId == wardId)
                                                                .AnyAsync();

                if (existingStreet)
                {
                    var streets = await _databaseConstructor.StreetCollection
                                                                .Find(f => f.AwardId == wardId)
                                                                .ToListAsync();

                    return streets;
                }
                else
                {
                    var content = await GetStreetsFromApi(cityName, districtName, wardName);
                    var json = JToken.Parse(content);
                    var streetSet = new HashSet<string>();

                    foreach (var element in json["elements"])
                    {
                        if (element["tags"]?["name"] != null && !streetSet.Contains(element["tags"]?["name"].ToString()))
                        {
                            streetSet.Add(element["tags"]?["name"].ToString());

                            Street street = new Street()
                            {
                                AwardId = wardId,
                                Name = element["tags"]?["name"].ToString(),
                            };

                            await _databaseConstructor.StreetCollection
                                                        .InsertOneAsync(street);
                        }
                    }

                    var streets = await _databaseConstructor.StreetCollection
                                                                .Find(f => f.AwardId == wardId)
                                                                .ToListAsync();

                    return streets;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
