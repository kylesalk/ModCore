using System;
using System.Threading.Tasks;

namespace ModCore
{
    internal static class Program
    {
        private static async Task Main() {
            try
            {
                await new ModCore().InitializeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
