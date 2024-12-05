namespace AoC2024;

public abstract class ProblemSolution<TInput>
{
    public abstract int CurrentDay();

    public abstract TInput ReadInput(string[] rawInput);
    public abstract IFormattable Part1(TInput input);
    
    public abstract IFormattable Part2(TInput input);
}