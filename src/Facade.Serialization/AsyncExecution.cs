using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Facade.Serialization
{
    public class Execution
    {

        public static Task Async<TIn>(TIn argument, Action<TIn> handler)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            try
            {
                handler(argument);
            }
            catch (Exception ex)
            {
                
                    tcs.SetException(ex);
                
               
            }
            finally
            {
                tcs.SetResult(0);

            }

            return tcs.Task;
        }

        public static Task<TOut> Async<TIn, TOut>(TIn argument, Func<TIn, TOut> handler)
        {
            TaskCompletionSource<TOut> tcs = new TaskCompletionSource<TOut>();

            TOut result = default(TOut);

            try
            {
                result = handler(argument);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                 
                    tcs.SetException(ex);
              }
         

            return tcs.Task;
        }

    }
}
