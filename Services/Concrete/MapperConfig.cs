using AutoMapper;

namespace Services.Concrete;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Entities.Models.User, Entities.DTO.UserGetDto>();
        CreateMap<Entities.DTO.UserPutDto, Entities.Models.User>();

        CreateMap<Entities.Models.Category, Entities.DTO.CategoryGetDto>();
        CreateMap<Entities.DTO.CategoryPostDto, Entities.Models.Category>();
        CreateMap<Entities.DTO.CategoryPutDto, Entities.Models.Category>();

        CreateMap<Entities.Models.Project, Entities.DTO.ProjectGetDto>();
        CreateMap<Entities.DTO.ProjectPostDto, Entities.Models.Project>();
        CreateMap<Entities.DTO.ProjectPutDto, Entities.Models.Project>();
        CreateMap<Entities.Models.ProjectUser, Entities.DTO.ProjectUserGetDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<Entities.Models.Task, Entities.DTO.TaskGetDto>();
        CreateMap<Entities.DTO.TaskPostDto, Entities.Models.Task>();
        CreateMap<Entities.DTO.TaskPutDto, Entities.Models.Task>();
    }
}
