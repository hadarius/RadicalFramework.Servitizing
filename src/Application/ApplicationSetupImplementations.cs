using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using Radical.Series;
using Radical.Servitizing.Data.Service.Link;
using Radical.Servitizing.Data.Service;
using Radical.Servitizing.Data.Store;
using Radical.Servitizing.Repository.Client.Linked;
using Radical.Servitizing.Repository;
using Radical.Servitizing.Entity;

namespace Radical.Servitizing.Application
{
    public partial class ApplicationSetup
    {
        public static void AddOpenDataServiceImplementations()
        {
            IServiceManager sm = ServiceManager.GetManager();
            IServiceRegistry service = sm.Registry;
            HashSet<Type> duplicateCheck = new HashSet<Type>();
            Type[] stores = new Type[] { typeof(IEntryStore), typeof(IReportStore) };

            /**************************************** DataService Entity Type Routines ***************************************/
            foreach (ISeries<IEdmEntityType> contextEntityTypes in OpenDataServiceRegistry.Entities)
            {
                foreach (IEdmEntityType _entityType in contextEntityTypes)
                {
                    Type entityType = OpenDataServiceRegistry.Mappings[_entityType.Name];

                    if (duplicateCheck.Add(entityType))
                    {
                        Type callerType = DataStoreRegistry.Callers[entityType.FullName];

                        /*****************************************************************************************/
                        foreach (Type store in stores)
                        {
                            if ((entityType != null) && (OpenDataServiceRegistry.GetContextType(store, entityType) != null))
                            {
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(ILinkedRepository<,>).MakeGenericType(store, entityType),
                                    typeof(LinkedRepository<,>).MakeGenericType(store, entityType));

                                service.AddScoped(
                                    typeof(IEntityCache<,>).MakeGenericType(store, entityType),
                                    typeof(EntityCache<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(ILinkedSet<,>).MakeGenericType(store, entityType),
                                    typeof(LinkedSet<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                if (callerType != null)
                                {
                                    /*********************************************************************************************/
                                    service.AddScoped(
                                        typeof(IRepositoryLink<,,>).MakeGenericType(store, callerType, entityType),
                                        typeof(RepositoryLink<,,>).MakeGenericType(store, callerType, entityType));

                                    service.AddScoped(
                                        typeof(ILinkedObject<,>).MakeGenericType(store, callerType),
                                        typeof(RepositoryLink<,,>).MakeGenericType(store, callerType, entityType));
                                    /*********************************************************************************************/
                                }
                            }
                        }
                    }
                }
            }
            //app.RebuildProviders();
        }
    }
}