using System;

namespace Net.Bluewalk.MongoDbEntities.Abstract
{
    public interface IEntityGuidBase
    {
        Guid Id { get; set; }
    }
}