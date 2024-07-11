using AutoMapper;
using SimpleChat.DbLogic;
using SimpleChat.DTOs;

namespace SimpleChat
{
    public class ModelProfile: Profile
    {
        public ModelProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Chat, ChatDTO>();
            CreateMap<Message, MessageDTO>();

            CreateMap<UserDTO, User>();
            CreateMap<ChatDTO, Chat>();
            CreateMap<MessageDTO, Message>();
        }
    }
}
