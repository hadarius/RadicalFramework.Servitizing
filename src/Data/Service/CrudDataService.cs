using Microsoft.OData.Edm;
using System;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Data.Service
{
    public partial class CrudDataService<TStore> where TStore : IDataServiceStore
    {
        public CrudDataService(Uri serviceUri)
        {
        }
    }
}