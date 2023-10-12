using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Radical.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Radical.Servitizing.Entity
{
    using Instant.Updating;
    using Instant;
    using Identifier;
    using Instant.Proxies;
    using Radical.Servitizing.Event;
    using AutoMapper;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public class Entity : UniqueIdentifiable, IEntity, INotifyPropertyChanged
    {
        public Entity()
        {
            CreateProxy((v) =>
            {
                Type type = this.GetProxyEntityType();

                if (TypeKey == 0)
                    TypeKey = type.UniqueKey32();

                if (type.IsAssignableTo(typeof(IProxy)) || type.IsAssignableTo(typeof(Event)))
                    return;

                v.Proxy = ProxyFactory.GetCreator(type, (uint)TypeKey).Create(this);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public override ulong TypeKey
        {
            get =>
                TypeKey == 0
                    ? (TypeKey = this.GetProxyEntityTypeKey32())
                    : TypeKey;
            set
            {
                if (value != 0 && value != TypeKey)
                    TypeKey = this.GetProxyEntityTypeKey32();
            }
        }
    }
}
