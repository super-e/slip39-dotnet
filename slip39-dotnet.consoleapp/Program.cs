using System.Diagnostics;

namespace slip39_dotnet.consoleapp;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
