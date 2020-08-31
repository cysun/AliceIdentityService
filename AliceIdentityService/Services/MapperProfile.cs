using AliceIdentityService.Models;
using AutoMapper;

namespace SciCAFE.NET.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegistrationInputModel, User>();
            CreateMap<NewUserInputModel, User>();
            CreateMap<User, EditUserInputModel>();
            CreateMap<User, UserViewModel>();
            CreateMap<EditUserInputModel, User>();
        }
    }
}
