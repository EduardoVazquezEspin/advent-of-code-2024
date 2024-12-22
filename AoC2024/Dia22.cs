using System.Text.RegularExpressions;

namespace AoC2024;

public struct Dia22Input
{
    public long[] SecretList { get; init; }
    public long FirstMultiplier { get; init; }
    public long Divisor { get; init; }
    public long SecondMultiplier { get; init; }
    public long Modulo { get; init; }
    public long Iterations { get; init; }
}

public class Dia22 : ProblemSolution<Dia22Input>
{
    public override int CurrentDay()
    {
        return 22;
    }

    public long NextSecret(long current, Dia22Input input)
    {
        var result = current;
        var x = result * input.FirstMultiplier;
        result ^= x;
        result %= input.Modulo;

        x = result / input.Divisor;
        result ^= x;
        result %= input.Modulo;

        x = result * input.SecondMultiplier;
        result ^= x;
        result %= input.Modulo;

        return result;
    }

    private readonly Regex _paramsRegex = new (@"(\d+) (\d+) (\d+) (\d+) (\d+)");
    
    public override Dia22Input ReadInput(string[] rawInput)
    {
        var list = rawInput
            .Where((_, index) => index < rawInput.Length - 2)
            .Select(long.Parse)
            .ToArray();

        var match = _paramsRegex.Match(rawInput[^1]);
        var firstMultiplier = long.Parse(match.Groups[1].Value);
        var divisor = long.Parse(match.Groups[2].Value);
        var secondMultiplier = long.Parse(match.Groups[3].Value);
        var modulo = long.Parse(match.Groups[4].Value);
        var iterations = long.Parse(match.Groups[5].Value);

        return new Dia22Input
        {
            SecretList = list,
            FirstMultiplier = firstMultiplier,
            SecondMultiplier = secondMultiplier,
            Divisor = divisor,
            Modulo = modulo,
            Iterations = iterations
        };
    }

    public override object Part1(Dia22Input input)
    {
        long total = 0L;
        
        foreach (var secret in input.SecretList)
        {
            var current = secret;
            for (int i = 0; i < input.Iterations; i++)
                current = NextSecret(current, input);
            total += current;
        }

        return total;
    }

    public override object Part2(Dia22Input input)
    {
        var dictionary = new Dictionary<string, long>();
        foreach (var secret in input.SecretList)
        {
            var previous = secret;
            var current = secret;
            var last4Differences = new List<long>();
            var hasBeenSeen = new HashSet<string>();
            for (int i = 0; i < input.Iterations; i++)
            {
                current = NextSecret(current, input);
                var diff = current%10 - previous%10;
                if(i >= 4)
                    last4Differences.RemoveAt(0);
                last4Differences.Add(diff);
                if (i >= 3)
                {
                    var key = last4Differences[0] 
                              + "," + last4Differences[1] 
                              + "," + last4Differences[2] 
                              + "," + last4Differences[3];
                    if (!hasBeenSeen.Contains(key))
                    {
                        hasBeenSeen.Add(key);
                        if(!dictionary.ContainsKey(key))
                            dictionary.Add(key, 0L);
                        dictionary[key] += current % 10;
                    }
                }
                
                previous = current;
            }
        }

        long max = -1;
        foreach (var price in dictionary.Values)
            if (price > max)
                max = price;
        
        return max;
    }
}