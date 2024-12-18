namespace AoC2024;

public abstract class ProblemSolution<TInput>
{
    public abstract int CurrentDay();

    public abstract TInput ReadInput(string[] rawInput);
    public abstract object Part1(TInput input);
    
    public abstract object Part2(TInput input);
}