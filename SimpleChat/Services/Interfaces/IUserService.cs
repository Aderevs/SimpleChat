using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public interface IUserService
    {
        Task<UserDTO> CreateUser(UserDTO userDTO);
        Task DeleteUser(int userId);
    }
}