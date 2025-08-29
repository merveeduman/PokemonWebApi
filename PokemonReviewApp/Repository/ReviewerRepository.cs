using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReviewerRepository(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            reviewer.CreatedUserId = GetUserId();
            reviewer.CreatedUserDateTime = DateTime.Now;

            _context.Add(reviewer);
            return Save();
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        /*   public bool DeleteReviewer(Reviewer reviewer)
           {
               _context.Remove(reviewer);
               return Save();
           }
        */
        public Reviewer GetReviewer(int reviewerId)
        {
            return _context.Reviewers.Where(r => r.Id == reviewerId).Include(e => e.Reviews).FirstOrDefault();
        }

        /*   public ICollection<Reviewer> GetReviewers()
           {
               return _context.Reviewers.ToList();
           }
        */
        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }
        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)  // userId: güncelleyen kullanıcı
        {
            var existing = _context.Reviewers.FirstOrDefault(r => r.Id == reviewer.Id);
            if (existing == null) return false;

            // Sadece güncellenmesini istediğin alanları kopyala
            existing.FirstName = reviewer.FirstName;
            existing.LastName = reviewer.LastName;

            // Oluşturan bilgileri koru, değiştirme
            existing.CreatedUserId = existing.CreatedUserId;
            existing.CreatedUserDateTime = existing.CreatedUserDateTime;

           
            _context.Update(existing);
            return Save();
        }


        public bool SoftDeleteReviewer(int id)
        {
            var reviewer = GetReviewer(id);
            if (reviewer == null) return false;

            reviewer.IsDeleted = true;
            _context.Update(reviewer);

            return Save();
        }
    }
}