namespace BackendTask.DbModels
{
    public class SearchResult
    {
        public int Id { get; set; }
        public int SearchRequestId { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; } 
        public string? CensusBlock { get; set; } 
        public string? Country { get; set; } 
        public string? CrossStreet { get; set; } 
        public string? Dma { get; set; } 
        public string? FormattedAddress { get; set; } 
        public string? Locality { get; set; } 
        public string? Postcode { get; set; } 
        public string? Region { get; set; } 
        public string? AddressExtended { get; set; } 

        public virtual SearchRequest SearchRequest { get; set; }
    }
}
