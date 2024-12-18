using System.Text.RegularExpressions;

namespace AoC2024;

public struct MachineStats
{
    public Tuple<uint, uint> ButtonAMovement { get; init; }
    public Tuple<uint, uint> ButtonBMovement { get; init; }
    public Tuple<ulong, ulong> PricePosition { get; init; }
    
    public uint ButtonACost { get; init; }
    public uint ButtonBCost { get; init; }
}

public struct Dia13Input
{
    public List<MachineStats> MachineStatsList { get; init; }
}

public class Dia13 : ProblemSolution<Dia13Input>
{
    public override int CurrentDay()
    {
        return 13;
    }

    private readonly Regex _buttonARegex = new Regex(@"Button A: X\+(\d+), Y\+(\d+)");
    private readonly Regex _buttonBRegex = new Regex(@"Button B: X\+(\d+), Y\+(\d+)");
    private readonly Regex _priceRegex = new Regex(@"Prize: X=(\d+), Y=(\d+)");
    
    public override Dia13Input ReadInput(string[] rawInput)
    {
        var list = new List<MachineStats>();
        var numberOfMachines = (rawInput.Length + 3) / 4;
        for (int i = 0; i < numberOfMachines; i++)
        {
            var matchButtonA = _buttonARegex.Match(rawInput[4 * i]);
            var buttonAMovementX = uint.Parse(matchButtonA.Groups[1].Value);
            var buttonAMovementY = uint.Parse(matchButtonA.Groups[2].Value);
            var matchButtonB = _buttonBRegex.Match(rawInput[4 * i + 1]);
            var buttonBMovementX = uint.Parse(matchButtonB.Groups[1].Value);
            var buttonBMovementY = uint.Parse(matchButtonB.Groups[2].Value);
            var matchPrice = _priceRegex.Match(rawInput[4 * i + 2]);
            var pricePositionX = ulong.Parse(matchPrice.Groups[1].Value);
            var pricePositionY = ulong.Parse(matchPrice.Groups[2].Value);
            list.Add(new MachineStats
            {
                ButtonAMovement = new Tuple<uint, uint>(buttonAMovementX, buttonAMovementY),
                ButtonBMovement = new Tuple<uint, uint>(buttonBMovementX, buttonBMovementY),
                PricePosition = new Tuple<ulong, ulong>(pricePositionX, pricePositionY),
                ButtonACost = 3,
                ButtonBCost = 1
            });
        }

        return new Dia13Input
        {
            MachineStatsList = list,
        };
    }

    private bool FindMachineSolution(MachineStats stats, out long solution)
    {
        var potentialSolutions = new List<long>();
        uint buttonAPresses = 0;
        long currentMissingPositionX;
        long currentMissingPositionY;
        while ((currentMissingPositionX = (long) stats.PricePosition.Item1 - (long) stats.ButtonAMovement.Item1 * buttonAPresses) >= 0  &&
               (currentMissingPositionY = (long) stats.PricePosition.Item2 -  (long) stats.ButtonAMovement.Item2 * buttonAPresses) >= 0 )
        {
            if (currentMissingPositionX % stats.ButtonBMovement.Item1 == 0
                && currentMissingPositionY % stats.ButtonBMovement.Item2 == 0
                && currentMissingPositionX / stats.ButtonBMovement.Item1 ==
                currentMissingPositionY / stats.ButtonBMovement.Item2
               )
            {
                potentialSolutions.Add(stats.ButtonACost * buttonAPresses + stats.ButtonBCost * (currentMissingPositionX / stats.ButtonBMovement.Item1));
            }
            buttonAPresses++;
        }

        solution = 0L;
        if (!potentialSolutions.Any())
            return false;

        solution = potentialSolutions.Max();
        return true;
    }

    private bool SolveRank2System(MachineStats stats, long determinant, out long solution)
    {
        var pressesTimesDeterminant = new Tuple<long, long>(
            (long) (stats.ButtonBMovement.Item2 * stats.PricePosition.Item1) - (long) (stats.ButtonBMovement.Item1 * stats.PricePosition.Item2),
            - (long) (stats.ButtonAMovement.Item2 * stats.PricePosition.Item1) + (long) (stats.ButtonAMovement.Item1 * stats.PricePosition.Item2));

        if (pressesTimesDeterminant.Item1 % determinant != 0 ||
            pressesTimesDeterminant.Item2 % determinant != 0 ||
            Math.Sign(pressesTimesDeterminant.Item1) * Math.Sign(determinant) < 0 ||
            Math.Sign(pressesTimesDeterminant.Item2) * Math.Sign(determinant) < 0)
        {
            solution = 0L;
            return false;
        }

        solution = stats.ButtonACost * (pressesTimesDeterminant.Item1 / determinant) +
                   stats.ButtonBCost * (pressesTimesDeterminant.Item2 / determinant);
        return true;
    }

    private bool SolveRank1System(MachineStats stats, out long solution)
    {
        throw new NotImplementedException();
    }

    private bool FindMachineSolutionAnalytically(MachineStats stats, out long solution)
    {
        long determinant = (long) (stats.ButtonAMovement.Item1 * stats.ButtonBMovement.Item2) -
                           (long) (stats.ButtonAMovement.Item2 * stats.ButtonBMovement.Item1);

        return determinant == 0
            ? SolveRank1System(stats, out solution)
            : SolveRank2System(stats, determinant, out solution);
    }
    
    public override object Part1(Dia13Input input)
    {
        return input.MachineStatsList
            .Aggregate(0L, (acc, machineStats) => 
                FindMachineSolutionAnalytically(machineStats, out long solution) 
                    ? acc + solution 
                    : acc);
    }

    public override object Part2(Dia13Input input)
    {
        return input.MachineStatsList
            .Aggregate(0L, (acc, machineStats) => 
                FindMachineSolutionAnalytically(new MachineStats
                {
                    ButtonAMovement = machineStats.ButtonAMovement,
                    ButtonBMovement = machineStats.ButtonBMovement,
                    ButtonACost = machineStats.ButtonACost,
                    ButtonBCost = machineStats.ButtonBCost,
                    PricePosition = new Tuple<ulong, ulong>(machineStats.PricePosition.Item1 + 10000000000000L, machineStats.PricePosition.Item2 + 10000000000000L)
                }, out long solution) 
                    ? acc + solution 
                    : acc);
    }
}