namespace Motel.Utility.Database
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CitiesCollectionName { get; set; } = null!;
        public string DistrictsCollectionName { get; set; } = null!;
        public string AwardsCollectionName { get; set; } = null!;
        public string StreetsCollectionName { get; set; } = null!;
        public string RolesCollectionName { get; set; } = null!;
        public string CategoriesCollectionName { get; set; } = null!;
        public string VipsCollectionName { get; set; } = null!;
        public string UserAccountsCollectionName { get; set; } = null!;
        public string PostsCollectionName { get; set; } = null!;
        public string BillsCollectionName { get; set; } = null!;

    }
}
