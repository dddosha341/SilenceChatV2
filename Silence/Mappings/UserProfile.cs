using AutoMapper;
using TaskManagement.Infrastructure.Entities;
using TaskManagement.Infrastructure.ViewModels;

namespace TaskManagement.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName));

            CreateMap<UserViewModel, User>();
        }
    }
}
