namespace AoC2024.Classes;

public class CharMap<T> where T : new()
{
    private readonly T[][] _map;
    
    public CharMap(string[] input, Func<char, int, int, T> mapper)
    {
        _map = new T[input.Length][];
        for (int i = 0; i < input.Length; i++)
        {
            var originalRow = input[i];
            var row = new T[originalRow.Length];
            for (int j = 0; j < originalRow.Length; j++)
                row[j] = mapper(originalRow[j], i, j);
            _map[i] = row;
        }
    }

    public CharMap(string[] input, Func<char, Tuple<int, int>, T> mapper) : this(input, (c, i, j) => mapper(c, new Tuple<int, int>(i, j))) { }
    public CharMap(string[] input, Func<char, T> mapper) : this(input, (c, _, _) => mapper(c)) { }
    
    public CharMap(int height, int width, Func<int, int, T> mapper)
    {
        _map = new T[height][];
        for (int i = 0; i < height; i++)
        {
            var row = new T[width];
            for (int j = 0; j < width; j++)
                row[j] = mapper(i, j);
            _map[i] = row;
        }
    }

    public CharMap(int height, int width, Func<T> mapper) : this(height, width, (_, _) => mapper()) { }

    public bool IsInBounds(int i, int j, out T value)
    {
        value = new T();
        if (i < 0)
            return false;
        if (j < 0)
            return false;
        if (i >= _map.Length)
            return false;
        if (j >= _map[i].Length)
            return false;
        value = _map[i][j];
        return true;
    }

    public bool IsInBounds(int i, int j) => IsInBounds(i, j, out T _);
    public bool IsInBounds(Tuple<int, int> position, out T value) => IsInBounds(position.Item1, position.Item2, out value);
    public bool IsInBounds(Tuple<int, int> position) => IsInBounds(position.Item1, position.Item2, out T _);

    public T Get(int i, int j) => _map[i][j];
    public T Get(Tuple<int, int> position) => _map[position.Item1][position.Item2];
    
    public void Set(int i, int j, T value) => _map[i][j] = value;
    public void Set(Tuple<int, int> position, T value) => _map[position.Item1][position.Item2] = value;

    public List<TR> MapAllCells<TR>(Func<T, int, int, TR> mapper)
    {
        var result = new List<TR>();
        for(int i =0; i<_map.Length; i++) for(int j= 0; j<_map[i].Length; j++)
            result.Add(mapper(_map[i][j], i, j));
        return result;
    }
    
    public List<TR> MapAllCells<TR>(Func<T, Tuple<int, int>, TR> mapper)
    {
        var result = new List<TR>();
        for(int i =0; i<_map.Length; i++) for(int j= 0; j<_map[i].Length; j++)
            result.Add(mapper(_map[i][j], new Tuple<int, int>(i, j)));
        return result;
    }
    
    public List<TR> MapAllCells<TR>(Func<T, TR> mapper)
    {
        var result = new List<TR>();
        foreach (var row in _map)
            foreach (var cell in row)
                result.Add(mapper(cell));

        return result;
    }

    public void MapAllCells(Action<T, int, int> action)
    {
        for(int i =0; i<_map.Length; i++) for(int j= 0; j<_map[i].Length; j++)
            action(_map[i][j], i, j);
    }
    
    public void MapAllCells(Action<T, Tuple<int, int>> action)
    {
        for(int i =0; i<_map.Length; i++) for(int j= 0; j<_map[i].Length; j++)
            action(_map[i][j], new Tuple<int, int>(i, j));
    }
    
    public void MapAllCells(Action<T> action)
    {
        foreach (var row in _map)
            foreach (var cell in row)
                action(cell);
    }

    public void Print(Func<T, int, int, object> show)
    {
        for(int i = 0; i< _map.Length; i++)
        {
            var row = _map[i];
            for(int j = 0; j<row.Length; j++)
                Console.Write(show(row[j], i, j).ToString());
            
            Console.WriteLine();
        }
    }

    public void Print(Func<T, object> show) => Print((cell, _, j) => show(cell));

    public TResult[][] Copy<TResult>(Func<T, int, int, TResult> mapper)
    {
        var result = new TResult[_map.Length][];
        for (int i = 0; i < _map.Length; i++)
        {
            var originalRow = _map[i];
            var row = new TResult[originalRow.Length];
            for (int j = 0; j < originalRow.Length; j++)
                row[j] = mapper(originalRow[j], i, j);
            result[i] = row;
        }

        return result;
    }
    public TResult[][] Copy<TResult>(Func<T, TResult> mapper) => Copy(((cell, _, _) => mapper(cell)));
    public TResult[][] Copy<TResult>(Func<TResult> mapper) => Copy(((_, _, _) => mapper()));
}

public class CharMap : CharMap<char>
{
    public CharMap(string[] input) : base(input, c => c) { }
    
    public CharMap(int height, int width, char value = '.') : base(height, width, () => value) { }

    public void Print()
    {
        Print(c => c);
    }
}