using System.Text.RegularExpressions;

namespace AoC2024;

public class Dia3 : ProblemSolution<string>
{
    public override int CurrentDay()
    {
        return 3;
    }

    public override string ReadInput(string[] rawInput)
    {
        return rawInput.Aggregate("", (acc, curr) => acc + "\n" + curr);
    }

    private readonly Regex _mulRegex = new Regex(@"mul\((\d+),(\d+)\)");
    
    public override IFormattable Part1(string input)
    {
        var total = 0;
        
        foreach (var obj in _mulRegex.Matches(input))
        {
            Match match = (obj as Match)!;
            var first = Int32.Parse(match.Groups[1].Value);
            var second = Int32.Parse(match.Groups[2].Value);
            total += first * second;
        }

        return total;
    }

    private readonly Regex _doRegex = new Regex(@"do\(\)");
    private readonly Regex _dontRegex = new Regex(@"don\'t\(\)");

    enum MatchType
    {
        Mul,
        Do,
        Dont,
        None
    }

    private MatchType MatchInput(string input, out Match? match)
    {
        match = null;
        MatchType type = MatchType.None;
        var mulMatch = _mulRegex.Match(input);
        if (mulMatch.Success)
        {
            match = mulMatch;
            type = MatchType.Mul;
        }
        var doMatch = _doRegex.Match(input);
        if (doMatch.Success && (match == null || doMatch.Index < match.Index))
        {
            match = doMatch;
            type = MatchType.Do;
        }
        var dontMatch = _dontRegex.Match(input);
        if (dontMatch.Success && (match == null || dontMatch.Index < match.Index))
        {
            match = dontMatch;
            type = MatchType.Dont;
        }
        return type;
    }
    
    public override IFormattable Part2(string input)
    {
        int total = 0;
        bool isEnabled = true;
        MatchType type;
        while ((type = MatchInput(input, out Match? match)) != MatchType.None)
        {
            switch (type)
            {
                case MatchType.Mul:
                    if (isEnabled)
                    {
                        var first = Int32.Parse(match!.Groups[1].Value);
                        var second = Int32.Parse(match.Groups[2].Value);
                        total += first * second;
                    }
                    break;
                case MatchType.Do:
                    isEnabled = true;
                    break;
                case MatchType.Dont:
                    isEnabled = false;
                    break;
            }
            input = input.Substring(match!.Index + match.Length);
        }

        return total;
    }
}