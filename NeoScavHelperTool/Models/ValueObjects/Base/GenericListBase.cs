using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NeoScavHelperTool.Models.ValueObjects.Base
{
    public abstract class GenericListBase<T, V> : ValueObject, IReadOnlyList<T>
        where T : IStringListItem<V>
    {
        private List<T> _values { get; set; }

        public T this[int index] => _values[index];

        public int Count => _values.Count;

        protected GenericListBase(List<T> values)
        {
            this._values = values;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected string ToString(string separator)
        {
            return string.Join(separator, _values);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var item in _values)
            {
                yield return item;
            }
        }

        protected static List<T> ParseInternal(string value, string separator)
        {
            string[] split = value.Split(
                separator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries
            );
            Func<string, T> parser;
            if (typeof(T) == typeof(ModRefValue<V>))
            {
                parser = (v) => (T)(object)ModRefValue<V>.Parse(v);
            }
            else if (typeof(T) == typeof(OrModRefValues<V>))
            {
                parser = (v) => (T)(object)OrModRefValues<V>.Parse(v);
            }
            else
            {
                throw new NotImplementedException(
                    $"Need to implement \"{typeof(T).Name}\" for parsing"
                );
            }

            return split.Select(parser.Invoke).ToList();
        }

        protected static List<T> OfInternal(params V[] values)
        {
            Func<V, T> factory;
            if (typeof(T) == typeof(ModRefValue<V>))
            {
                factory = (v) => (T)(object)ModRefValue<V>.Of(v);
            }
            else if (typeof(T) == typeof(OrModRefValues<V>))
            {
                factory = (v) => (T)(object)OrModRefValues<V>.Of(v);
            }
            else
            {
                throw new NotImplementedException(
                    $"Need to implement \"{typeof(T).Name}\" for factory"
                );
            }
            return values.Select(factory.Invoke).ToList();
        }
    }
}
