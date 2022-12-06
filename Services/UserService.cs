using FinalTercero.Interfaces;
using FinalTercero.Models;

namespace FinalTercero.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Usuario> GetByCredentials(string username, string password)
        {
            return await _userRepository.GetUserByCredentials(username, password);
        }
    }
}
