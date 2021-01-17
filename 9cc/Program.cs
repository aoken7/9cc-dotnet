using System;

namespace _9cc
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Error.WriteLine("Incorrect number of arguments");
                return 1;
            }

            var compiler = new Compiler();
            Console.WriteLine(compiler.compile(args[0]));

            return 0;
        }
    }
}
