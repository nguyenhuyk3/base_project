using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Models;
using WebProject.Models.Address;

namespace Motel.Utility.Database
{
    public class DatabaseConstructor
    {
        private readonly IMongoCollection<City> _cityCollection;
        private readonly IMongoCollection<District> _districtCollection;
        private readonly IMongoCollection<Ward> _wardCollection;
        private readonly IMongoCollection<Street> _streetCollection;
        private readonly IMongoCollection<Role> _roleCollection;
        private readonly IMongoCollection<Catergory> _categoryCollection;
        private readonly IMongoCollection<Vip> _vipCollection;
        private readonly IMongoCollection<UserAccount> _userAccountCollection;
        private readonly IMongoCollection<Post> _postCollection;
        private readonly IMongoCollection<Bill> _billCollection;

        public IMongoCollection<City> CityCollection => _cityCollection;
        public IMongoCollection<District> DistrictCollection => _districtCollection;
        public IMongoCollection<Ward> WardCollection => _wardCollection;
        public IMongoCollection<Street> StreetCollection => _streetCollection;
        public IMongoCollection<Role> RoleCollection => _roleCollection;
        public IMongoCollection<Catergory> CategoryCollection => _categoryCollection;
        public IMongoCollection<Vip> VipCollection => _vipCollection;
        public IMongoCollection<UserAccount> UserAccountCollection => _userAccountCollection;
        public IMongoCollection<Post> PostCollection => _postCollection;
        public IMongoCollection<Bill> BillCollection => _billCollection;

        public DatabaseConstructor(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _cityCollection = database.GetCollection<City>
                (databaseSettings.Value.CitiesCollectionName);
            _districtCollection = database.GetCollection<District>
               (databaseSettings.Value.DistrictsCollectionName);
            _wardCollection = database.GetCollection<Ward>
              (databaseSettings.Value.WardsCollectionName);
            _streetCollection = database.GetCollection<Street>
               (databaseSettings.Value.StreetsCollectionName);
            _roleCollection = database.GetCollection<Role>
              (databaseSettings.Value.RolesCollectionName);
            _categoryCollection = database.GetCollection<Catergory>
               (databaseSettings.Value.CategoriesCollectionName);
            _vipCollection = database.GetCollection<Vip>
              (databaseSettings.Value.VipsCollectionName);
            _userAccountCollection = database.GetCollection<UserAccount>
               (databaseSettings.Value.UserAccountsCollectionName);
            _postCollection = database.GetCollection<Post>
              (databaseSettings.Value.PostsCollectionName);
            _billCollection = database.GetCollection<Bill>
               (databaseSettings.Value.BillsCollectionName);
        }
    }
}
