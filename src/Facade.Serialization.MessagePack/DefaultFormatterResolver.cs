using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Facade.Serialization.MessagePack
{
    /// <summary>
    /// This is a static type that folows MessagePack-csharp approach to make available a set of formatter resolvers to be used during serialization and deserilization operations
    /// </summary>
    public class DefaultFormatterResolver : IFormatterResolver
    {

        /// <summary>
        /// Singleton instance to FormatterResolver
        /// </summary>
        public static readonly IFormatterResolver Instance = new DefaultFormatterResolver();

        /// <summary>
        /// Container of available resolvers
        /// </summary>
        public static readonly List<IFormatterResolver> Resolvers = new List<IFormatterResolver>
       {
                 DynamicEnumAsStringResolver.Instance,
                 DynamicObjectResolver.Instance,
                 StandardResolver.Instance,
                 BuiltinResolver.Instance,
                 DynamicGenericResolver.Instance,

        };

        /// <summary>
        /// Gets the Formatter that support type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        /// <summary>
        /// Adds a new resolver to the list of available formatters
        /// </summary>
        /// <param name="resolver"></param>
        public static void Add(IFormatterResolver resolver)
        {
            if (!Resolvers.Contains(resolver))
                Resolvers.Insert(0, resolver);
        }

        /// <summary>
        /// Adds a list of formatters
        /// </summary>
        /// <param name="resolvers"></param>
        public static void Register(IEnumerable<IFormatterResolver> resolvers)
        {
            Resolvers.AddRange(resolvers);
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {

                foreach (var resolver in Resolvers)
                {
                    Formatter = resolver.GetFormatter<T>();
                    if (Formatter != null)
                    {
                        return;
                    }
                }
            }


        }
    }
}
