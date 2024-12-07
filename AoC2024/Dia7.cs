using System.Text.RegularExpressions;

namespace AoC2024;

public struct Equation
{
    public long Result { get; init; }
    public List<long> Values { get; init; }
}

public class Dia7 : ProblemSolution<List<Equation>>
{
    public override int CurrentDay()
    {
        return 7;
    }

    private readonly Regex _numberRegex = new Regex(@"\d+");
    
    public override List<Equation> ReadInput(string[] rawInput)
    {
        return rawInput.Select(line =>
        {
            long? result = null;
            List<long> values = new List<long>();

            var matches = _numberRegex.Matches(line);
            foreach (var obj in matches)
            {
                var match = (obj as Match)!;
                if (result is null)
                    result = Int64.Parse(match.Value);
                else
                    values.Add(Int64.Parse(match.Value));
            }

            return new Equation
            {
                Result = result!.Value,
                Values = values
            };
        }).ToList();
    }

    private bool HasSolutionPart1(Equation equation, long currentValue)
    {
        if (!equation.Values.Any())
            return equation.Result == currentValue;
        var firstValue = equation.Values[0];
        var restOfValues = equation.Values.Where((_, i) => i > 0).ToList();
        
        bool hasSumSolution = HasSolutionPart1(new Equation
        {
            Result = equation.Result,
            Values = restOfValues
        }, currentValue + firstValue);
        if (hasSumSolution)
            return true;

        bool hasProdSolution = HasSolutionPart1(new Equation
        {
            Result = equation.Result,
            Values = restOfValues
        }, currentValue * firstValue);

        return hasProdSolution;
    }

    public override IFormattable Part1(List<Equation> input)
    {
        return input.Aggregate(0L, (acc, curr) =>
        {
            var allValuesExceptFirst = curr.Values.Where((_, i) => i > 0).ToList();
            return HasSolutionPart1(
                    new Equation {Result = curr.Result, Values = allValuesExceptFirst}, curr.Values[0])
                    ? acc + curr.Result
                    : acc;
        });
    }

    private long Pow10(int n)
    {
        long p = 1;
        for (int i = 0; i < n; i++)
            p *= 10L;
        return p;
    }

    private long Concat(long a, long b)
    {
        return a * Pow10(b.ToString().Length) + b;
    }
    
    private bool HasSolutionPart2(Equation equation, long currentValue)
    {
        if (!equation.Values.Any())
            return equation.Result == currentValue;
        var firstValue = equation.Values[0];
        var restOfValues = equation.Values.Where((_, i) => i > 0).ToList();
        
        bool hasSumSolution = HasSolutionPart2(new Equation
        {
            Result = equation.Result,
            Values = restOfValues
        }, currentValue + firstValue);
        if (hasSumSolution)
            return true;

        bool hasProdSolution = HasSolutionPart2(new Equation
        {
            Result = equation.Result,
            Values = restOfValues
        }, currentValue * firstValue);

        if (hasProdSolution)
            return true;

        bool hasConcatSolution = HasSolutionPart2(new Equation
        {
            Result = equation.Result,
            Values = restOfValues
        }, Concat(currentValue, firstValue));

        return hasConcatSolution;
    }

    public override IFormattable Part2(List<Equation> input)
    {
        return input.Aggregate(0L, (acc, curr) =>
        {
            var allValuesExceptFirst = curr.Values.Where((_, i) => i > 0).ToList();
            return HasSolutionPart2(
                new Equation {Result = curr.Result, Values = allValuesExceptFirst}, curr.Values[0])
                ? acc + curr.Result
                : acc;
        });
    }
}