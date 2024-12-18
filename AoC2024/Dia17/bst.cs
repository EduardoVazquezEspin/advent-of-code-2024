namespace AoC2024.Dia17;

// ReSharper disable once InconsistentNaming
public class bst : InstructionRunner
{
    public override void Run()
    {
        var combo = GetCombo() % 8;
        SetRegister('B', combo);
        
        base.Run();
    }

    public bst(Func<char, long> getRegister, Action<char, long> setRegister, Func<long> getCombo, Func<long> getLiteral, Action<long> output, Func<int> getPointer, Action<int> setPointer) : base(getRegister, setRegister, getCombo, getLiteral, output, getPointer, setPointer)
    {
    }
}