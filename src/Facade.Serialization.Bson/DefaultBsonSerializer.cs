using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Facade.Serialization.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Facade.Serialization.Bson
{
    public class DefaultBsonSerializer : ISerializer
    {
        private readonly IDictionary<Type, IBsonSerializer> serializers;
        private BsonBinaryWriterSettings writerSettings;
        private BsonBinaryReaderSettings readerSettings;

        public DefaultBsonSerializer(BsonBinaryReaderSettings readerSettings = null, BsonBinaryWriterSettings writerSettings = null)
        {
            this.writerSettings = writerSettings ?? BsonBinaryWriterSettings.Defaults;
            this.readerSettings = readerSettings ?? BsonBinaryReaderSettings.Defaults;
        }



        public DefaultBsonSerializer(IDictionary<Type, IBsonSerializer> serializers)
        {
            this.serializers = serializers;
            this.RegisterSerializers(serializers);
        }

        private void RegisterSerializers(IDictionary<Type, IBsonSerializer> serializers)
        {
            foreach (var serializer in serializers)
            {
                BsonSerializer.RegisterSerializer(serializer.Key, serializer.Value);
            }
            
        }

        public Task<T> DeSerialize<T>(ReadOnlySequence<byte> bytes) where T : class
        {
            MemoryStream memStream = new MemoryStream(bytes.ToArray());
            memStream.Position = 0;
            var instanceTask = DeSerialize<T>(memStream);
            return instanceTask;

        }

        public Task<T> DeSerialize<T>(Stream stream) where T : class
        {
            
            return Execution.Async<Stream, T>(stream, (_stream) => 
            {
                var serializer = BsonSerializer.LookupSerializer(typeof(T));
                 
                using (var bsonReader = new BsonBinaryReader(_stream, readerSettings))
                {
                    var context = BsonDeserializationContext.CreateRoot(bsonReader);
                    BsonDeserializationArgs args = new BsonDeserializationArgs();
                    args.NominalType = typeof(T);
                    T instance = serializer.Deserialize(context, args) as T;
                    return instance;
                }
                 
            });
            
        }

        public Task Serialize<T>(Stream stream, T instance) where T : class
        {
            return  Execution.Async(instance, (_instance) =>
            {
                var serializer = BsonSerializer.LookupSerializer(typeof(T));

                using (var bsonWriter = new BsonBinaryWriter(stream, writerSettings ?? BsonBinaryWriterSettings.Defaults))
                {
                    var context = BsonSerializationContext.CreateRoot(bsonWriter);
                    BsonSerializationArgs args = new BsonSerializationArgs();
                    args.NominalType = typeof(T);
                    serializer.Serialize(context, args, instance);
                }


            });
            
        }

        public Task<ReadOnlySequence<byte>> Serialize<T>(T instance) where T : class
        {
            return Execution.Async(instance, (_instance) => 
            {

                if (_instance == null)
                {
                    return new ReadOnlySequence<byte>();
                }
                
                var serializer = BsonSerializer.LookupSerializer(typeof(T));

                using (var memoryStream = new MemoryStream())
                {
                    Serialize(memoryStream, instance);
                    memoryStream.Position = 0;
                    return new ReadOnlySequence<byte>(memoryStream.ToArray());
                }
            });
            

        }
    }
}
