using System.Text.RegularExpressions;

namespace AoC2024;

public struct Dia19Input
{
    public List<string> AvailablePatterns { get; init; }
    public List<string> TargetPatterns { get; init; }
}

class PatternFinder
{
    private readonly List<string> _availablePatterns;
    private readonly Dictionary<string, List<List<string>>> _allPatternsDictionary;
    private readonly Dictionary<string, List<string>?> _samplePatternDictionary;
    private readonly Dictionary<string, long> _countPatternDictionary;

    public PatternFinder(List<string> availablePatterns)
    {
        _availablePatterns = availablePatterns;

        var emptySolution = new List<string>();
        _allPatternsDictionary = new Dictionary<string, List<List<string>>> {{"", new List<List<string>> { emptySolution }}};
        _samplePatternDictionary = new Dictionary<string, List<string>?> {{"", emptySolution}};
        _countPatternDictionary = new Dictionary<string, long> {{"", 1}};
    }

    public List<List<string>> FindAllPatterns(string input)
    {
        if (_allPatternsDictionary.TryGetValue(input, out var solution))
            return solution;

        var solutions = new List<List<string>>();
        foreach (var pattern in _availablePatterns)
        {
            if(pattern.Length > input.Length)
                continue;
            if(input.Substring(0, pattern.Length) != pattern)
                continue;

            var partialSolutions = FindAllPatterns(input.Substring(pattern.Length));
            foreach (var partialSolution in partialSolutions)
            {
                var newList = new List<string> {pattern};
                foreach(var part in partialSolution)
                    newList.Add(part);
                solutions.Add(newList);
            }
        }
        
        _allPatternsDictionary.Add(input, solutions);

        return solutions;
    }
    
    public List<string>? FindSamplePattern(string input)
    {
        if (_samplePatternDictionary.TryGetValue(input, out var solution))
            return solution;

        foreach (var pattern in _availablePatterns)
        {
            if(pattern.Length > input.Length)
                continue;
            if(input.Substring(0, pattern.Length) != pattern)
                continue;

            var partialSolution = FindSamplePattern(input.Substring(pattern.Length));
            if(partialSolution != null){
                var newList = new List<string> {pattern};
                foreach(var part in partialSolution)
                    newList.Add(part);
                _samplePatternDictionary.Add(input, newList);
                return newList;
            }
        }

        _samplePatternDictionary.Add(input, null);

        return null;
    }

    public long CountAllPatterns(string input)
    {
        if (_countPatternDictionary.TryGetValue(input, out var solution))
            return solution;

        long solutions = 0L;
        foreach (var pattern in _availablePatterns)
        {
            if(pattern.Length > input.Length)
                continue;
            if(input.Substring(0, pattern.Length) != pattern)
                continue;

            solutions += CountAllPatterns(input.Substring(pattern.Length));
        }
        
        _countPatternDictionary.Add(input, solutions);

        return solutions;
    }
}

public class Dia19 : ProblemSolution<Dia19Input>
{
    public override int CurrentDay()
    {
        return 19;
    }

    private readonly Regex _patternRegex = new Regex(@"[wubrg]+");
    
    public override Dia19Input ReadInput(string[] rawInput)
    {
        var availablePatterns = new List<string>();
        var matches = _patternRegex.Matches(rawInput[0]);
        foreach (var obj in matches)
        {
            var match = (obj as Match)!;
            availablePatterns.Add(match.Value);
        }

        var targetPatterns = new List<string>();
        for(int i = 2; i<rawInput.Length; i++)
            targetPatterns.Add(rawInput[i]);

        return new Dia19Input
        {
            AvailablePatterns = availablePatterns,
            TargetPatterns = targetPatterns
        };
    }

    public override object Part1(Dia19Input input)
    {
        var solver = new PatternFinder(input.AvailablePatterns);
        
        var total = 0;
        foreach (var target in input.TargetPatterns)
            total += solver.FindSamplePattern(target) != null ? 1 : 0;

        return total;
    }

    public override object Part2(Dia19Input input)
    {
        var solver = new PatternFinder(input.AvailablePatterns);
        
        long total = 0;
        foreach (var target in input.TargetPatterns)
            total += solver.CountAllPatterns(target);

        return total;
    }
}