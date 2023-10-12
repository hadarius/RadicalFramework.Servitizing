using Microsoft.OData.Edm;
using System;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Data.Service
{
    public partial class StreamDataService<TStore> where TStore : IDataServiceStore
    {
        public StreamDataService(Uri serviceUri)
        {
        }
    }
}