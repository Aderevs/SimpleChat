﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SimpleChat.DbLogic;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DTOs;
using System.Runtime.InteropServices;
using System.Transactions;

namespace SimpleChat.Services
{
    public class ChatService
    {
        private readonly IMapper _mapper;
        private readonly ChatsRepository _chatsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly MessagesRepository _messagesRepository;
        private readonly ChatDbContext _dbContext;

        public ChatService(
            IMapper mapper,
            ChatsRepository chatsRepository,
            UsersRepository usersRepository,
            MessagesRepository messagesRepository,
            ChatDbContext dbContext)
        {
            _mapper = mapper;
            _chatsRepository = chatsRepository;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Chat>> GetAllChats()
        {
            var chatsDb = await _chatsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Chat>>(chatsDb);
        }
        public async Task<IEnumerable<ChatDTO>> GetAllChatsOfUserById(int userId)
        {
            var userDb = await _usersRepository.GetByIdIncludeChatsConnectedToOrDefaultAsync(userId);
            if (userDb == null)
            {
                throw new ArgumentException("No user with such id was found");
            }
            return _mapper.Map<List<ChatDTO>>(userDb.ChatsConnectedTo);
        }
        public async Task<ChatDTO> CreateChat(ChatDTO chat)
        {

            var chatDb = _mapper.Map<Chat>(chat);
            var userDb = await _usersRepository.GetByIdOrDefaultAsync(chatDb.HostUserId);
            if (userDb == null)
            {
                throw new ArgumentException($"No user with id such as {nameof(chatDb.HostUserId)} from chat from argument was found");
            }
            chatDb.UsersInvited.Add(userDb);
            var createdChat = await _chatsRepository.AddAsync(chatDb);
            return _mapper.Map<ChatDTO>(createdChat);
        }
        public async Task DeleteChatAndDisconnectUsersViaTransactionAsync(int chatId, int userId)
        {
            var chatDb = await _chatsRepository.GetByIdOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("No chat with such id was found");
            }
            if (chatDb.HostUserId != userId)
            {
                throw new UnauthorizedAccessException("Delete chat can only its host");
            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _usersRepository.DisconnectAllFromChatBeforeDeleteByChatIdAsync(chatId);
                await _messagesRepository.DeleteAllFromChatByIdAsync(chatId);
                await _chatsRepository.DeleteAsync(chatDb);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ChatDTO>> SearchForChat(string query)
        {
            var chatsDb = await _chatsRepository.GetAllAsync();
            chatsDb = chatsDb.Where(chat => chat.Name.Contains(query));
            return _mapper.Map<List<ChatDTO>>(chatsDb);
        }
        public async Task ConnectUserToChat(int userId, int chatId)
        {
            var chatDb = await _chatsRepository.GetByIdIncludeUsersInvolvedOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("No chat with such id was found");
            }
            var userDb = await _usersRepository.GetByIdOrDefaultAsync(chatId);
            if (userDb == null)
            {
                throw new ArgumentException("No user with such id was found");
            }
            if (chatDb.UsersInvited.Any(user => user.UserId == userId))
            {
                throw new InvalidOperationException("Such user is already in this chat");
            }
            chatDb.UsersInvited.Add(userDb);
            await _chatsRepository.UpdateAsync(chatDb);
        }
    }
}
