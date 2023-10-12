using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Radical.Servitizing.DTO
{
    [Serializable]
    public class QueryDTO
    {
        public List<FilterDTO> Filter { get; set; } = new();

        public List<SortDTO> Sort { get; set; } = new();

    }
}
