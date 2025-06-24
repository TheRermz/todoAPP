using AutoMapper;
using todoApp.Dtos.Auth;
using todoApp.Models;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, LoginResponseDto>();
    }
}
