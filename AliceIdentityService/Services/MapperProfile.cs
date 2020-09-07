using AliceIdentityService.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace SciCAFE.NET.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegistrationInputModel, User>();
            CreateMap<NewUserInputModel, User>();
            CreateMap<User, EditUserInputModel>();
            CreateMap<EditUserInputModel, User>();
            CreateMap<User, UserViewModel>();
            CreateMap<ApiScopeInputModel, ApiScope>();
            CreateMap<ApiScope, ApiScopeInputModel>();
            CreateMap<IdentityResource, IdentityResourceInputModel>();
            CreateMap<IdentityResourceInputModel, IdentityResource>();
            CreateMap<IdentityResourceClaimInputModel, IdentityResourceClaim>();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimInputModel>();
        }
    }
}
