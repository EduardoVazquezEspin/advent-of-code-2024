using System.Text.RegularExpressions;
using AoC2024.Helpers;

namespace AoC2024;

public struct Dia5Input
{
    public List<Tuple<int, int>> Rules { get; init; }
    public List<List<int>> UpdateOrder { get; init; }
} 

public class Dia5: ProblemSolution<Dia5Input>
{
    public override int CurrentDay()
    {
        return 5;
    }

    private readonly Regex _ruleRegex = new Regex(@"(\d+)\|(\d+)");
    private readonly Regex _numberRegex = new Regex(@"\d+");

    public override Dia5Input ReadInput(string[] rawInput)
    {
        int index = 0;
        string line;
        var rules = new List<Tuple<int, int>> ();
        while ((line = rawInput[index++]) != "")
        {
            var match = _ruleRegex.Match(line);
            var first = Int32.Parse(match.Groups[1].Value);
            var second = Int32.Parse(match.Groups[2].Value);
            rules.Add(new Tuple<int, int>(first, second));
        }

        var updateOrder = new List<List<int>>();
        while (index++ < rawInput.Length)
        {
            line = rawInput[index-1];
            var update = new List<int>();
            var matches = _numberRegex.Matches(line);
            foreach (var obj in matches)
            {
                Match match = (obj as Match)!;
                update.Add(Int32.Parse(match.Value));
            }
            updateOrder.Add(update);
        }

        return new Dia5Input
        {
            Rules = rules,
            UpdateOrder = updateOrder
        };
    }

    private bool IsOrderCorrect(List<Tuple<int, int>> rules, List<int> order)
    {
        for (int i = 0; i < order.Count - 1; i++)
        {
            for (int j = i + 1; j < order.Count; j++)
            {
                foreach (var rule in rules)
                {
                    if (rule.Item2 == order[i] && rule.Item1 == order[j])
                        return false;
                }
            }
        }

        return true;
    }

    private List<int> GetCorrectOrder(List<Tuple<int, int>> rules, List<int> order)
    {
        Func<int, List<int>> goesTo = (int x) => order.Where(other =>
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (rule.Item1 == other && rule.Item2 == x)
                    return true;
            }
            return false;
        }).ToList();
        return GraphAlgorithms.GetTopologicalOrder(order, goesTo);
    }

    public override IFormattable Part1(Dia5Input input)
    {
        var total = 0;
        foreach (var order in input.UpdateOrder)
        {
            if (IsOrderCorrect(input.Rules, order))
                total += order[order.Count / 2];
            
        }
        return total;
    }

    public override IFormattable Part2(Dia5Input input)
    {
        var total = 0;
        foreach (var order in input.UpdateOrder)
        {
            if (!IsOrderCorrect(input.Rules, order))
            {
                var correctOrder = GetCorrectOrder(input.Rules, order);
                total += correctOrder[correctOrder.Count / 2];
            }
        }
        return total;
    }
}