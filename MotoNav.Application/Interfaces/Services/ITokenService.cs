using MotoNav.Domain.Entities.Users;

namespace MotoNav.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(User user);
}