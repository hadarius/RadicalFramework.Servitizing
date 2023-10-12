using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Radical.Uniques;

namespace Radical.Servitizing.Entity.Identifier;

[DataContract]
public class Identifier<TEntity> : Identifier, IIdentifier<TEntity> where TEntity : UniqueIdentifiable
{
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual TEntity Entity { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public class Identifier : Entity, IIdentifier
{
    private long _code;

    private string _lastvalue;
    private string _value;

    public virtual long EntityId { get; set; }

    public virtual IdKind Kind { get; set; }

    public virtual string Name { get; set; }

    public virtual string Value
    {
        get => _value;
        set => _value = value;
    }

    public new long Key
    {
        get
        {
            if (_value != _lastvalue)
            {
                _code = (long)_value.UniqueKey();
                _lastvalue = _value;
            }
            return _code;
        }
        set
        {
            if (_value == _lastvalue
                && _code != value)
            {
                _code = value;
            }
        }
    }
}

public enum IdKind
{
    ID = 1,
    EXTID = 3,
    EMAIL = 7,
    PHONE = 11,
    SSO = 13,
    EAN = 17,
    UPC = 19,
    PAN = 23,
    ISBN = 31,
    CARD = 37,
    CHIP = 43,
    IBAN = 51,
    NFC = 61,
    VAT = 91,
    TAX = 123,
    PERSONAL = 137,
    NATIONAL = 143,
    IDCARD = 151,
    PASSPORT = 167,
    NAME = 173,
    CATEGORY = 179,
    GROUP = 181,
    IP = 191,
    URI = 193,
    DATETIME = 197,
    MONEY = 199,
    SCORE = 211,
    CODE = 223,
    NUMBER = 227
}
