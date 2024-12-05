namespace AoC2024.Helpers;

public static class GraphAlgorithms
{
    struct GetTopologicalOrderQueueNode<T>
    {
        public bool Add { get; init; }
        public T Node { get; init; }
    }
    public static List<T> GetTopologicalOrder<T>(List<T> nodes, Func<T, List<T>> goesTo) where T : notnull
    {
        List<GetTopologicalOrderQueueNode<T>> queue = new List<GetTopologicalOrderQueueNode<T>>();
        nodes.ForEach(node =>        
            queue.Add( new GetTopologicalOrderQueueNode<T>
            {
                Add = false,
                Node = node
            })
            );   

        GetTopologicalOrderQueueNode<T> top;
        Dictionary<T, bool> hasBeenAdded = new Dictionary<T, bool>();
        var result = new List<T>(); 
        while (queue.Any())
        {
            top = queue[^1];
            queue.RemoveAt(queue.Count-1);
            var xivato = top.Node;
            var isAdd = top.Add;
            if (top.Add && !hasBeenAdded[top.Node])
            {
                hasBeenAdded.Remove(top.Node);
                hasBeenAdded.Add(top.Node, true);
                result.Add(top.Node);
            }
            else if(!hasBeenAdded.ContainsKey(top.Node))
            {
                hasBeenAdded.Add(top.Node, false);
                var children = goesTo(top.Node);
                queue.Add(new GetTopologicalOrderQueueNode<T>
                {
                    Add = true,
                    Node = top.Node
                });
                children.ForEach(child =>
                {
                    queue.Add(new GetTopologicalOrderQueueNode<T>
                    {
                        Add = false,
                        Node = child
                    });
                });
            }
        }
        return result;
    }
}