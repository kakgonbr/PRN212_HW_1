using System.Runtime.CompilerServices;

namespace PRN212_HW_1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("------------------------------------- HW1 -------------------------------------");
            HW1.Run(args);

            Console.WriteLine("------------------------------------- HW2 -------------------------------------");
            HW2.Run(args);

            Console.WriteLine("------------------------------------- HW3 -------------------------------------");
            await HW3.Run(args);
        }
    }
}
