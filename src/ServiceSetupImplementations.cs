using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Radical.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Servitizing;

using Data.Store;
using Entity;
using Repository;
using Repository.Client.Linked;
using Repository.Endpoint.Store;

public partial class ServiceSetup
{
    public IServiceSetup AddDomainImplementations()
    {
        IServiceRegistry service = registry;        
      
        HashSet<Type> duplicateCheck = new HashSet<Type>();
        Type[] stores = DataStoreRegistry.Stores.Where(s => s.IsAssignableTo(typeof(IDatabaseStore))).ToArray();

        service.AddScoped<ILinkedSynchronizer, LinkedSynchronizer>();

        foreach (ISeries<IEntityType> contextEntityTypes in DataStoreRegistry.Entities)
        {
            foreach (IEntityType _entityType in contextEntityTypes)
            {
                Type entityType = _entityType.ClrType;
                if (duplicateCheck.Add(entityType))
                {
                    foreach (Type store in stores)
                    {
                        service.AddScoped(
                            typeof(IStoreRepository<,>).MakeGenericType(store, entityType),
                            typeof(StoreRepository<,>).MakeGenericType(store, entityType));

                        service.AddSingleton(
                            typeof(IEntityCache<,>).MakeGenericType(store, entityType),
                            typeof(EntityCache<,>).MakeGenericType(store, entityType));
                    }
                }
            }
        }
        return this;
    }
}