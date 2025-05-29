using Demo4Library;

namespace Demo4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            float result = Helpers.Instance.MyFunction(3.5f, 0.5f);
            Console.WriteLine($"Result: {result}");
        }
    }
}
