using Radical.Servitizing.Entity.Identifier;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Radical.Servitizing.DTO
{
    [DataContract]
    public class IdentifierDTO<TDto> : IdentifierDTO where TDto : DTO
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TDto Entity { get; set; }
    } 

    public class IdentifierDTO : DTO
    {
        public virtual long EntityId { get; set; }
         
        public IdKind Kind { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public long Key { get; set; }
    }
}