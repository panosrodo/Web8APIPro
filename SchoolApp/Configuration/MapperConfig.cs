using AutoMapper;
using SchoolApp.Data;
using SchoolApp.DTO;

namespace SchoolApp.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>().ReverseMap();
        }
    }
}
