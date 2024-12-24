using System.Text.RegularExpressions;

namespace AoC2024;

public enum GateOperation
{
    And,
    Or,
    Xor
}

public struct GateConnection
{
    public Tuple<string, string> Dependencies { get; init; }
    public GateOperation Operation { get; init; }
    public string Result { get; set; }
}

public struct Dia24Input
{
    public readonly Dictionary<string, bool> InitialState {get; init;}
    public readonly List<GateConnection> GateConnections { get; init; }
}

public class Dia24 : ProblemSolution<Dia24Input>
{
    public override int CurrentDay()
    {
        return 24;
    }
    
    private readonly Regex _wireRegex = new(@"([a-z0-9]{3})\: ([01])");
    private readonly Regex _gateRegex = new(@"([a-z0-9]{3}) (AND|OR|XOR) ([a-z0-9]{3}) -> ([a-z0-9]{3})");
    
    public override Dia24Input ReadInput(string[] rawInput)
    {
        var index = 0;
        string line;
        Dictionary<string, bool> initialState = new Dictionary<string, bool>();
        while ((line = rawInput[index++]) != "")
        {
            var match = _wireRegex.Match(line);
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value == "1";
            initialState.Add(key, value);
        }

        var connections = new List<GateConnection>();
        while (index < rawInput.Length)
        {
            line = rawInput[index++];
            var match = _gateRegex.Match(line);
            var firstWire = match.Groups[1].Value;
            var secondWire = match.Groups[3].Value;
            var resultingWire = match.Groups[4].Value;
            var operationStr = match.Groups[2].Value;
            GateOperation operation = GateOperation.Xor;
            switch (operationStr)
            {
                case "AND":
                    operation = GateOperation.And;
                    break;
                case "OR":
                    operation = GateOperation.Or;
                    break;
            }
            connections.Add(new GateConnection
            {
                Dependencies = new Tuple<string, string>(firstWire, secondWire),
                Operation = operation,
                Result = resultingWire
            });
        }

        return new Dia24Input
        {
            GateConnections = connections,
            InitialState = initialState
        };
    }

    private bool Operate(bool first, bool second, GateOperation operation)
    {
        switch (operation)
        {
            case GateOperation.And:
                return first && second;
            case GateOperation.Or:
                return first || second;
            case GateOperation.Xor:
                return first ^ second;
        }

        throw new Exception("Invalid Operation " + operation);
    }

    private long ReadFromDictionary(char c, Dictionary<string, bool> dictionary)
    {
        var zWires = new List<Tuple<string, bool>>();
        foreach (var (key,value) in dictionary)
            if (key[0] == c)
                zWires.Add(new Tuple<string, bool>(key, value));

        var sorted = zWires.OrderBy(it => it.Item1).ToList();

        long total = 0;
        long p = 1;
        foreach (var tuple in sorted)
        {
            total += p * (tuple.Item2 ? 1 : 0);
            p *= 2;
        }

        return total;
    }
    
    private void WriteToDictionary(char c, long number, Dictionary<string, bool> dictionary)
    {
        string numberStr = Convert.ToString(number, 2);
        
        var zWires = new List<Tuple<string, bool>>();
        foreach (var (key,value) in dictionary)
            if (key[0] == c)
                zWires.Add(new Tuple<string, bool>(key, value));

        var sorted = zWires.OrderBy(it => it.Item1).ToList();

        for (int i = 0; i < sorted.Count && i < numberStr.Length; i++)
        {
            var digit = numberStr[numberStr.Length - i - 1];
            sorted[i] = new Tuple<string, bool>(sorted[i].Item1, digit == '1');
        }

        for (int i = numberStr.Length; i < sorted.Count; i++)
            sorted[i] = new Tuple<string, bool>(sorted[i].Item1, false);

        foreach (var (key, value) in sorted)
            dictionary[key] = value;
    }

    public override object Part1(Dia24Input input)
    {
        var pendingConnections = new List<GateConnection>();
        foreach(var connection in input.GateConnections)
            pendingConnections.Add(connection);
        var state = new Dictionary<string, bool>();
        foreach(var tuple in input.InitialState)
            state.Add(tuple.Key, tuple.Value);
        int pendingZConnectionsCount = 1;
        while (pendingZConnectionsCount > 0)
        {
            pendingZConnectionsCount = 0;
            for (int i = 0; i < pendingConnections.Count; i++)
            {
                var connection = pendingConnections[i];
                if (
                    state.TryGetValue(connection.Dependencies.Item1, out var value1) 
                    && state.TryGetValue(connection.Dependencies.Item2, out var value2)
                )
                {
                    var result = Operate(value1, value2, connection.Operation);
                    state.Add(connection.Result, result);
                    pendingConnections.RemoveAt(i--);
                }
                else if (connection.Result[0] == 'z')
                    pendingZConnectionsCount++;
            }
        }

        return ReadFromDictionary('z', state);
    }

