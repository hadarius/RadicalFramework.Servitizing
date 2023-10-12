using System.Text.Json.Serialization;

namespace Radical.Servitizing.Data.Store.Relation;

using Entity;
using Uniques;

public interface IRelationNode<TLeft, TRight> : IUniqueIdentifiable where TLeft : class, IUniqueIdentifiable where TRight : class, IUniqueIdentifiable
{
    [JsonIgnore]
    TLeft LeftEntity { get; set; }
    long LeftEntityId { get; set; }
    [JsonIgnore]
    TRight RightEntity { get; set; }
    long RightEntityId { get; set; }
}