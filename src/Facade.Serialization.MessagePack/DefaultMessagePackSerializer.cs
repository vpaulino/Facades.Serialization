using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using Facade.Serialization.Extensions;

namespace Facade.Serialization.MessagePack
{
    public class DefaultMessagePackSerializer : ISerializer
    {

        IFormatterResolver formatterResolver;

        public DefaultMessagePackSerializer(IFormatterResolver formatterResolver)
        {
            this.formatterResolver = formatterResolver;
        }

        public Task Serialize<T>(Stream stream, T instance) where T : class
        {
            return global::MessagePack.MessagePackSerializer.SerializeAsync<T>(stream, instance, this.formatterResolver);
        }

        public Task<ReadOnlySequence<byte>> Serialize<T>(T instance) where T : class
        {

            return Execution.Async<T, ReadOnlySequence<byte>>(instance, (_instance) =>
            {

                if (_instance == null)
                {
                    return new ReadOnlySequence<byte>();
                }


                byte[] bytes = global::MessagePack.MessagePackSerializer.Serialize(_instance, formatterResolver);

                return new ReadOnlySequence<byte>(bytes);
            });

            
        }

        public Task<T> DeSerialize<T>(ReadOnlySequence<byte> bytesSequence) where T : class
        { 

            return Execution.Async<ReadOnlySequence<byte>, T>(bytesSequence, (_bytesSequence) =>
            {

                var bytes = _bytesSequence.GetArraySegment().Array;
                T instance = global::MessagePack.MessagePackSerializer.Deserialize<T>(bytes, formatterResolver);

                return instance;

            });
            
        }

        public Task<T> DeSerialize<T>(Stream stream) where T : class
        {
            if (stream == null || (stream != null && (!stream.CanRead || stream.Length == 0)))
                return Task.FromResult<T>(default(T));    

            var instance =  global::MessagePack.MessagePackSerializer.DeserializeAsync<T>(stream, formatterResolver);

            return instance;
        }




    }
}
