using Alfa.ChatMS.Models;

namespace Alfa.ChatMS.Business
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User Create(User user, string password);
        void Delete(int id);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Update(User userParam, string password = null);
    }
}