namespace PokemonReviewApp.Dto
{
    public class BulkDeleteResultDto
    {
        public List<int> DeletedIds { get; set; } = new List<int>();
        public List<int> NotFoundIds { get; set; } = new List<int>();
    }
}
