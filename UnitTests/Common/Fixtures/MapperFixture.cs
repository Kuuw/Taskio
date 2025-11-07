using AutoMapper;
using Services.Concrete;

namespace UnitTests.Common.Fixtures;

public class MapperFixture
{
    public IMapper Mapper { get; }

    public MapperFixture()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MapperConfig>();
        });
        Mapper = config.CreateMapper();
    }
}
