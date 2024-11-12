using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glitch9.IO.Network
{
    /// <summary>
    /// Provides methods to convert objects to and from cloud-storable formats.
    /// </summary>
    public static class CloudConverter
    {
        /// <summary>
        /// Sets the logger to output logs.
        /// </summary>
        public static ILogger Logger { get; set; } = new CloudConverterLogger();

        private static readonly Dictionary<Type, ICloudConverter> k_Converters = new();
        private static readonly object k_LockObject = new(); // Lock object for synchronization

        private static readonly Type[] k_TypesToIgnore = new Type[]
        {
            typeof(DateTime),
            typeof(Sync<>),
        };

        static CloudConverter()
        {
            RegisterAllConverters();
        }

        /// <summary>
        /// Registers all converters that implement the ICloudConverter interface.
        /// </summary>
        private static void RegisterAllConverters()
        {
            Type converterInterface = typeof(ICloudConverter);
            List<Type> converters = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => converterInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            foreach (Type converterType in converters)
            {
                if (converterType.IsGenericTypeDefinition) continue; // Skip generic type definitions

                Type targetType = GetTargetType(converterType);
                if (targetType == null) continue;

                ICloudConverter instance = (ICloudConverter)Activator.CreateInstance(converterType);
                k_Converters[targetType] = instance;
            }

            // Register the ArrayConverter manually
            k_Converters[typeof(Array)] = new ArrayConverter();
        }

        /// <summary>
        /// Registers a generic converter for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be converted.</typeparam>
        /// <param name="converter">The converter instance.</param>
        public static void RegisterGenericConverter<T>(CloudConverter<T> converter)
        {
            lock (k_LockObject)
            {
                k_Converters[typeof(T)] = converter;
            }
        }

        /// <summary>
        /// Retrieves the converter for the specified type.
        /// </summary>
        /// <param name="type">The type for which to retrieve the converter.</param>
        /// <returns>The converter for the specified type, or null if no converter is found.</returns>
        public static ICloudConverter GetConverter(Type type)
        {
            if (type == null)
            {
                GNLog.Error("The type to be converted is null.");
                return null;
            }

            lock (k_LockObject) // Synchronize using the lock object
            {
                if (k_Converters.TryGetValue(type, out ICloudConverter converter))
                {
                    return converter;
                }

                // Try to register a generic converter dynamically
                if (type.IsGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();

                    // Register an EnumConverter dynamically
                    if (type.IsEnum)
                    {
                        Type genericConverterType = typeof(EnumConverter<>).MakeGenericType(type);
                        converter = (ICloudConverter)Activator.CreateInstance(genericConverterType);

                        k_Converters.Add(type, converter);
                        return converter;
                    }

                    if (genericTypeDefinition == typeof(Dictionary<,>))
                    {
                        Type keyType = type.GetGenericArguments()[0];
                        Type valueType = type.GetGenericArguments()[1];
                        Type genericConverterType = typeof(DictionaryConverter<,>).MakeGenericType(keyType, valueType);
                        converter = (ICloudConverter)Activator.CreateInstance(genericConverterType);

                        k_Converters.Add(type, converter);
                        return converter;
                    }

                    if (genericTypeDefinition == typeof(List<>))
                    {
                        Type elementType = type.GetGenericArguments()[0];
                        Type genericConverterType = typeof(ListConverter<>).MakeGenericType(elementType);
                        converter = (ICloudConverter)Activator.CreateInstance(genericConverterType);

                        k_Converters.Add(type, converter);
                        return converter;
                    }

                    if (genericTypeDefinition == typeof(MinMax<>))
                    {
                        Type elementType = type.GetGenericArguments()[0];
                        Type genericConverterType = typeof(MinMaxConverter<>).MakeGenericType(elementType);
                        converter = (ICloudConverter)Activator.CreateInstance(genericConverterType);

                        k_Converters.Add(type, converter);
                        return converter;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the given property value to the local format.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The converted value in local format.</returns>
        public static object ToLocalFormat(Type type, string propertyName, object propertyValue)
        {
            if (propertyValue == null)
            {
                GNLog.Error($"The value of {propertyName} is null.");
                return null;
            }

            if (IsIgnoredType(type)) return propertyValue;

            if (type == typeof(string) || type.HasInterface<ICloudIdOnly>()) return propertyValue.ToString();
            if (type == typeof(Guid)) return new Guid(propertyValue.ToString());
            if (type == typeof(ClockTime)) return new ClockTime(Convert.ToInt32(propertyValue));
            if (type == typeof(UnixTime)) return new UnixTime(Convert.ToInt64(propertyValue));
            if (type.IsNumber() && !type.IsEnum) return Convert.ChangeType(propertyValue, type);

            ICloudConverter converter = GetConverter(type);
            if (converter == null) return propertyValue;
            object converted = converter.ToLocalFormat(type, propertyName, propertyValue);
            return converted ?? Activator.CreateInstance(type);
        }

        /// <summary>
        /// Converts the given property value to the cloud format.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The converted value in cloud format.</returns>
        public static object ToCloudFormat(Type type, object propertyValue)
        {
            if (propertyValue == null)
            {
                GNLog.Warning($"The value of {type.Name} is null.");
                return null;
            }

            if (IsIgnoredType(type)) return propertyValue;

            if (type.HasInterface<ICloudIdOnly>())
            {
                if (propertyValue is ICloudIdOnly idOnly) return idOnly.GetId();
                else return LogWrongType<ICloudIdOnly>(propertyValue);
            }

            if (type == typeof(Guid))
            {
                if (propertyValue is Guid uuid) return uuid.ToString();
                else return LogWrongType<Guid>(propertyValue);
            }

            if (type == typeof(string))
            {
                if (propertyValue is string stringValue) return stringValue;
                else return LogWrongType<string>(propertyValue);
            }

            if (type == typeof(ClockTime)) // Should not come after type.IsNumericType() (might be recognized as a numeric value)
            {
                if (propertyValue is ClockTime clockTime) return clockTime.Value;
                else return LogWrongType<ClockTime>(propertyValue);
            }

            if (type == typeof(UnixTime)) // Should not come after type.IsNumericType() (might be recognized as a numeric value)
            {
                if (propertyValue is UnixTime unixTime) return unixTime.Value;
                else return LogWrongType<UnixTime>(propertyValue);
            }

            // Do not convert numeric types (Firestore handles this automatically)
            if (type.IsNumber()) return propertyValue;

            ICloudConverter converter = GetConverter(type);
            if (converter == null) return propertyValue;

            return converter.ToCloudFormat(type, propertyValue);
        }

        /// <summary>
        /// Logs an error for a wrong type and returns null.
        /// </summary>
        /// <typeparam name="TType">The expected type.</typeparam>
        /// <param name="propertyValue">The actual value.</param>
        /// <returns>Null.</returns>
        private static object LogWrongType<TType>(object propertyValue)
        {
            Logger.Error($"The value is not of type {typeof(TType).Name}: {propertyValue.GetType()}");
            return null;
        }

        /// <summary>
        /// Checks if the specified type is in the list of ignored types.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is ignored, otherwise false.</returns>
        private static bool IsIgnoredType(Type type)
        {
            foreach (Type ignoreType in k_TypesToIgnore)
            {
                if (ignoreType == type)
                {
                    Logger.Error($"{type.Name} is a type that cannot be converted.");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves the target type of a converter.
        /// </summary>
        /// <param name="converterType">The type of the converter.</param>
        /// <returns>The target type, or null if it cannot be determined.</returns>
        private static Type GetTargetType(Type converterType)
        {
            if (converterType.IsGenericType && converterType.GetGenericTypeDefinition() == typeof(CloudConverter<>))
            {
                return converterType.GetGenericArguments()[0];
            }

            return converterType.BaseType?.GetGenericArguments().FirstOrDefault();
        }
    }
}
