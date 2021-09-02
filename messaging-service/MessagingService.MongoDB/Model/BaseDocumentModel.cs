using MongoDB.Bson;
using System;

namespace MessagingService.MongoDB.Model
{
    public abstract class BaseDocumentModel
    {
        public ObjectId Id { get; set; }
        public DateTime InsertAt { get; set; }
    }
}
