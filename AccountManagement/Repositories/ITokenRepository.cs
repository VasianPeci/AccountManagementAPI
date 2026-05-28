using Microsoft.AspNetCore.Identity;

namespace AccountManagement.Repositories
{
    public interface ITokenRepository
    {
        public string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
