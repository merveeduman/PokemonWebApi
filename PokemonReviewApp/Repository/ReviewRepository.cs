using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReviewRepository(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CreateReview(Review review)
        {
            review.CreatedUserId = GetUserId();
            review.CreatedUserDateTime = DateTime.Now;

            _context.Add(review);
            return Save();
        }

        /* public bool DeleteReview(Review review)
         {
             _context.Remove(review);
             return Save();
         }
        */
        /* public bool DeleteReviews(List<Review> reviews)
         {
             _context.RemoveRange(reviews);
             return Save();
         }
        */
        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        /*  public ICollection<Review> GetReviews()
          {
              return _context.Reviews.ToList();
          }
        */
        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return _context.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }
        public bool SoftDeleteReview(int id)
        {
            var review = GetReview(id);
            if (review == null) return false;

            review.IsDeleted = true;
            _context.Update(review);

            return Save();
        }
    }
}