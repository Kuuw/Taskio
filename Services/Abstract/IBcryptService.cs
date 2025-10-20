namespace Services.Abstract;

public interface IBcryptService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
