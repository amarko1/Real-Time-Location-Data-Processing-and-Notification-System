using BackendTask.DbModels;

namespace BackendTask.Repository
{

    public interface IRepository
    {
        IEnumerable<SearchResult> GetAll();
        void AddSearchRequest(SearchRequest searchRequest);
        void AddSearchResult(SearchResult searchResult);
        Task SaveChangesAsync();
    }

    public class Repository : IRepository
    {
        private readonly AppDbContext _dbContext;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SearchResult> GetAll()
        {
            return _dbContext.SearchResults;
        }

        public void AddSearchRequest(SearchRequest searchRequest)
        {
            _dbContext.SearchRequests.Add(searchRequest);
        }

        public void AddSearchResult(SearchResult searchResult)
        {
            _dbContext.SearchResults.Add(searchResult);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
