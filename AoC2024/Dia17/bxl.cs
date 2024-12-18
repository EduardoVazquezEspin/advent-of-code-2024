using System.Numerics;

namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class bxl : InstructionRunner
{

    public override void Run()
    {
        var value = GetRegister('B');
        var literal = GetLiteral();
        var result = value ^ literal;
        SetRegister('B', result);
        
        base.Run();
    }

    public bxl(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    {
    }
}