    private List<GateConnection> _connections = new List<GateConnection>();
    private Dictionary<string, bool> _state = new Dictionary<string, bool>();

    private void Swap(string result1, string result2, List<GateConnection> connections)
    {
        var index1 = connections.FindIndex(it => it.Result == result1);
        var index2 = connections.FindIndex(it => it.Result == result2);

        var connection1 = connections[index1];
        var connection2 = connections[index2];

        connections[index1] = new GateConnection
        {
            Dependencies = connection1.Dependencies,
            Operation = connection1.Operation,
            Result = result2
        };

        connections[index2] = new GateConnection
        {
            Dependencies = connection2.Dependencies,
            Operation = connection2.Operation,
            Result = result1
        };
    }

    private bool ConnectionFilter(GateConnection gate, Tuple<string, string> dependencies, GateOperation[] operations)
    {
        if (!operations.Contains(gate.Operation))
            return false;
        if (dependencies.Item1 == gate.Dependencies.Item1 && dependencies.Item2 == gate.Dependencies.Item2)
            return true;
        if (dependencies.Item1 == gate.Dependencies.Item2 && dependencies.Item2 == gate.Dependencies.Item1)
            return true;
        return false;
    }

    private bool ConnectionFilter(GateConnection gate, Tuple<string, string> dependencies, GateOperation operation) =>
        ConnectionFilter(gate, dependencies, new[] {operation});
    
    private bool ConnectionFilter(GateConnection gate, string dependency1, string dependency2, GateOperation[] operations) =>
        ConnectionFilter(gate, new Tuple<string, string>(dependency1, dependency2), operations);
    
    private bool ConnectionFilter(GateConnection gate, string dependency1, string dependency2, GateOperation operation) =>
        ConnectionFilter(gate, new Tuple<string, string>(dependency1, dependency2), new[] {operation});

    private string CreateKey(char c, int index) => c.ToString() + (index < 10 ? "0" : "") + index;
    
    public override object Part2(Dia24Input input)
    {
        Swap("hmk", "z16", input.GateConnections);
        Swap("fhp", "z20", input.GateConnections);
        Swap("tpc", "rvf", input.GateConnections);
        Swap("fcd", "z33", input.GateConnections);
        
        var keyDictionary = new Dictionary<string, string>();
        var gate0 = input.GateConnections.Find(it => ConnectionFilter(it, "x00", "y00", GateOperation.And));
        keyDictionary.Add(CreateKey('p', 1), gate0.Result);
        for (int i = 1; i < 44; i++)
        {
            var pkey = keyDictionary[CreateKey('p', i)];
            var xKey = CreateKey('x', i);
            var yKey = CreateKey('y', i);
            var resGate = input.GateConnections.Find(it => ConnectionFilter(it, xKey, yKey, GateOperation.Xor));
            var rKey = resGate.Result;
            var interGate = input.GateConnections.Find(it => ConnectionFilter(it, xKey, yKey, GateOperation.And));
            var iKey = interGate.Result;
            var jGate = input.GateConnections.Find(it => ConnectionFilter(it, pkey, rKey, GateOperation.And));
            var jKey = jGate.Result;
            var zGate = input.GateConnections.Find(it => ConnectionFilter(it, pkey, rKey, GateOperation.Xor));
            var zKey = zGate.Result;
            if (zKey != CreateKey('z', i))
                throw new Exception("Invalid zKey for Index " + i);

            var newPGate = input.GateConnections.Find(it =>
                ConnectionFilter(it, iKey, jKey, new GateOperation[] {GateOperation.Or, GateOperation.Xor}));
            keyDictionary.Add(CreateKey('p', i + 1), newPGate.Result);
        }

        return "OK";
    }
    
    public object SetupState(Dia24Input input)
    {
        _connections = input.GateConnections;
        _state = input.InitialState;
        
        
        Swap("hmk", "z16", input.GateConnections);
        Swap("fhp", "z20", input.GateConnections);
        Swap("tpc", "rvf", input.GateConnections);
        Swap("fcd", "z33", input.GateConnections);
        
        return "";
    }

    public long Sum(long first, long second)
    {
        WriteToDictionary('x', first, _state);
        WriteToDictionary('y', second, _state);

        var result = Part1(new Dia24Input {GateConnections = _connections, InitialState = _state});
        
        return long.Parse(result.ToString() ?? string.Empty);
    }
}