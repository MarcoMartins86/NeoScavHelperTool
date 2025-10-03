using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.ValueObjects.Base;

namespace NeoScavHelperTool.Models.ValueObjects
{
    public class AndModRefValues<V> : GenericListBase<ModRefValue<V>, V>, IStringListItem<V>
    {
        private const string SEPARATOR = "&";

        protected AndModRefValues(List<ModRefValue<V>> values)
            : base(values) { }

        public static AndModRefValues<V> Parse(string value)
        {
            return string.IsNullOrEmpty(value)
                ? null
                : new AndModRefValues<V>(ParseInternal(value, SEPARATOR));
        }

        public override string ToString()
        {
            return ToString(SEPARATOR);
        }

        public static AndModRefValues<V> Of(params V[] values)
        {
            return values == null ? null : new AndModRefValues<V>(OfInternal(values));
        }

        public static AndModRefValues<V> Of(string mod, params V[] values)
        {
            return values == null ? null : new AndModRefValues<V>(OfInternal(mod, values));
        }
    }
}
