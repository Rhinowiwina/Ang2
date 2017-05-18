using System;

namespace LS.Core.Interfaces
{
    public interface IEntity<TPrimaryKey>
    {

        TPrimaryKey Id { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        bool IsDeleted { get; set; }
    }
}
