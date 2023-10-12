using Microsoft.OData.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Radical.Series;
using System.Threading.Tasks;
using Radical.Uniques;
using Radical.Servitizing.Repository;

namespace Radical.Servitizing.Data.Service.Link
{
    public interface ILinkedSet<TStore, TEntity> : ILinkedSet<TEntity>
    {
    }

    public interface ILinkedSet<TEntity> : ICollection<TEntity>, IEnumerable<TEntity>, IEnumerable, IList<TEntity>
    {
        DataServiceContext Context { get; }

        void LoadAsync<TOrigin>(TOrigin origin, Func<TOrigin, Expression<Func<TEntity, bool>>> predicate);

        void Load<TOrigin>(TOrigin origin, Func<TOrigin, Expression<Func<TEntity, bool>>> predicate);

        void Load(Expression<Func<TEntity, bool>> predicate);

        void LoadAsync(Expression<Func<TEntity, bool>> predicate);

        void Load(Func<IQueryable<TEntity>, IQueryable<TEntity>> query);

        void LoadAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query);

        event EventHandler<LoadCompletedEventArgs> LoadCompleted;

        void LoadAsync(int offset = 0, int limit = 0);

        void Load(int offset = 0, int limit = 0);

        Task SaveAsync();

        void Save();
    }

    public class LinkedSet<TStore, TEntity> : LinkedSet<TEntity>, ILinkedSet<TStore, TEntity> where TEntity : class, IUniqueIdentifiable
    {
        private ILinkedRepository<TEntity> _repository;

        public LinkedSet(ILinkedRepository<TStore, TEntity> repository) : base(repository.Query)
        {
            _repository = repository;
        }
    }

    public class LinkedSet<TEntity> : DataServiceCollection<TEntity>, ILinkedSet<TEntity>, IFindableSeries where TEntity : class, IUniqueIdentifiable
    {
        protected DataServiceContext context;
        protected DataServiceQuery<TEntity> _query;
        protected ISeries<TEntity> _deck = new Registry<TEntity>();
        protected object origin;

        public DataServiceContext Context => context;

        public object this[object key]
        {
            get
            {
                if (_deck.TryGet(key, out TEntity entity))
                    return entity;
                var query = context.CreateQuery<TEntity>(KeyString(key), true);
                entity = query.FirstOrDefault();
                if (entity != null)
                    InsertItem(Count, entity);
                return entity;
            }
            set => SetItem(_deck.GetItem(((TEntity)this[key]).Id).Index, (TEntity)value);
        }

        public object this[object[] keys]
        {
            get
            {
                if (_deck.TryGet(keys, out TEntity entity))
                    return entity;
                var query = context.CreateQuery<TEntity>(KeyString(keys), true);
                entity = query.FirstOrDefault();
                if (entity != null)
                    InsertItem(Count, entity);
                return entity;
            }
            set => SetItem(_deck.GetItem(((TEntity)this[keys]).Id).Index, (TEntity)value);
        }

        public LinkedSet() : base()
        {
        }
        public LinkedSet(DataServiceQuery<TEntity> query) : base(query.Context)
        {
            _query = query;
            context = query.Context;
        }
        public LinkedSet(DataServiceContext context, IQueryable<TEntity> query) : base(context)
        {
            _query = (DataServiceQuery<TEntity>)query;
            this.context = context;
        }

        public LinkedSet(DataServiceQuerySingle<TEntity> item) : base(item)
        {
            context = item.Context;
            _query = context.CreateQuery<TEntity>(typeof(TEntity).Name);
        }
        public LinkedSet(IEnumerable<TEntity> items) : base(items)
        {
        }
        public LinkedSet(TrackingMode trackingMode, DataServiceQuerySingle<TEntity> item)
            : base(trackingMode, item)
        {
            context = item.Context;
            _query = context.CreateQuery<TEntity>(typeof(TEntity).Name);
        }
        public LinkedSet(IEnumerable<TEntity> items, TrackingMode trackingMode)
            : base(items, trackingMode)
        {
        }
        public LinkedSet(DataServiceContext context) : base(context)
        {
            this.context = context;
            _query = context.CreateQuery<TEntity>(typeof(TEntity).Name);
        }
        public LinkedSet(DataServiceContext context, string entitySetName, Func<EntityChangedParams, bool> entityChangedCallback, Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : base(context, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
            this.context = context;
            _query = context.CreateQuery<TEntity>(typeof(TEntity).Name);
        }
        public LinkedSet(IEnumerable<TEntity> items, TrackingMode trackingMode, string entitySetName, Func<EntityChangedParams, bool> entityChangedCallback, Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : base(items, trackingMode, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }
        public LinkedSet(DataServiceContext context, IEnumerable<TEntity> items, TrackingMode trackingMode, string entitySetName, Func<EntityChangedParams, bool> entityChangedCallback, Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : base(context, items, trackingMode, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
            this.context = context;
            _query = context.CreateQuery<TEntity>(typeof(TEntity).Name);
        }

        public virtual DataServiceQuery<TEntity> Query => _query;

        public virtual void Load(int offset = 0, int limit = 0)
        {
            var q = limit > 0 ? _query.Skip(offset).Take(limit) : _query;
            Load(q);
        }
        public void LoadAsync(int offset = 0, int limit = 0)
        {
            var q = limit > 0 ? _query.Skip(offset).Take(limit) : _query;
            LoadAsync(q);
        }
        public virtual void Load(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
        {
            Load(query(_query));
        }
        public virtual void Load(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate != null)
                Load(_query.Where(predicate));
        }
        public virtual void Load<TOrigin>(TOrigin origin, Func<TOrigin, Expression<Func<TEntity, bool>>> predicate)
        {
            this.origin = origin;
            if (predicate != null)
                Load(predicate.Invoke(origin));
        }

        public virtual void LoadAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
        {
            LoadAsync(query(_query));
        }
        public virtual void LoadAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate != null)
                LoadAsync(_query.Where(predicate));
        }
        public virtual void LoadAsync<TOrigin>(TOrigin origin, Func<TOrigin, Expression<Func<TEntity, bool>>> predicate)
        {
            this.origin = origin;
            if (predicate != null)
                LoadAsync(predicate.Invoke(origin));
        }

        public virtual void Save()
        {
            context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        public virtual async Task SaveAsync()
        {
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
        }

        public virtual void AddRange(IEnumerable<TEntity> items)
        {
            items.DoEach(e => Add(e));
        }

        protected override void InsertItem(int index, TEntity item)
        {
            _deck.Insert(index, item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            _deck.RemoveAt(index);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TEntity item)
        {
            _deck.Set(item.Id, item);
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            _deck.Clear();
            base.ClearItems();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            var olditem = _deck[oldIndex];
            _deck.RemoveAt(oldIndex);
            _deck[newIndex] = olditem;
            base.MoveItem(oldIndex, newIndex);
        }

        public bool ContainsKey(object key)
        {
            return _deck.ContainsKey(key);
        }

        public string KeyString(params object[] keys)
        { return $"{typeof(TEntity).Name}({(keys.Length > 1 ? keys.Aggregate(string.Empty, (a, b) => $"{a},{b}") : keys[0])})"; }

    }
}