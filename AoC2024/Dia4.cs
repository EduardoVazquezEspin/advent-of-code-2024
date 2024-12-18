namespace AoC2024;

public class Dia4 : ProblemSolution<string[]>
{
    public override int CurrentDay()
    {
        return 4;
    }

    public override string[] ReadInput(string[] rawInput)
    {
        return rawInput;
    }

    private bool IsInBounds(string[] map, int x, int y)
    {
        if (x < 0)
            return false;
        if (y < 0)
            return false;
        if (x >= map.Length)
            return false;
        if (y >= map[x].Length)
            return false;
        return true;
    }

    private readonly int[][] _directions =
    {
        new[] {1, 0},
        new[] {1, 1},
        new[] {0, 1},
        new[] {-1, 1},
        new[] {-1, 0},
        new[] {-1, -1},
        new[] {0, -1},
        new[] {1, -1},
    };
    
    // ReSharper disable once InconsistentNaming
    private readonly string XMAS = "XMAS";
    
    // ReSharper disable once InconsistentNaming
    private int FindXMAS(string[] map, int x, int y)
    {
        if (map[x][y] != 'X')
            return 0;

        return _directions.Aggregate(0, (acc, direction) =>
        {
            for (int i = 1; i < XMAS.Length; i++)
            {
                int posX = x + i * direction[0];
                int posY = y + i * direction[1];
                if (!IsInBounds(map, posX, posY))
                    return acc;
                if (map[posX][posY] != XMAS[i])
                    return acc;
            }
            return acc + 1;
        });
    }

    public override object Part1(string[] input)
    {
        var total = 0;
        for(int x = 0; x<input.Length; x++)
        for (int y = 0; y < input[x].Length; y++)
            total += FindXMAS(input, x, y);
        return total;
    }

    private bool FindX_MAS(string[] map, int x, int y)
    {
        if (map[x][y] != 'A')
            return false;

        int masTotal = 0;
        if (map[x - 1][y - 1] == 'M' && map[x + 1][y + 1] == 'S') masTotal++;
        if (map[x - 1][y - 1] == 'S' && map[x + 1][y + 1] == 'M') masTotal++;
        if (map[x - 1][y + 1] == 'M' && map[x + 1][y - 1] == 'S') masTotal++;
        if (map[x - 1][y + 1] == 'S' && map[x + 1][y - 1] == 'M') masTotal++;

        return masTotal == 2;
    }

    public override object Part2(string[] input)
    {
        var total = 0;
        for(int x = 1; x<input.Length - 1; x++)
        for (int y = 1; y < input[x].Length - 1; y++)
            total += FindX_MAS(input, x, y) ? 1 : 0;
        return total;
    }
}