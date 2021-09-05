﻿using MessagingService.DataAccess.Model;
using System.Collections.Generic;

namespace MessagingService.DataAccess.Repositories
{
    public interface IMessageRepository : IGenericRepository<MessageModel>
    {
        MessageModel GetSingleMessage(string id, string username);
        List<string> GetOlderMessages(string id, string username);
        string GetLatestMessage(string senderUsername, string receiverUsername);
    }

}
