using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Radical.Servitizing.DTO
{
    [Serializable]
    public class FilterDTO
    {
        public string Property { get; set; }

        public string Operand { get; set; }

        public string Data { get; set; }

        public object Value { get; set; }

        public string Type { get; set; }

        public string Logic { get; set; } = "And";
    }
}
