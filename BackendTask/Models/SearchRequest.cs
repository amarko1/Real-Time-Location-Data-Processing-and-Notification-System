namespace BackendTask.DbModels
{
    public class SearchRequest
    {
        public int Id { get; set; }
        public string? Query { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ICollection<SearchResult> SearchResults { get; set; }
    }
}
