using System.Threading.Tasks;

namespace ModCore
{
    internal static class Program
    {
        private static Task Main(string[] args) => (ModCore = new MainCore()).InitializeAsync(args);

        public static MainCore ModCore { get; private set; }
    }
}
