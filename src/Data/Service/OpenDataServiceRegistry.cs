using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Radical.Servitizing.Data.Service;

using DTO;
using Link;
using Logging;
using Series;
using Store;
using Uniques;

public static class OpenDataServiceRegistry
{
    const int RECONNECT_TRYOUTS = 60;

    public static ISeries<ISeries<IEdmEntityType>> Entities = new Registry<ISeries<IEdmEntityType>>();
    public static ISeries<Type> Mappings = new Registry<Type>();
    public static ISeries<ISeries<Type>> Contexts = new Registry<ISeries<Type>>();
    public static ISeries<IEdmModel> EdmModels = new Registry<IEdmModel>();
    public static ISeries<Type> Stores = new Registry<Type>();
    public static ISeries<DataServiceLinkBase> Links = new Registry<DataServiceLinkBase>(true);

    public static IEdmModel GetEdmModel(this OpenDataService context)
    {
        Task<IEdmModel> model = context.GetEdmModelAsync();
        model.Wait();
        return model.Result;
    }

    public static async Task<IEdmModel> GetEdmModelAsync(this OpenDataService context)
    {
        // Get the service metadata's Uri
        var metadataUri = context.GetMetadataUri();
        // Create a HTTP request to the metadata's Uri 
        // in order to get a representation for the data model
        HttpClientHandler clientHandler = new HttpClientHandler();
        using (HttpClient client = new HttpClient(clientHandler))
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
            }
            int tryouts = RECONNECT_TRYOUTS;
            do
            {
                try
                {
                    using (var response = await client.GetAsync(metadataUri))
                    {
                        client.Success<Weblog>("Data Service Client - Connected");

                        if (response.IsSuccessStatusCode)
                        {
                            client.Success<Datalog>("Data Service Client - Metadata Retrieved");
                            // Translate the response into an in-memory stream
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {   // Convert the stream into an XML representation
                                using (var reader = XmlReader.Create(stream))
                                {   // Parse the XML representation of the data model
                                    // into an EDM that can be utilized by OData client libraries
                                    return CsdlReader.Parse(reader);
                                }
                            }
                        }
                        else
                        {
                            tryouts--;
                            client.Warning<Weblog>("Data Service Client - Http Get Metadata Request Failed", response);
                            Thread.Sleep(5000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tryouts--;
                    client.Warning<Datalog>("Data Service Client - Http Connection Failed", client.BaseAddress, ex);
                    Thread.Sleep(5000);
                }
            }
            while (tryouts > 0);

            client.Warning<Datalog>("Data Service Client - Retry Connection Limit Exceeded " +
                                    "- Unable To Retrieve Metadata From Endpoint", client.BaseAddress);
        }
        return null;
    }

    public static ISeries<IEdmEntityType> GetEdmEntityTypes(this OpenDataService context)
    {
        var contextType = context.GetType();

        if (!Entities.TryGet(contextType, out ISeries<IEdmEntityType> dsEntities))
        {
            dsEntities = new Registry<IEdmEntityType>();

            var entityTypes = context.GetServiceModel()
                                            .EntityContainer
                                            .EntitySets()
                                            .Select(p => p.EntityType())
                                            .ToArray();

            var iface = GetLinkedStoreTypes(contextType);

            foreach (var entityType in entityTypes)
            {
                dsEntities.Add(entityType.Name, entityType);

                var localEntityType = EdmAssemblyResolve(entityType);

                dsEntities.Add(localEntityType.FullName, entityType);

                if (!Contexts.TryGet(entityType.Name, out ISeries<Type> dsEntityContext))
                    dsEntityContext = new Registry<Type>();

                dsEntityContext.Put(iface, contextType);
                Contexts.Put(entityType.Name, dsEntityContext);
            }
            Entities.Add(contextType, dsEntities);
        }
        return dsEntities;
    }

    public static Type GetLinkedStoreType(this OpenDataService context)
    {
        return GetLinkedStoreTypes(context.GetType());
    }
    public static Type GetLinkedStoreTypes(Type contextType)
    {
        if (!Stores.TryGet(contextType, out Type iface))
        {
            var type = contextType.IsGenericType
                ? contextType
                : contextType.BaseType;

            iface = type.GenericTypeArguments
                   .Where(i => i
                   .IsAssignableTo(typeof(IDataServiceStore)))
                   .FirstOrDefault();

            if (iface == null)
                iface = typeof(IDatabaseStore);

            Stores.Put(iface, contextType);
            Stores.Put(contextType, iface);
        }
        return iface;
    }

    public static Type GetMappedType(this OpenDataService context, string name)
    {
        string sn = name.Split('.').Last();
        if (Mappings.TryGet(name, out Type t) ||
            Mappings.TryGet(sn, out t))
            return t;
        return Assemblies.FindType(sn);
    }
    public static string GetMappedName(this OpenDataService context, Type type)
    {
        string n = type.FullName;
        if (Entities.TryGet(context.GetType(), out ISeries<IEdmEntityType> deck))
            if (deck.TryGet(type.Name, out IEdmEntityType et))
                n = et.FullTypeName();
        return n;
    }

    public static Type GetContextType<TStore, TEntity>() where TEntity : class, IUniqueIdentifiable
    {
        return GetContextType(typeof(TStore), typeof(TEntity));
    }
    public static Type GetContextType(Type storeType, Type entityType)
    {
        if (Contexts.TryGet(entityType.Name, out ISeries<Type> dbEntityContext))
        {
            var iface = storeType
                .GetInterfaces()
                .Where(i => i.GetInterfaces()
                    .Contains(typeof(IDatabaseStore))).FirstOrDefault();

            if (iface == null && storeType == typeof(IDatabaseStore))
                iface = typeof(IDatabaseStore);

            if (dbEntityContext.TryGet(storeType, out Type contextType))
                return contextType;
        }

        return null;
    }
    public static Type[] GetContextTypes<TEntity>() where TEntity : class, IUniqueIdentifiable
    {
        if (Contexts.TryGet(typeof(TEntity).Name, out ISeries<Type> dbEntityContext))
            return dbEntityContext.ToArray();
        return null;
    }

    private static Type EdmAssemblyResolve(IEdmEntityType entityType)
    {
        var remoteName = entityType.Name;
        var remoteFullName = entityType.FullTypeName();
        Type localEntityType = null;
        if (remoteName.Contains("Identifier"))
        {
            var entityName = remoteName.Replace("IdentifierDTO", null);
            var argumentType = Assemblies.FindType(entityName);
            localEntityType = typeof(IdentifierDTO<>).MakeGenericType(argumentType);
        }
        else
        {
            localEntityType = Assemblies.FindType(remoteName);
        }

        if (localEntityType == null)
            return null;

        Mappings.Put(remoteName, localEntityType);
        Mappings.Put(remoteFullName, localEntityType);

        return localEntityType;
    }
}
