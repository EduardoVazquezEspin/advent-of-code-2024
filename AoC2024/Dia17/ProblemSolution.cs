using System.Diagnostics;
using System.Text.RegularExpressions;
using AoC2024.Helpers;

namespace AoC2024.Dia17;

public struct Dia17Input
{
    public long RegisterA { get; init; }
    public long RegisterB { get; init; }
    public long RegisterC { get; init; }
    
    public int[] Program { get; init; }
}

public class Dia17 : ProblemSolution<Dia17Input>
{
    public override int CurrentDay()
    {
        return 17;
    }

    private long ReadRegister(char register, string line)
    {
        var regex = new Regex(@"Register " + register + @": (\d+)");
        return long.Parse(regex.Match(line).Groups[1].Value);
    }

    private readonly Regex _numberRegex = new Regex(@"\d+");
    
    public override Dia17Input ReadInput(string[] rawInput)
    {
        var registerA = ReadRegister('A', rawInput[0]);
        var registerB = ReadRegister('B', rawInput[1]);
        var registerC = ReadRegister('C', rawInput[2]);
        
        var matches = _numberRegex.Matches(rawInput[4]);
        var program = new List<int>();
        foreach (var obj in matches)
        {
            Match match = (obj as Match)!;
            program.Add(int.Parse(match.Value));
        }

        return new Dia17Input
        {
            RegisterA = registerA,
            RegisterB = registerB,
            RegisterC = registerC,
            Program = program.ToArray()
        };
    }

    public override object Part1(Dia17Input input)
    {
        var computer = new Computer(input.RegisterA, input.RegisterB, input.RegisterC);
        var output = computer.Run(input.Program);
        return string.Join(',', output.Select(it => it.ToString()));
    }

    private bool AreEqual(int[] instructions, List<long> output)
    {
        if (instructions.Length != output.Count)
            return false;
        for(int i =0; i< instructions.Length; i++)
            if (instructions[i] != output[i])
                return false;
        return true;
    }

    public object Part2BruteForce(Dia17Input input)
    {
        var output = new List<long>();
        for (int iteration = 0;; iteration++)
        {
            var computer = new Computer(iteration, input.RegisterB, input.RegisterC);
            bool hasError = false;
            try
            {
                output = computer.Run(input.Program, input.Program.Length, 500);
            }
            catch (Exception _)
            {
                hasError = true;
            }

            if (!hasError && AreEqual(input.Program, output))
                return iteration;
        }
    }

    public override object Part2(Dia17Input input)
    {
        // This whole code is tailor suited to my input instructions
        var solutions = new List<ulong> {0L};
        for (int i = input.Program.Length - 1; i >= 0; i--)
        {
            var newSolutions = new List<ulong>();
            foreach (var solution in solutions)
            {
                for (int digit = 0; digit < 8; digit++)
                {
                    ulong x = 8 * solution + (ulong) digit;
                    if (
                        (((x % 8) ^ 6) ^ (x / ArithmeticAlgorithms.Power(2, (uint)((x % 8) ^ 3))))%8 == (ulong) input.Program[i]
                    )
                    {
                        newSolutions.Add(x);
                    }
                }
            }

            if (!newSolutions.Any())
                throw new Exception("Weird Error");

            solutions = newSolutions;
        }

        return string.Join("\n", solutions) ;
    }
}