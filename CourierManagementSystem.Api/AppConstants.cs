namespace CourierManagementSystem.Api.Constants
{
    public static class AppConstants
    {
        public const int latitude_precision = 10;
        public const int Latitude_scale = 8;
        public const int longitude_precision = 11;
        public const int longitude_scale = 8;
        public const int weight_precision = 8;
        public const int weight_scale = 2;
        public const int length_precision = 6;
        public const int length_scale = 2;
        public const int width_precision = 6;
        public const int width_scale = 2;
        public const int height_precision = 6;
        public const int height_scale = 2;
        public const int maxweight_precision = 8;
        public const int maxweight_scale = 2;
        public const int maxvolume_precision = 8;
        public const int maxvolume_scale = 3;
        public const double earth_radius = 6371;
        public const decimal speedKmPerHour = 60m;
        // Assuming average speed of 40 km/h in urban areas
        public const decimal averageSpeedKmh = 40m;
        public const int maxRoutesPerDay = 9;
    }
}