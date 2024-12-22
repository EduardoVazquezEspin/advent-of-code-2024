using System.Numerics;
using System.Text.RegularExpressions;

namespace AoC2024.Dia21;

public struct Dia21Input
{
    public string[] NumpadInput { get; init; }
    public int KeypadIterations { get; init; }
}

public class Dia21 : ProblemSolution<Dia21Input>
{
    public override int CurrentDay()
    {
        return 21;
    }

    public override Dia21Input ReadInput(string[] rawInput)
    {
        var keypadIterations = int.Parse(rawInput[^1]);
        var numpadInput = rawInput.Where((it, index) => index < rawInput.Length - 2).ToArray();

        return new Dia21Input
        {
            NumpadInput = numpadInput,
            KeypadIterations = keypadIterations
        };
    }

    private readonly Regex _numberRegex = new (@"\d+");
    public override object Part1(Dia21Input input)
    {
        var numpadController = new NumpadController();
        var keypadController = new KeypadController();
        long total = 0L;
        
        foreach (var line in input.NumpadInput)
        {
            var numValue = long.Parse(_numberRegex.Match(line).Value);
            var robotInput = numpadController.GetInput(line);
            for(int iteration = 0; iteration<input.KeypadIterations; iteration++) 
                robotInput = robotInput.SelectMany(it => keypadController.GetInput(it)).ToArray();
            
            var minSolution = robotInput.Select(it => it.Length).Min();
            
            total += numValue * minSolution;
        }

        return total;
    }

    public override object Part2(Dia21Input input)
    {
        var numpadController = new NumpadController();
        var keypadController = new KeypadController();
        BigInteger total = BigInteger.Zero;
        
        foreach (var line in input.NumpadInput)
        {
            var numValue = BigInteger.Parse(_numberRegex.Match(line).Value);
            var robotInput = numpadController.GetGreedyInput(line);
            var dictionary = new Dictionary<string, BigInteger>();
            var split = robotInput.Split('A');
            for (int i = 0; i < split.Length - 1; i++)
            {
                var unit = split[i] + 'A';
                if(!dictionary.ContainsKey(unit))
                    dictionary.Add(unit, BigInteger.Zero);
                dictionary[unit]++;
            }
            for (int i = 0; i < input.KeypadIterations; i++)
            {
                var newDictionary = new Dictionary<string, BigInteger>();
                foreach (var unit in dictionary.Keys)
                {
                    var result = keypadController.GetGreedyInput(unit);
                    split = result.Split('A');
                    for (int j = 0; j < split.Length - 1; j++)
                    {
                        var newUnit = split[j] + 'A';
                        if(!newDictionary.ContainsKey(newUnit))
                            newDictionary.Add(newUnit, BigInteger.Zero);
                        newDictionary[newUnit] += dictionary[unit];
                    }
                }

                dictionary = newDictionary;
            }
            
            BigInteger length = BigInteger.Zero;
            foreach (var unit in dictionary.Keys)
                length += dictionary[unit] * (BigInteger) unit.Length;
            
            total += numValue * length;
        }

        return total;
    }
}