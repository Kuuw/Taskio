using Services.Concrete;
using Xunit;

namespace UnitTests.Services.Concrete;

public class BcryptServiceTests
{
    private readonly BcryptService _service = new();

    [Fact]
    public void HashPassword_ReturnsDifferentHash_ForSameInput()
    {
        var password = "Test123!";
        var hash1 = _service.HashPassword(password);
        var hash2 = _service.HashPassword(password);

        Assert.NotEqual(hash1, hash2);
        Assert.True(_service.VerifyPassword(password, hash1));
        Assert.True(_service.VerifyPassword(password, hash2));
    }

    [Fact]
    public void VerifyPassword_Fails_ForWrongPassword()
    {
        var password = "Secret123!";
        var hash = _service.HashPassword(password);

        Assert.False(_service.VerifyPassword("OtherPassword", hash));
    }

    [Fact]
    public void VerifyPassword_Succeeds_ForCorrectPassword()
    {
        var password = "MySecurePassword!";
        var hash = _service.HashPassword(password);
        Assert.True(_service.VerifyPassword(password, hash));
    }
}
