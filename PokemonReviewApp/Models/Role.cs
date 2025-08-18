namespace PokemonReviewApp.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
