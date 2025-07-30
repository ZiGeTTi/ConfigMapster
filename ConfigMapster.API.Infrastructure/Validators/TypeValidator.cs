using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.Infrastructure.Validators
{

    public static class TypeValidator
    {
        public static T Parse<T>(string value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    if (int.TryParse(value, out var intValue))
                        return (T)(object)intValue;
                    break;

                case TypeCode.Double:
                    if (double.TryParse(value, out var doubleValue))
                        return (T)(object)doubleValue;
                    break;

                case TypeCode.Boolean:
                    if (bool.TryParse(value, out var boolValue))
                        return (T)(object)boolValue;
                    break;

                case TypeCode.String:
                    return (T)(object)value;

                default:
                    throw new InvalidCastException($"Unsupported type: {typeof(T)}");
            }

            throw new InvalidCastException($"Unable to parse '{value}' to type {typeof(T)}");
        }
    }
}
