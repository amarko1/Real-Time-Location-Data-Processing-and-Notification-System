namespace BackendTask.Models
{
    public class NearbyPlacesRequest
    {
        public string Fields { get; set; }
        public string Ll { get; set; }
        public double? Hacc { get; set; }
        public double? Altitude { get; set; }
        public string Query { get; set; }
        public int? Limit { get; set; }
    }

    public class Location
    {
        public string Address { get; set; }
        public string CensusBlock { get; set; }
        public string Country { get; set; }
        public string CrossStreet { get; set; }
        public string Dma { get; set; }
        public string FormattedAddress { get; set; }
        public string Locality { get; set; }
        public string Postcode { get; set; }
        public string Region { get; set; }
        public string AddressExtended { get; set; } 
    }

    public class Result
    {
        public Location Location { get; set; }
        public string Name { get; set; }
    }

    public class FoursquareResponse
    {
        public List<Result> Results { get; set; }
    }

}
