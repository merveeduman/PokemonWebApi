using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CountryDto, Country>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<PokemonDto, Pokemon>();
            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewerDto, Reviewer>();
            CreateMap<Country, CountryDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();

            // Food ile ilgili mappingler
            CreateMap<Food, FoodDto>();
            CreateMap<FoodDto, Food>();
            CreateMap<FoodType, FoodTypeDto>();
            CreateMap<FoodTypeDto, FoodType>();
            CreateMap<PokemonFoodDto, PokemonFood>();
            CreateMap<PokemonFood, PokemonFoodDto>()
          .ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => src.Food.Name))
          .ForMember(dest => dest.PokemonName, opt => opt.MapFrom(src => src.Pokemon.Name));

            CreateMap<PokemonFoodDtoPost, PokemonFood>();
            CreateMap<FoodTypeDto, FoodType>()  
    .ForMember(dest => dest.Id, opt => opt.Ignore());


            //user için ve role
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<UserRole, UserRoleDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<UserRole, UserRoleDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));


            //role ve permission için 
            CreateMap<Permission, PermissionDto>();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<RolePermissionCreateDto, RolePermission>();
            CreateMap<RolePermissionCreateDto, RolePermission>().ReverseMap();
            CreateMap<RolePermission, RolePermissionDto>();
            CreateMap<RolePermission, RolePermissionDto>().ReverseMap();


        }
    }
}