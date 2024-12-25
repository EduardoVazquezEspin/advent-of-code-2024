namespace AoC2024;

public struct Dia25Input
{
    public List<int[]> Locks { get; init; }
    public List<int[]> Keys { get; init; }
}

public class Dia25 : ProblemSolution<Dia25Input>
{
    public override int CurrentDay()
    {
        return 25;
    }

    public override Dia25Input ReadInput(string[] rawInput)
    {
        var locks = new List<int[]>();
        var keys = new List<int[]>();
        for (int i = 0; i < (rawInput.Length + 7) / 8; i++)
        {
            if (rawInput[8 * i][0] == '#')
            {
                var lockInstance = new int[5];
                for (int j = 0; j < 5; j++)
                {
                    lockInstance[j] = 0;
                    while (lockInstance[j] < 5 && rawInput[8 * i + lockInstance[j] + 1][j] == '#')
                        lockInstance[j]++;
                }
                locks.Add(lockInstance);
            }
            else
            {
                var key = new int[5];
                for (int j = 0; j < 5; j++)
                {
                    key[j] = 0;
                    while (key[j] < 5 && rawInput[8 * i + 5 - key[j]][j] == '#')
                        key[j]++;
                }
                keys.Add(key);
            }
        }

        return new Dia25Input
        {
            Locks = locks,
            Keys = keys
        };
    }

    public override object Part1(Dia25Input input)
    {
        var total = 0;
        foreach (var lockInstance in input.Locks)
        foreach (var keys in input.Keys)
        {
            var theyFit = true;
            for (int i = 0; theyFit && i < 5; i++)
                theyFit = lockInstance[i] + keys[i] <= 5;

            if (theyFit)
                total++;
        }

        return total;
    }

    public override object Part2(Dia25Input input)
    {
        return "Happy XMAS! :)";
    }
}