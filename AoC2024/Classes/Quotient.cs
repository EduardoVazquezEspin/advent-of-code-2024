namespace AoC2024.Classes;

internal class NodeClass
{
    public NodeClass? Parent { get; set; }
}

public class Quotient<T> where T : notnull
{
    private Dictionary<T, NodeClass>? _nodeMap;
    public int ClassCount { get; private set; }

    public Quotient()
    {
        _nodeMap = new Dictionary<T, NodeClass>();
        
        ClassCount = 0;
    }

    public Quotient(List<T> nodes) : this()
    {
        foreach (var node in nodes)
            if(GetClass(node) == null)
                SetClass(node);
    }
    
    public Quotient(params T[] nodes) : this()
    {
        foreach (var node in nodes)
            if(GetClass(node) == null)
                SetClass(node);
    }

    ~Quotient()
    {
        _nodeMap = null;
    }

    private void SetClass(T node, NodeClass color)
    {
        var previous = GetClass(node);
        if (previous == null || previous == color)
        {
            _nodeMap![node] = color;
            return;
        }

        previous.Parent = color;
        ClassCount--;
        _nodeMap!.Remove(node);
        _nodeMap.Add(node, color);
    }

    public void SetClass(T node)
    {
        var color = GetClass(node);
        if (color != null)
            return;
        color = new NodeClass();
        ClassCount++;
        SetClass(node, color);
    }
    
    public void SetClass(params T[] nodes)
    {
        if (nodes.Length < 1)
            return ;
        SetClass(nodes[0]);
        var color = GetClass(nodes[0]);
        for (int i = 1; i < nodes.Length; i++)
            SetClass(nodes[i], color!);
    }

    public void SetEqual(T node1, T node2)
    {
        NodeClass? color = GetClass(node1) ?? GetClass(node2);
        if (color == null)
        {
            SetClass(node1);
            color = GetClass(node1);
        }
        SetClass(node2, color!);
    }

    private NodeClass? GetClass(T node)
    {
        if (!_nodeMap!.TryGetValue(node, out NodeClass? color))
            return null;

        return GetClass(color, node);
    }

    private NodeClass GetClass(NodeClass color, T? node)
    {
        var parent = color.Parent;
        if (parent == null)
            return color;
        
        var colorList = new List<NodeClass>{ color, parent };
        NodeClass current = parent;
        NodeClass? next;
        while ((next = current.Parent) != null)
        {
            colorList.Add(next);
            current = next;
        }

        for (int i = 0; i < colorList.Count - 1; i++)
            colorList[i].Parent = current;
        

        if (node != null)
        {
            _nodeMap!.Remove(node);
            _nodeMap.Add(node, current);
        }

        return current;
    }

    public bool AreEqual(T node1, T node2)
    {
        var color1 = GetClass(node1);
        var color2 = GetClass(node2);
        if (color1 == null && color2 == null)
            return node1.Equals(node2);
        if (color1 == null || color2 == null)
            return false;
        return color1 == color2;
    }
}