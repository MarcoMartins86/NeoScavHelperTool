using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.ValueObjects.Base;

namespace NeoScavHelperTool.Models.ValueObjects
{
    public class StringList<T, V> : GenericListBase<T, V>
        where T : IStringListItem<V>
    {
        private const string SEPARATOR = ",";

        protected StringList(List<T> values)
            : base(values) { }

        public static StringList<T, V> Parse(string value)
        {
            return string.IsNullOrEmpty(value)
                ? null
                : new StringList<T, V>(ParseInternal(value, SEPARATOR));
        }

        public override string ToString()
        {
            return ToString(SEPARATOR);
        }

        public static StringList<T, V> Of(params V[] values)
        {
            return values == null ? null : new StringList<T, V>(OfInternal(values));
        }
    }
}
