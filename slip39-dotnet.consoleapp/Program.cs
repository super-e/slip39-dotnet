///    Copyright (C) 2022  Super-E-  

using slip39_dotnet.models;
using System.Diagnostics;
using System.Security.Cryptography;

namespace slip39_dotnet.consoleapp;

 
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
class Program
{
    static void Main(string[] args)
    {
        FiniteFieldElement a = 1;
        FiniteFieldElement b = 2;
        FiniteFieldElement c = 7;
        FiniteFieldElement fa = 56;
        FiniteFieldElement fb = 245;
        FiniteFieldElement fc = 111;
        FiniteFieldElement x = 255;

        var d0a = (x - b) / (a - b) * (x - c) / (a - c);
        var d0b = (x - a) / (b - a) * (x - c) / (b - c);
        var d0c = (x - a) / (c - a) * (x - b) / (c - b);

        var f0 = fa * d0a + fb * d0b + fc * d0c;

        var N = 8;
        var t = 3;

        byte S = 0xa3;
        List<(FiniteFieldElement x, FiniteFieldElement y)> startingValues = new();
        var rnd = RandomNumberGenerator.Create();

        byte[] tempByte = new byte[1];
        for (byte i = 0; i < t - 2; i++)
        {
            rnd.GetBytes(tempByte);
            startingValues.Add((i, tempByte[0]));
        }

        startingValues.Add((254, 0));
        startingValues.Add((255, S));

        List<(FiniteFieldElement x, FiniteFieldElement y)> calculatedShares = new();

        for (byte j = (byte)(t - 2); j < N; j++)
        {
            byte result = 0;
            foreach (var element in startingValues)
            {
                byte parameter = 1;
                foreach (var otherElement in startingValues)
                {
                    if (element.x == otherElement.x) continue;
                    parameter *= (j - otherElement.x) / (element.x - otherElement.x);
                }
                result += element.y * parameter;
            }
            calculatedShares.Add((j, result));
        }
        startingValues.Remove((255, S));
        startingValues.AddRange(calculatedShares);


        foreach (var share in startingValues)
        {
            Console.WriteLine($"({(byte)share.x}, {(byte)share.y})");
        }
        Console.WriteLine($"Secret is {(byte)f0}");
        Console.ReadLine();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
