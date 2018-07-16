using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Facade.Serialization.Extensions;

namespace Facade.Serialization.Json
{
    

    public class DefaultJsonSerializer : ISerializer
    {
        private Encoding encoding;
        private bool shouldThrowOnError = true;
        public DefaultJsonSerializer(bool shouldThrowOnError = true)
        {
            encoding = Encoding.UTF8;
            this.shouldThrowOnError = shouldThrowOnError;
            this.Settings = new JsonSerializerSettings()
            {
                TraceWriter = new DefaultTraceWriter(new LoggerFactory()),
            };
        }
          
        public DefaultJsonSerializer(Encoding encoding, bool shouldThrowOnError = true) : this(shouldThrowOnError)
        {
            this.encoding = encoding;
        }


        public DefaultJsonSerializer(JsonSerializerSettings settings,  bool shouldThrowOnError = true) : this(shouldThrowOnError)
        {
            this.Settings = settings;
        }

        public DefaultJsonSerializer(bool shouldThrowOnError = true, params JsonConverter[] converters) : this(shouldThrowOnError)
        {
            
            this.Settings.Converters = this.Converters;
        }


        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        public JsonSerializerSettings Settings { get; }
        public JsonConverter[] Converters { get; }

       
        public Task<T> DeSerialize<T>(ReadOnlySequence<byte> bytesSequence) where T : class
        {
            Task<T> result = Execution.Async(bytesSequence, (_bytesSequence) => 
            {
                var bytesSequement = _bytesSequence.GetArraySegment();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(encoding.GetString(bytesSequement.Array));
            });

            return result;

        }

        public Task<T> DeSerialize<T>(Stream stream) where T : class
        { 

            Task<T> resultTask = Execution.Async<Stream, T>(stream, (_stream) => 
            {
                using (var sr = new StreamReader(_stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var result = serializer.Deserialize<T>(jsonTextReader);
                    return result;
                }
                
            });

            return resultTask;


        }

        public Task Serialize<T>(Stream stream, T instance) where T : class
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
             
            try
            {

                using (var sr = new StreamWriter(stream))
                using (var jsonTextWriter = new JsonTextWriter(sr))
                {
                    serializer.Serialize(jsonTextWriter, instance, typeof(T));
                }
            }
            catch (Exception ex)
            {
                if (shouldThrowOnError)
                {
                    tcs.SetException(ex);
                }
                throw ex;
            }
            finally
            {
                tcs.SetResult(0);

            }

            return tcs.Task;
        }

        public Task<ReadOnlySequence<byte>> Serialize<T>(T instance) where T : class
        {
            Task<ReadOnlySequence<byte>> result = Execution.Async<T, ReadOnlySequence<byte>>(instance, (_instance) =>
            {

                if (_instance == null)
                {
                    return new ReadOnlySequence<byte>();
                }

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(instance, this.Settings);

                var bytes = encoding.GetBytes(json);

                return bytes.AsReadOnlySequence();
            });

            return result;
            
        }
    }
}
