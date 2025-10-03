using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.ValueObjects.Base;

namespace NeoScavHelperTool.Models.ValueObjects
{
    public class OrModRefValues<V> : GenericListBase<ModRefValue<V>, V>, IStringListItem<V>
    {
        private const string SEPARATOR = "|";

        protected OrModRefValues(List<ModRefValue<V>> values)
            : base(values) { }

        public static OrModRefValues<V> Parse(string value)
        {
            return string.IsNullOrEmpty(value)
                ? null
                : new OrModRefValues<V>(ParseInternal(value, SEPARATOR));
        }

        public override string ToString()
        {
            return ToString(SEPARATOR);
        }

        public static OrModRefValues<V> Of(params V[] values)
        {
            return values == null ? null : new OrModRefValues<V>(OfInternal(values));
        }

        public static OrModRefValues<V> Of(string mod, params V[] values)
        {
            return values == null ? null : new OrModRefValues<V>(OfInternal(mod, values));
        }
    }
}
