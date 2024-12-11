using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using AoC2024.Helpers;

namespace AoC2024;

public struct Dia11Input
{
    public List<ulong> Stones { get; init; }
    public uint Iterations { get; init; }
}

public class Dia11 : ProblemSolution<Dia11Input>
{
    public override int CurrentDay()
    {
        return 11;
    }

    private Regex _digitsRegex = new Regex(@"\d+");
    
    public override Dia11Input ReadInput(string[] rawInput)
    {
        var rawStones = rawInput[0];
        var matches = _digitsRegex.Matches(rawStones);
        var stones = new List<ulong>();
        foreach (var obj in matches)
        {
            Match match = (Match) obj;
            stones.Add(ulong.Parse(match.Value));
        }

        var iterations = uint.Parse(rawInput[2]);
        return new Dia11Input
        {
            Stones = stones,
            Iterations = iterations
        };
    }
    
    private ulong[] Iterate(ulong stone)
    {
        if (stone == 0)
            return new ulong[] {1};

        uint digits = ArithmeticAlgorithms.NumberOfDigits(stone); 
        if (digits % 2 == 0)
        {
            var semi = digits / 2;
            var pow = ArithmeticAlgorithms.Power((ulong) 10, semi);
            var first = stone / pow;
            var second = stone % pow;
            return new ulong[] {first, second};
        }

        return new ulong[] {2024 * stone};
    }
    
    public override IFormattable Part1(Dia11Input input)
    {
        var stoneList = input.Stones;
        for (int i = 0; i < input.Iterations; i++)
            stoneList = stoneList.SelectMany(Iterate).ToList();
        
        return stoneList.Count;
    }

    public override IFormattable Part2(Dia11Input input)
    {
        var dictionary = new Dictionary<ulong, ulong>();
        foreach(var stone in input.Stones)
            if (dictionary.ContainsKey(stone))
                dictionary[stone] += 1;
            else
                dictionary.Add(stone,1);
        
        for (int i = 0; i < input.Iterations; i++)
        {
            var newDictionary = new Dictionary<ulong, ulong>();
            foreach (var stone in dictionary.Keys)
            {
                var result = Iterate(stone);
                foreach(var newStone in result)
                    if (newDictionary.ContainsKey(newStone))
                        newDictionary[newStone] += dictionary[stone];
                    else
                        newDictionary.Add(newStone, dictionary[stone]);
            }

            dictionary = newDictionary;
        }

        ulong total = 0;
        foreach (var value in dictionary.Values)
            total += value;
        
        return total;
    }
}