using AutoMapper;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public static class MapperExtensions
{
    public static void AddUserMappings(this IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<User, UserGetDto>();
        cfg.CreateMap<UserPutDto, User>();
    }

    public static void AddCategoryMappings(this IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Category, CategoryGetDto>();
        cfg.CreateMap<CategoryPostDto, Category>();
        cfg.CreateMap<CategoryPutDto, Category>();
    }

    public static void AddProjectMappings(this IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Project, ProjectGetDto>();
        cfg.CreateMap<ProjectPostDto, Project>();
        cfg.CreateMap<ProjectPutDto, Project>();
        cfg.CreateMap<ProjectUser, ProjectUserGetDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
    }

    public static void AddTaskMappings(this IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Entities.Models.Task, TaskGetDto>();
        cfg.CreateMap<TaskPostDto, Entities.Models.Task>();
        cfg.CreateMap<TaskPutDto, Entities.Models.Task>();
    }
}