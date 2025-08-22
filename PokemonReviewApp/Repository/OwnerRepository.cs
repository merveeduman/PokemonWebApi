using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OwnerRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CreateOwner(Owner owner)
        {
            owner.CreatedUserId = GetUserId();
            owner.CreatedUserDateTime = DateTime.Now;

            _context.Add(owner);
            return Save();
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        /* public bool DeleteOwner(Owner owner)
         {
             _context.Remove(owner);
             return Save();
         }
        */
        public Owner GetOwner(int ownerId)
        {
            return _context.Owners
                .Include(o => o.PokemonOwners)
                    .ThenInclude(po => po.Pokemon)
                .FirstOrDefault(o => o.Id == ownerId);
        }


        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return _context.PokemonOwners.Where(p => p.Pokemon.Id == pokeId).Select(o => o.Owner).ToList();
        }

        /*  public ICollection<Owner> GetOwners()
          {
              return _context.Owners.ToList();
          }
  */
        public ICollection<Owner> GetOwnersOfAPokemon(int pokemonId)
        {
            return _context.PokemonOwners
                .Where(po => po.PokemonId == pokemonId)
                .Include(po => po.Owner)
                    .ThenInclude(o => o.PokemonOwners)
                        .ThenInclude(po => po.Pokemon)
                .Select(po => po.Owner)
                .ToList();
        }


        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.Where(c => !c.IsDeleted).OrderBy(c => c.Id).ToList();
        }
        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(p => p.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }
        public bool SoftDeleteOwner(int id)
        {
            var owner = GetOwner(id);
            if (owner == null) return false;

            owner.IsDeleted = true;
            _context.Update(owner);

            return Save();
        }
    }
}