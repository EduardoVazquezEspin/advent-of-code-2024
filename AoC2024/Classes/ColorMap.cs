namespace AoC2024.Classes;

public class ColorMap<T> where T : notnull
{
    private uint _serial;
    private Dictionary<T, uint>? _nodeMap;
    private Dictionary<uint, uint>? _colorMap;
    public int TotalColors { get; private set; }

    public ColorMap()
    {
        _nodeMap = new Dictionary<T, uint>();
        _colorMap = new Dictionary<uint, uint>();
        _serial = 0;
        
        TotalColors = 0;
    }

    ~ColorMap()
    {
        _nodeMap = null;
        _colorMap = null;
    }

    public void SetColor(T node, uint color)
    {
        if (color >= _serial)
            throw new Exception("Invalid Color!");
        var previous = GetColor(node);
        if (previous == null || previous == color)
        {
            _nodeMap![node] = color;
            return;
        }

        _colorMap![(uint) previous] = color;
        TotalColors--;
        _nodeMap!.Remove(node);
        _nodeMap.Add(node, color);
    }

    public void SetColor(T node)
    {
        var color = _serial++;
        TotalColors++;
        SetColor(node, color);
    }

    public uint? GetColor(T node)
    {
        if (!_nodeMap!.TryGetValue(node, out uint color))
            return null;

        return GetColor(color, node);
    }

    private uint GetColor(uint color, T? node)
    {
        if (!_colorMap!.TryGetValue(color, out uint parent))
            return color;
        
        var colorList = new List<uint>{ color, parent };
        uint current = parent;
        while (_colorMap.TryGetValue(current, out uint next))
        {
            colorList.Add(next);
            current = next;
        }

        for (int i = 0; i < colorList.Count - 1; i++)
        {
            var item = colorList[i];
            _colorMap.Remove(item);
            _colorMap.Add(item, current);
        }

        if (node != null)
        {
            _nodeMap!.Remove(node);
            _nodeMap.Add(node, current);
        }

        return current;
    }

    public bool Compare(T node1, T node2)
    {
        var color1 = GetColor(node1);
        var color2 = GetColor(node2);
        return color1 == color2;
    }
}