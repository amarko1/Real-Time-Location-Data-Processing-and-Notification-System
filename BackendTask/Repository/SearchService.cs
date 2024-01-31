using BackendTask.DbModels;
using BackendTask.Models;
using RestSharp;

namespace BackendTask.Repository
{
    public class SearchService
    {
        private readonly IRepository _repository;
        private readonly RestClient _restClient;

        public SearchService(IRepository repository)
        {
            _repository = repository;
            _restClient = new RestClient("https://api.foursquare.com/v3/places/nearby");
        }

        public async Task<FoursquareResponse> GetNearbyPlacesAsync(NearbyPlacesRequest requestParams)
        {
            var request = new RestRequest();
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "fsq3HJpxZgfF0ds3phBT+LJ0H0m3QioW8Qkp7ETaSiGDGE0=");

            if (!string.IsNullOrWhiteSpace(requestParams.Fields)) request.AddParameter("fields", requestParams.Fields);
            if (!string.IsNullOrWhiteSpace(requestParams.Ll)) request.AddParameter("ll", requestParams.Ll);
            if (requestParams.Hacc.HasValue) request.AddParameter("hacc", requestParams.Hacc.Value);
            if (requestParams.Altitude.HasValue) request.AddParameter("altitude", requestParams.Altitude.Value);
            if (!string.IsNullOrWhiteSpace(requestParams.Query)) request.AddParameter("query", requestParams.Query);
            if (requestParams.Limit.HasValue) request.AddParameter("limit", requestParams.Limit.Value);

            var response = await _restClient.GetAsync(request);
            var nearbyPlacesResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FoursquareResponse>(response.Content);

            var searchRequest = new SearchRequest
            {
                Query = requestParams.Query ?? string.Empty,
                Timestamp = DateTime.Now
            };
            _repository.AddSearchRequest(searchRequest);
            await _repository.SaveChangesAsync();

            foreach (var result in nearbyPlacesResponse.Results)
            {
                var searchResult = new SearchResult
                {
                    SearchRequestId = searchRequest.Id,
                    Name = result.Name, 
                    Address = result.Location.Address, 
                    CensusBlock = result.Location.CensusBlock, 
                    Country = result.Location.Country, 
                    CrossStreet = result.Location.CrossStreet, 
                    Dma = result.Location.Dma, 
                    FormattedAddress = result.Location.FormattedAddress, 
                    Locality = result.Location.Locality, 
                    Postcode = result.Location.Postcode, 
                    Region = result.Location.Region, 
                    AddressExtended = result.Location.AddressExtended 
                };
                _repository.AddSearchResult(searchResult);
            }

            await _repository.SaveChangesAsync();

            return nearbyPlacesResponse;
        }
    }
}
