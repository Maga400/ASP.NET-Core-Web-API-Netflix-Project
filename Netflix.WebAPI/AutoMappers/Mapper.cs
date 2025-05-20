using AutoMapper;
using Netflix.Entities.Models;
using Netflix.WebAPI.Dtos;

namespace Netflix.WebAPI.AutoMappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CustomIdentityUser, UserDto>().ReverseMap();
        }
    }
}
