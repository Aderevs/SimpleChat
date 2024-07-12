using AutoMapper;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DTOs;

namespace SimpleChat
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Chat, ChatDTO>();
            CreateMap<Message, MessageDTO>();

            CreateMap<UserDTO, User>();
            CreateMap<ChatDTO, Chat>()
                .ForMember(dest => dest.HostUserId, opt => opt.MapFrom(src => src.HostUser.UserId));
            CreateMap<MessageDTO, Message>();
        }
    }
}
