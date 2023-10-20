using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private static int i = 0;
        static ConcurrentDictionary<int, object> _syncLock = new();
        private readonly static ConcurrentDictionary<int, SemaphoreSlim> _asyncLock = new();

        public TestController()
        {
        }

        [HttpGet]
        public int Synchonorus(int key)
        {
            lock (_syncLock.GetOrAdd(key, new object()))
            {
                Console.WriteLine($"Start {key}");
                var a = i;
                Thread.Sleep(5000);

                Console.WriteLine($"Done {key}");
                i++;
                return a;

            }
        }

        [HttpGet]
        public async Task<int> Asynchonorus(int key)
        {
            SemaphoreSlim asyncLock = _asyncLock.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            try
            {
                await asyncLock.WaitAsync();
                Console.WriteLine($"Start {key}");
                var a = i;
                await Task.Delay(5000);

                Console.WriteLine($"Done {key}");
                i++;
                return a;
            }
            finally
            {
                asyncLock.Release();
            }
        }

    }
}