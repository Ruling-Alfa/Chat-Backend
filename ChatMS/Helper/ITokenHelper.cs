using Alfa.ChatMS.Models;

namespace Alfa.ChatMS.Helper
{
    public interface ITokenHelper
    {
        string GetToken(User User);
    }
}