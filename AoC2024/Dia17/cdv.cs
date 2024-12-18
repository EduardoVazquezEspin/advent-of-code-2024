using AoC2024.Helpers;

namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class cdv : InstructionRunner
{
    public cdv(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    {
    }

    public override void Run()
    {
        var numerator = GetRegister('A');
        var denominator = ArithmeticAlgorithms.Power(2, (uint) GetCombo());
        SetRegister('C', numerator / (long) denominator);
        
        base.Run();
    }
}