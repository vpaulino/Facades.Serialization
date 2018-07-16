using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Facade.Serialization
{
    public interface ISerializer
    {
        Task<T> DeSerialize<T>(ReadOnlySequence<byte> bytes) where T: class;
        Task<T> DeSerialize<T>(Stream stream) where T : class;
        Task Serialize<T>(Stream stream, T instance) where T : class;
        Task<ReadOnlySequence<byte>> Serialize<T>(T instance) where T : class;
    }
}