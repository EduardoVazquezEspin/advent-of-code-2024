using System.Diagnostics;
using AoC2024.Classes;

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
        var queue = new List<GetTopologicalOrderQueueNode<T>>();
        nodes.ForEach(node =>        
                queue.Add( new GetTopologicalOrderQueueNode<T>
                {
                    Add = false,
                    Node = node
                })
            );

        Dictionary<T, bool> hasBeenAdded = new Dictionary<T, bool>();
        var result = new List<T>(); 
        while (queue.Any())
        {
            var top = queue[^1];
            queue.RemoveAt(queue.Count-1);
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
    
    public struct ConnectivityResult<TN> where TN : notnull
    {
        public int Connectivity { get; init; }
        public Quotient<TN> Quotient;
    }

    public static ConnectivityResult<TN> GetConnectivity<TN>(
        List<TN> nodes, 
        Func<TN, List<TN>> goesTo
    ) where TN : notnull
    {
        var quotient = new Quotient<TN>(nodes);
        
        foreach (var node in nodes)
        {
            var neighbours = goesTo(node);
            foreach (var neighbour in neighbours)
                quotient.SetEqual(node, neighbour);
        }

        return new ConnectivityResult<TN> {Connectivity = quotient.ClassCount, Quotient = quotient};
    }

    public interface IEdge<out TNode> where TNode : notnull
    {
        public TNode From { get; }
        public TNode To { get; }
    }
    
    public static List<TE> KargersMinCut<TN, TE> (List<TE> edges) 
        where TN :notnull 
        where TE : IEdge<TN>
    {
        var quotient = new Quotient<TN>();
        foreach (var edge in edges)
        {
            quotient.SetClass(edge.From);
            quotient.SetClass(edge.To);
        }

        if (quotient.ClassCount < 2)
            throw new Exception("Not Enough Nodes");

        var internalEdges = edges.Select(edge => edge).ToList();
        Random rnd = new Random();
        
        while (quotient.ClassCount > 2 && internalEdges.Any())
        {
            var index = rnd.Next(internalEdges.Count);
            var edge = internalEdges[index];
            quotient.SetEqual(edge.From, edge.To);
            internalEdges.RemoveAt(index);
        }

        if (quotient.ClassCount > 2)
            throw new Exception("Graph is not Connected with > 2 Connected Subgraphs");

        return internalEdges.Where(edge => !quotient.AreEqual(edge.From, edge.To)).ToList();
    }

    public static void DepthFirstSearch<T>(T[] startingNodes, Func<T, T[]> getNeighbours, Action<T> action) where T: notnull
    {
        var stack = new List<T>(startingNodes);
        var hasBeenVisited = new HashSet<T>();
        while (stack.Any())
        {
            var top = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            if (!hasBeenVisited.Contains(top))
            {
                hasBeenVisited.Add(top);
                action(top);
                var neighbours = getNeighbours(top);
                foreach (var neighbour in neighbours)
                    stack.Add(neighbour);
            }
        }
    }
    
    private struct BreadthFirstSearchNode<T>
    {
        public T Node { get; init; }
        public int Distance { get; init; }
    }

    public static Dictionary<T, int> BreadthFirstSearch<T>(T[] startingNodes, Func<T, T[]> getNeighbours, T? stoppingNode = default, Action<T>? action = null) where T: IEquatable<T>
    {
        var stack = new List<BreadthFirstSearchNode<T>>(
            startingNodes.Select(node => new BreadthFirstSearchNode<T>
                {
                    Node = node,
                    Distance = 0
                })
            );
        if(action != null)
            foreach (var node in startingNodes)
                action(node);
            
        var distance = new Dictionary<T, int>();
        foreach (var node in startingNodes)
            distance.Add(node, 0);
        
        while (stack.Any())
        {
            var top = stack[0];
            stack.RemoveAt(0);

            var node = top.Node;
            var dist = top.Distance;
            var neighbours = getNeighbours(node).Where(it => !distance.ContainsKey(it));
            foreach (var neighbour in neighbours)
            {
                distance.Add(neighbour, dist + 1);
                if(action != null)
                    action(node);
                if (stoppingNode != null && stoppingNode.Equals(neighbour))
                    return distance;
                stack.Add(new BreadthFirstSearchNode<T>{ Node = neighbour, Distance = dist +1 });
            }
        }

        return distance;
    }
    
    public static Dictionary<T, int> BreadthFirstSearch<T>(T startingNode, Func<T, T[]> getNeighbours, T? stoppingNode = default, Action<T>? action = null)
        where T: IEquatable<T> =>
        BreadthFirstSearch(new []{ startingNode }, getNeighbours, stoppingNode, action);
}