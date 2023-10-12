using Radical.Instant;
using Radical.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Radical.Servitizing.Data.Service.Link;

using Instant.Rubrics;

public enum Towards
{
    None,
    ToSet,
    ToSingle,
    SetToSet
}

public abstract class DataServiceLinkBase : IDataServiceLink
{
    protected Uscn serialcode;

    protected DataServiceLinkBase() { }

    public virtual Towards Towards { get; set; }

    public virtual IUnique Empty => Uscn.Empty;

    public virtual Uscn SerialCode
    {
        get => serialcode;
        set => serialcode = value;
    }

    public virtual ulong Key
    {
        get => serialcode.Key;
        set => serialcode.Key = value;
    }
    public virtual ulong TypeKey
    {
        get => serialcode.TypeKey;
        set => serialcode.TypeKey = value;
    }

    public virtual int CompareTo(IUnique other)
    {
        return serialcode.CompareTo(other);
    }

    public virtual bool Equals(IUnique other)
    {
        return serialcode.Equals(other);
    }

    public virtual byte[] GetBytes()
    {
        return serialcode.GetBytes();
    }

    public virtual byte[] GetKeyBytes()
    {
        return serialcode.GetKeyBytes();
    }

    public virtual MemberRubric LinkedMember { get; }
}
