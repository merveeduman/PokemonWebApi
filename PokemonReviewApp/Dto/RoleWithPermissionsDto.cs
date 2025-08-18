namespace PokemonReviewApp.Dto
{
    public class RoleWithPermissionsDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}

