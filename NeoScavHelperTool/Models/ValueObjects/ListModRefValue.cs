using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Models.ValueObjects
{
    public class ListModRefValue<T> : ValueObject, IReadOnlyList<ModRefValue<T>>
    {
        private static readonly char[] LIST_VALUE_SEPARATOR = { ',' };
        private List<ModRefValue<T>> _modRefValues { get; set; }

        protected ListModRefValue() { }

        protected ListModRefValue(List<ModRefValue<T>> values)
        {
            this._modRefValues = values;
        }

        public ModRefValue<T> this[int index] => _modRefValues[index];

        public int Count => _modRefValues.Count;

        public IEnumerator<ModRefValue<T>> GetEnumerator()
        {
            return _modRefValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ListModRefValue<T> Parse(string value)
        {
            string[] split = value.Split(
                LIST_VALUE_SEPARATOR,
                StringSplitOptions.RemoveEmptyEntries
            );
            return new ListModRefValue<T>(split.Select(ModRefValue<T>.Parse).ToList());
        }

        public override string ToString()
        {
            return string.Join(LIST_VALUE_SEPARATOR[0].ToString(), _modRefValues);
        }

        public static ListModRefValue<T> Of(params T[] values)
        {
            return new ListModRefValue<T>(values.Select(ModRefValue<T>.Of).ToList());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var item in _modRefValues)
            {
                yield return item;
            }
        }
    }
}
