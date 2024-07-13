using AutoMapper;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public UserService(IMapper mapper, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;
        }
        public async Task<UserDTO> CreateUser(UserDTO userDTO)
        {
            if (await _usersRepository.CheckIfUserWithSuchIdExistsAsync(userDTO.UserId))
            {
                throw new ArgumentException("User with such ID already exists");
            }
            var userDb = _mapper.Map<User>(userDTO);
            var createdUser = await _usersRepository.AddAsync(userDb);
            return _mapper.Map<UserDTO>(createdUser);
        }
        public async Task DeleteUser(int userId)
        {
            var userDb = await _usersRepository.GetByIdOrDefaultAsync(userId);
            if(userDb == null)
            {
                throw new ArgumentException("User with such ID was not found");
            }
            await _usersRepository.DeleteAsync(userDb);
        }
    }
}
