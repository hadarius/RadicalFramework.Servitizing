using System.ComponentModel.DataAnnotations.Schema;


namespace Radical.Servitizing.Data.Store.Relation;

using Uniques;
using Entity;

public class RelationNode<TLeft, TRight> : Entity, IRelationNode<TLeft, TRight> where TLeft : class, IUniqueIdentifiable where TRight : class, IUniqueIdentifiable
{
    public virtual long RightEntityId { get; set; }

    public virtual TRight RightEntity { get; set; }

    public virtual long LeftEntityId { get; set; }

    public virtual TLeft LeftEntity { get; set; }
}