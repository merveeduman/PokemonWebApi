namespace PokemonReviewApp.Models
{
    public class RolePermission : BaseEntity
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }

        public bool IsDeleted { get; set; }
    }
}
