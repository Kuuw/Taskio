using AutoMapper;
using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Concrete;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly IBcryptService _bcryptService;

    public AuthService(IConfiguration config, IUserRepository repository, IBcryptService bcrypt)
    {
        _config = config;
        _userRepository = repository;
        _bcryptService = bcrypt;
    }

    private string Generate(User user)
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY") ??
        throw new ApplicationException("JWT key is not configured.");
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        var token = new JwtSecurityToken(
          _config["JwtSettings:Issuer"],
          _config["JwtSettings:Audience"],
          claims,
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ServiceResult<AuthenticateResponse?>> AuthenticateAsync(UserLogin userLogin)
    {
        var user = await _userRepository.GetByEmailAsync(userLogin.Email);
        if (user == null) 
        { 
            return ServiceResult<AuthenticateResponse?>.BadRequest("Email or password is invalid."); 
        }

        if (_bcryptService.VerifyPassword(userLogin.Password, user.Password))
        {
            var token = Generate(user);
            var response = new AuthenticateResponse(user, token);

            return ServiceResult<AuthenticateResponse?>.Ok(response);
        }
        return ServiceResult<AuthenticateResponse?>.BadRequest("Email or password is invalid.");
    }
}
