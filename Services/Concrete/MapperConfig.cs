using AutoMapper;
using Services.Abstract;

namespace Services.Concrete;

public class MapperConfig : IMapperConfig
{
    public static Mapper InitializeAutomapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddUserMappings();
            cfg.AddCategoryMappings();
            cfg.AddProjectMappings();
            cfg.AddTaskMappings();
        });

        var mapper = new Mapper(config);
        return mapper;
    }
}