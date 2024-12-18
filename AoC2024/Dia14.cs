using System.Text.RegularExpressions;
using AoC2024.Classes;

namespace AoC2024;

public struct RobotCoordinates
{
    public Tuple<int, int> Position { get; init; }
    public Tuple<int, int> Velocity { get; init; }
}

public struct MapData
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int TimeElapsed { get; init; }
}

public struct Dia14Input
{
    public RobotCoordinates[] RobotData { get; init; }
    public MapData MapData { get; init; }
}

public class Dia14 : ProblemSolution<Dia14Input>
{
    public override int CurrentDay()
    {
        return 14;
    }

    private readonly Regex _coordinatesRegex = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)");
    private readonly Regex _paramsRegex = new Regex(@"(\d+) (\d+) (\d+)");

    public override Dia14Input ReadInput(string[] rawInput)
    {
        var robotData = new List<RobotCoordinates>();
        for (int i = 0; i < rawInput.Length - 2; i++)
        {
            var match = _coordinatesRegex.Match(rawInput[i]);
            robotData.Add(new RobotCoordinates
            {
                Position = new Tuple<int, int>(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value)),
                Velocity = new Tuple<int, int>(
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value))
            });
        }

        var paramsMatch = _paramsRegex.Match(rawInput[^1]);
        var mapData = new MapData
        {
            Width = int.Parse(paramsMatch.Groups[1].Value),
            Height = int.Parse(paramsMatch.Groups[2].Value),
            TimeElapsed = int.Parse(paramsMatch.Groups[3].Value)
        };
        return new Dia14Input
        {
            RobotData = robotData.ToArray(),
            MapData = mapData
        };
    }

    private RobotCoordinates RoboIterate(RobotCoordinates robotData, MapData mapData)
    {
        var position = new Tuple<long, long>(robotData.Position.Item1, robotData.Position.Item2);
        var diff = new Tuple<long, long>(
            (((long) mapData.TimeElapsed % mapData.Width) * ((long) robotData.Velocity.Item1 % mapData.Width))% mapData.Width, 
            (((long) mapData.TimeElapsed % mapData.Height) * ((long) robotData.Velocity.Item2 % mapData.Height))% mapData.Height);
        var result =  new Tuple<int, int>(
            (int) ((position.Item1 + diff.Item1) % mapData.Width),
            (int) ((position.Item2 + diff.Item2) % mapData.Height)
        );
        return new RobotCoordinates
        {
            Position = new Tuple<int, int>(
                result.Item1 < 0 ? result.Item1 + mapData.Width : result.Item1,
                result.Item2 < 0 ? result.Item2 + mapData.Height : result.Item2),
            Velocity = robotData.Velocity
        };
    }

    private long EvaluateRobots(IEnumerable<Tuple<int, int>> robotPosition, MapData mapData)
    {
        int topLeftCount = 0;
        int topRightCount = 0;
        int bottomLeftCount = 0;
        int bottomRightCount = 0;
        foreach (var position in robotPosition)
        {
            bool isRight = 2 * position.Item1 + 1 > mapData.Width;
            bool isLeft = 2 * position.Item1 + 1 < mapData.Width;
            bool isTop = 2 * position.Item2 + 1 < mapData.Height;
            bool isBottom = 2 * position.Item2 + 1 > mapData.Height;
            if (isTop && isLeft) topLeftCount++;
            if (isTop && isRight) topRightCount++;
            if (isBottom && isLeft) bottomLeftCount++;
            if (isBottom && isRight) bottomRightCount++;
        }

        return topLeftCount * topRightCount * bottomLeftCount * bottomRightCount;
    }

    public int EmptySquares(int size, RobotCoordinates[] robotData, MapData mapData)
    {
        var boxes = new int[mapData.Width / size][];
        for (int i = 0; i < mapData.Width / size; i++)
        {
            var row = new int[mapData.Height / size];
            for (int j = 0; j < mapData.Height / size; j++)
                row[j] = 0;
            boxes[i] = row;
        }

        foreach (var data in robotData)
        {
            var x = data.Position.Item1 / size;
            var y = data.Position.Item2 / size;
            if(x < mapData.Width / size && y < mapData.Height / size)
                boxes[x][y]++;
        }

        var total = 0;
        for (int i = 0; i < mapData.Width / size; i++)
        for (int j = 0; j < mapData.Height / size; j++)
            total += boxes[i][j] == 0 ? 1 : 0;

        return total;
    }

    public override object Part1(Dia14Input input)
    {
        var results = input.RobotData.Select(roboData => RoboIterate(roboData, input.MapData));
        return EvaluateRobots(results.Select(it => it.Position), input.MapData);
    }

    public override object Part2(Dia14Input input)
    {
        var positions = input.RobotData;
        string? userInput;
        var singleIteration = new MapData
        {
            Width = input.MapData.Width,
            Height = input.MapData.Height,
            TimeElapsed = 1
        };
        int iteration = 0;
        do
        {
            var map = new CharMap(input.MapData.Height, input.MapData.Width, ' ');
            foreach (var robot in positions)
                map.Set(robot.Position.Item2, robot.Position.Item1, '*');

            var entropy = EmptySquares(3, positions, singleIteration);

            if (entropy > 810 || iteration == 0)
            {
                Console.WriteLine("=========================================================================================");
                map.Print();
                Console.WriteLine();
                Console.WriteLine("ITERATION: " + iteration);
                Console.WriteLine("3x3 empty: " + entropy);
                Console.WriteLine();
                Console.Write("    > "); 
                userInput = Console.ReadLine();
                Console.Clear();
            }
            else
                userInput = "";
            
            positions = positions.Select(robotData => RoboIterate(robotData, singleIteration)).ToArray();
            iteration++;
        } while (userInput != "END");

        return 2;
    }
}