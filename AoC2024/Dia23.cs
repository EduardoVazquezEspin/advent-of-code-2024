using System.Text.RegularExpressions;
using AoC2024.Helpers;

namespace AoC2024;

public struct Dia23Input
{
    public List<string> Computers { get; init; }
    public Dictionary<string, List<string>> ConnectedTo { get; init; }
    public string StartsWith { get; init; }
}

public class Dia23 : ProblemSolution<Dia23Input>
{
    public override int CurrentDay()
    {
        return 23;
    }

    private readonly Regex _edgeRegex = new(@"([a-z]+)-([a-z]+)");

    public override Dia23Input ReadInput(string[] rawInput)
    {
        var computers = new List<string>();
        var dictionary = new Dictionary<string, List<string>>();

        for (int i = 0; i < rawInput.Length - 2; i++)
        {
            var match = _edgeRegex.Match(rawInput[i]);
            var first = match.Groups[1].Value;
            var second = match.Groups[2].Value;

            if (!computers.Contains(first))
            {
                dictionary.Add(first, new List<string>());
                computers.Add(first);
            }
                
            if (!computers.Contains(second))
            {
                dictionary.Add(second, new List<string>());
                computers.Add(second);
            }
            
            dictionary[first].Add(second);
            dictionary[second].Add(first);

        }

        return new Dia23Input
        {
            Computers = computers,
            ConnectedTo = dictionary,
            StartsWith = rawInput[^1]
        };
    }

    public override object Part1(Dia23Input input)
    {
        List<string> GoesTo(string computer) => input.ConnectedTo[computer];
        var triangles = GraphAlgorithms.FindAllTriangles(input.Computers, GoesTo);

        var startsWith = input.StartsWith;
        var trianglesWithT = triangles.Where(tuple =>
        {
            if (tuple.Item1.Substring(0, startsWith.Length) == startsWith)
                return true;
            if (tuple.Item2.Substring(0, startsWith.Length) == startsWith)
                return true;
            if (tuple.Item3.Substring(0, startsWith.Length) == startsWith)
                return true;
            return false;
        }).ToArray().Length;

        return trianglesWithT;
    }

    public override object Part2(Dia23Input input)
    {
        List<string> GoesTo(string computer) => input.ConnectedTo[computer];
        var clique = GraphAlgorithms.FindMaximumClique(input.Computers, GoesTo);
        clique.Sort();

        return string.Join(',', clique);
    }
}