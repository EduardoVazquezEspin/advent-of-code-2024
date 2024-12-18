namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class ovt : InstructionRunner 
{
    public ovt(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    { }

    public override void Run()
    {
        var combo = GetCombo() % 8;
        Output(combo);
        
        base.Run();
    }
}