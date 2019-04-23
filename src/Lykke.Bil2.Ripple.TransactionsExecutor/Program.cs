using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;

namespace Lykke.Bil2.Ripple.TransactionsExecutor
{
    [UsedImplicitly]
    class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            await LykkeStarter.Start<Startup>(true, 5001);
#else
            await LykkeStarter.Start<Startup>(false);
#endif
        }
    }
}
