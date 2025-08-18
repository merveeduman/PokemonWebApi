namespace PokemonReviewApp.Dto
{
    public class UserRoleDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }  

        public int RoleId { get; set; }
        public bool IsDeleted { get; set; }

        public string RoleName { get; set; }  


    }

}
