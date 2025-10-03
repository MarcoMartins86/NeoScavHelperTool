using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.ValueObjects.Base;

namespace NeoScavHelperTool.Models.ValueObjects
{
    public class ModRefValue<T> : ValueObject, IStringListItem<T>
    {
        private const string SEPARATOR = ":";

        public string ModRef { get; private set; }
        public T Value { get; private set; }

        protected ModRefValue(string modRef, T value)
        {
            this.ModRef = modRef;
            this.Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return ModRef;
            yield return Value;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(ModRef) ? Value.ToString() : $"{ModRef}{SEPARATOR}{Value}";
        }

        public static ModRefValue<T> Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] split = value.Split(
                SEPARATOR.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries
            );
            if (split.Length == 2)
            {
                return new ModRefValue<T>(split[0], (T)Convert.ChangeType(split[1], typeof(T)));
            }
            else if (split.Length == 1)
            {
                return new ModRefValue<T>("", (T)Convert.ChangeType(split[0], typeof(T)));
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid value \"{value}\" for a {typeof(ModRefValue<T>).Name}"
                );
            }
        }

        public static ModRefValue<T> Of(T value)
        {
            return value == null ? null : new ModRefValue<T>("", value);
        }

        public static ModRefValue<T> Of(string mod, T value)
        {
            return value == null ? null : new ModRefValue<T>(mod, value);
        }
    }
}
