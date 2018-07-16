using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MessagePack;
using MessagePack.Formatters;

namespace Facade.Serialization.MessagePack
{
    public abstract class TypeMessagePackFormatter<T> : IMessagePackFormatter<T>
    {
        protected delegate int SerializeMethod(object formatter, ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver);
        delegate object DeserializeMethod(object formatter, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize);

        protected Dictionary<string, Func<byte[], int, IFormatterResolver, T, int>> DeserializationHandlers = new Dictionary<string, Func<byte[], int, IFormatterResolver, T, int>>();
        

        protected int SerializeProperty<U>(ref byte[] bytes, int offset, string name, U contents, IFormatterResolver formatterResolver, Func<object> valueFormatterHandler = null)
        {
            var initialSize = offset;
            offset += formatterResolver.GetFormatter<string>().Serialize(ref bytes, offset, name, formatterResolver);

            if (contents == null)
            {
                offset += MessagePackBinary.WriteNil(ref bytes, offset);
                return offset - initialSize;
            }

            // element value

            var fromResolver = formatterResolver.GetFormatterDynamic(contents.GetType());
            var fromHandler = valueFormatterHandler != null ? valueFormatterHandler() : fromResolver;

            var primaryElementFormatter = fromResolver == null ? valueFormatterHandler() : fromResolver;
            offset += SerializeField(ref bytes, offset, contents, primaryElementFormatter, formatterResolver);


            return offset - initialSize;
        }


        protected virtual int DeserializeProperties(byte[] bytes, int offset, IFormatterResolver formatterResolver, int expectedFields, T instance)
        {
            int startOffset = offset;
            Func<byte[], int, IFormatterResolver, T, int> handler;
            do
            {
                int typeStringSize = 0;
                string fieldName = formatterResolver.GetFormatter<string>().Deserialize(bytes, offset, formatterResolver, out typeStringSize);
                offset += typeStringSize;

                if (!this.DeserializationHandlers.TryGetValue(fieldName, out handler))
                {
                    throw new KeyNotFoundException($"There is no handler to deserialize a deserialized member named {fieldName} on VBSubject.DataCollection type");
                }


                if (bytes[offset] == 0xc0)
                {
                    offset += 1;
                }
                else
                {
                    offset += handler(bytes, offset, formatterResolver, instance);

                    expectedFields--;
                }

                

            } while (expectedFields > 0);

            return offset - startOffset;
        }


        protected U DeSerializeProperty<U>(Func<object> valueFormatterHandler, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var initialSize = offset;

            if (bytes[offset] == 0xc0)
            {
                readSize = 1;
                return default(U);
            }

            //int fieldNameSize = 0;
            //_ = MessagePackBinary.ReadString(bytes, offset, out fieldNameSize);
            //offset += fieldNameSize;
            var fromResolver = formatterResolver.GetFormatterDynamic(typeof(U));
            var fromHandler = valueFormatterHandler != null ? valueFormatterHandler() : fromResolver;

            var primaryElementFormatter = fromResolver != null ? fromResolver : fromHandler;

            int valueSize = 0;
            object instance = DeSerializeField(typeof(U), bytes, offset, primaryElementFormatter, formatterResolver, out valueSize);
            offset += valueSize;
            readSize = offset - initialSize;
            return (U)instance;
        }

        protected virtual int SerializeField<U>(ref byte[] totalBytes, int offset, U field, object formatter, IFormatterResolver formatterResolver)
        {

            var fieldStartIndex = offset;

            var serializationMethod = GetSerializationMethod(field.GetType());

            offset += serializationMethod(formatter, ref totalBytes, offset, field, formatterResolver);
            var fieldEndIndex = offset - 1;

            return offset - fieldStartIndex;
        }

        protected virtual object DeSerializeField(Type type, byte[] totalBytes, int offset, object formatter, IFormatterResolver formatterResolver, out int readSize)
        {

            var fieldStartIndex = offset;

            var deSerializationMethod = GetDeSerializationMethod(type);

            object instance = deSerializationMethod(formatter, totalBytes, offset, formatterResolver, out readSize);

            return instance;
        }

        protected virtual SerializeMethod GetSerializationMethod(Type type)
        {

            var formatterType = typeof(IMessagePackFormatter<>).MakeGenericType(type);
            var param0 = Expression.Parameter(typeof(object), "formatter");
            var param1 = Expression.Parameter(typeof(byte[]).MakeByRefType(), "bytes");
            var param2 = Expression.Parameter(typeof(int), "offset");
            var param3 = Expression.Parameter(typeof(object), "value");
            var param4 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

            var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(byte[]).MakeByRefType(), typeof(int), type, typeof(IFormatterResolver) });

            var body = Expression.Call(
                Expression.Convert(param0, formatterType),
                serializeMethodInfo,
                param1,
                param2,
                type.GetTypeInfo().IsValueType ? Expression.Unbox(param3, type) : Expression.Convert(param3, type),
                param4);

            var lambda = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3, param4).Compile();

            return lambda;
        }


        private DeserializeMethod GetDeSerializationMethod(Type type)
        {
            var ti = type.GetTypeInfo();
            var formatterType = typeof(IMessagePackFormatter<>).MakeGenericType(type);
            var param0 = Expression.Parameter(typeof(object), "formatter");
            var param1 = Expression.Parameter(typeof(byte[]), "bytes");
            var param2 = Expression.Parameter(typeof(int), "offset");
            var param3 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");
            var param4 = Expression.Parameter(typeof(int).MakeByRefType(), "readSize");

            var deserializeMethodInfo = formatterType.GetRuntimeMethod("Deserialize", new[] { typeof(byte[]), typeof(int), typeof(IFormatterResolver), typeof(int).MakeByRefType() });

            var deserialize = Expression.Call(
                Expression.Convert(param0, formatterType),
                deserializeMethodInfo,
                param1,
                param2,
                param3,
                param4);

            Expression body = deserialize;
            if (ti.IsValueType)
                body = Expression.Convert(deserialize, typeof(object));
            var lambda = Expression.Lambda<DeserializeMethod>(body, param0, param1, param2, param3, param4).Compile();

            return lambda;

        }

        public abstract int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver);

        public abstract T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize);

      

    }
}
