using Topshelf;

namespace Homero.Client
{
    public class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.New(x =>
            {
                x.Service<Heart>();
            }).Run();
        }
    }
}