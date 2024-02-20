using BackendTask.DbModels;
using BackendTask.Models;
using BackendTask.Repository;
using BackendTask.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestSharp;
using System.Threading.Tasks.Dataflow;

namespace BackendTask.Controllers
{

    public class LocationController : Controller
    {
        private readonly IRepository _results;
        private readonly IHubContext<SearchHub> _hubContext;
        private readonly SearchService _searchService;
        private static ActionBlock<NearbyPlacesRequest> _requestProcessor;


        public LocationController(SearchService searchService, IRepository results, IHubContext<SearchHub> hubContext)
        {
            _results = results;
            _hubContext = hubContext;   
            _searchService = searchService;
        }

        //regular
        //[HttpGet("GetNearbyPlaces")]
        //public async Task<ActionResult> FindNearbyPlaces(NearbyPlacesRequest requestParams)
        //{
        //    try
        //    {
        //        var responseContent = await _searchService.GetNearbyPlacesAsync(requestParams);

        //        await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"New data is retrieved and saved:" +
        //            $"\n Fields: {requestParams.Fields} \n Searched category: {requestParams.Query} \n lat/lang: {requestParams.Ll}");

        //        return Ok(responseContent);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}


        //semafor
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(5);

        [HttpGet("GetNearbyPlaces")]
        public async Task<ActionResult> FindNearbyPlaces(NearbyPlacesRequest requestParams)
        {
            await _semaphore.WaitAsync();
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
            finally
            {
                _semaphore.Release();
            }
        }

        //action blok

        //[HttpGet("GetNearbyPlaces")]
        //public async Task<ActionResult> FindNearbyPlaces(NearbyPlacesRequest requestParams)
        //{
        //    var actionBlock = new ActionBlock<NearbyPlacesRequest>(async request =>
        //    {
        //        try
        //        {
        //            var responseContent = await _searchService.GetNearbyPlacesAsync(request);

        //            await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"New data is retrieved and saved:" +
        //                $"\n Fields: {request.Fields} \n Searched category: {request.Query} \n lat/lang: {request.Ll}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }, new ExecutionDataflowBlockOptions
        //    {
        //        MaxDegreeOfParallelism = 5 
        //    });

        //    actionBlock.Post(requestParams);
        //    actionBlock.Complete();
        //    await actionBlock.Completion;

        //    return Ok(); 
        //}


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
