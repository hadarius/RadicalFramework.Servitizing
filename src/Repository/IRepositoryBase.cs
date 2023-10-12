using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radical.Servitizing.Data.Mapper;
using Radical.Servitizing.Repository.Client.Linked;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository
{
    public interface IRepository : IRepositoryContext
    {
        Type ElementType { get; }

        Expression Expression { get; }

        IQueryProvider Provider { get; }

        IDataMapper Mapper { get; }

        CancellationToken Cancellation { get; set; }

        IEnumerable<ILinkedObject> LinkedObjects { get; set; }

        void LoadLinked(object entity);

        Task LoadLinkedAsync(object entity);

        void LoadRelated(EntityEntry entry, RelatedType relatedType);

        void LoadRelatedAsync(EntityEntry entry, RelatedType relatedType, CancellationToken token);
    }
}