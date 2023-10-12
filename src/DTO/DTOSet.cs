using System.Collections.ObjectModel;
using System.Linq;
using Radical.Series;
using Radical.Uniques;

namespace Radical.Servitizing.DTO
{

    public class DTOSet<TDto> : KeyedCollection<long, TDto>, IFindableSeries where TDto : IUniqueObject
    {
        protected override long GetKeyForItem(TDto item)
        {
            return item.Id == 0 ? item.AutoId() : item.Id;
        }

        public TDto Single
        { 
            get => this.FirstOrDefault();
        }

        public object this[object key]
        {
            get
            {
                TryGetValue((long)key.UniqueKey64(), out TDto result);
                return result;
            }
            set
            {
                Dictionary[(long)key.UniqueKey64()] = (TDto)value;
            }
        }
    }
}