namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class bxc : InstructionRunner
{
    public bxc(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    {
    }

    public override void Run()
    {
        var valueB = GetRegister('B');
        var valueC = GetRegister('C');
        GetLiteral();

        var result = valueB ^ valueC;
        SetRegister('B', result);
        
        base.Run();
    }
}