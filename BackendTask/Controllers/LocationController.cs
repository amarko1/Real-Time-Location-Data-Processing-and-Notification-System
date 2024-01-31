using BackendTask.DbModels;
using BackendTask.Models;
using BackendTask.Repository;
using BackendTask.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestSharp;

namespace BackendTask.Controllers
{
    public class LocationController : Controller
    {
        private readonly IRepository _results;
        private readonly IHubContext<SearchHub> _hubContext;
        private readonly SearchService _searchService;

        public LocationController(SearchService searchService, IRepository results, IHubContext<SearchHub> hubContext)
        {
            _results = results;
            _hubContext = hubContext;   
            _searchService = searchService; 
        }

        [HttpGet("GetNearbyPlaces")]
        public async Task<ActionResult> FindNearbyPlaces(NearbyPlacesRequest requestParams)
        {
            try
            {
                var responseContent = await _searchService.GetNearbyPlacesAsync(requestParams);

                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"New data is retrieved and saved:" +
                    $"\n Fields: {requestParams.Fields} \n Searched category: {requestParams.Query} \n lat/lang: {requestParams.Ll}");

                return Ok(responseContent); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<List<SearchResult>>> FilterByName(string name)
        {
            try
            {
                var searchResults = _results.GetAll()
                    .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"New search: {name}");

                return searchResults;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<SearchResult>>> GetAllDataFromDb()
        {
            try
            {
                var searchResults = _results.GetAll().ToList();

                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"All data from database is retrieved!");

                return searchResults;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
