namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class jnz : InstructionRunner
{
    public jnz(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    {
    }
    
    public override void Run()
    {
        var value = GetRegister('A');
        if (value == 0)
        {
            base.Run();
            return;
        }

        var literal = GetLiteral();
        SetPointer((int) literal);
    }
}