using Newtonsoft.Json.Linq;
using WebProject.Models.Address;

namespace Motel.Utility.Address
{
    public class GetCoordinates
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GetCoordinates(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<(double Latitude, double Longitude)> EncodeCoordinates(string streetName, 
                                                                        string wardName, 
                                                                        string districtName, 
                                                                        string cityName)
        {
            var address = streetName + ", " + wardName + ", " + districtName + ", " + cityName;
            var requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error calling Google Geocoding API: {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonResponse);

            if (json["status"].ToString() != "OK")
            {
                throw new Exception($"Error returned from Google Geocoding API: {json["status"]}");
            }

            var location = json["results"][0]["geometry"]["location"];
            var lat = location["lat"].Value<double>();
            var lng = location["lng"].Value<double>();

            return (lat, lng);
        }
    }
}
