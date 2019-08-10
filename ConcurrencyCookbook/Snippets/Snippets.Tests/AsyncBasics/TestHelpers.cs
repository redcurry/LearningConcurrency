using System;
using System.Threading.Tasks;

namespace Snippets.Tests.AsyncBasics
{
    public class TestHelpers
    {
        // From recipe 2.1
        public static async Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            await Task.Delay(delay);
            return result;
        }
    }
}