using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Facade.Serialization.Extensions
{
    public static class Extensions
    {
        public static ArraySegment<T> GetArraySegment<T>(this ReadOnlySequence<T> input)
        {
            if (input.IsSingleSegment)
            {
                var isArray = MemoryMarshal.TryGetArray(input.First, out var arraySegment);
               
                return arraySegment;
            }

            // Should be rare
            return new ArraySegment<T>(input.ToArray());
        }

        public static ReadOnlySequence<T> AsReadOnlySequence<T>(this T[] values)
        {
            return new ReadOnlySequence<T>(values);
        }

    }
}
