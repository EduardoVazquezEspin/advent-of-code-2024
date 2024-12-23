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

    public static Dictionary<T, int> BreadthFirstSearch<T>(
        T[] startingNodes, 
        Func<T, T[]> getNeighbours, 
        out Dictionary<T, T> comesFrom,
        Predicate<T>? stop = null,
        Action<T>? action = null
    ) where T: notnull
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
        comesFrom = new Dictionary<T, T>();
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
                comesFrom.Add(neighbour, node);
                if(action != null)
                    action(node);
                if (stop != null && stop(neighbour))
                    return distance;
                stack.Add(new BreadthFirstSearchNode<T>{ Node = neighbour, Distance = dist +1 });
            }
        }

        return distance;
    }
    
    public static Dictionary<T, int> BreadthFirstSearch<T>(T startingNode, Func<T, T[]> getNeighbours, out Dictionary<T, T> comesFrom, Predicate<T>? stop = null, Action<T>? action = null)
        where T: notnull =>
        BreadthFirstSearch(new []{ startingNode }, getNeighbours, out comesFrom, stop, action);
    
    public static Dictionary<T, int> BreadthFirstSearch<T>(T[] startingNodes, Func<T, T[]> getNeighbours, Predicate<T>? stop = null, Action<T>? action = null)
        where T: notnull =>
        BreadthFirstSearch(startingNodes, getNeighbours, out _, stop, action);
    
    public static Dictionary<T, int> BreadthFirstSearch<T>(T startingNode, Func<T, T[]> getNeighbours, Predicate<T>? stop = null, Action<T>? action = null)
        where T: notnull =>
        BreadthFirstSearch(new []{ startingNode }, getNeighbours, out _, stop, action);

    private struct DijkstraNode<T>
    {
        public T Node { get; init; }
        public T? Parent { get; init; }
        public bool IsRoot { get; init; }
    }
    
    public static Dictionary<T, int> Dijkstra<T>(
        T[] startingNodes, 
        Func<T, Tuple<T, int>[]> getNeighbours,
        out Dictionary<T, T> comesFrom,
        Predicate<T>? stop = null,
        Action<T>? action = null
    ) where T: notnull
    {
        var priorityQueue = new PriorityQueue<DijkstraNode<T>, int>();
        foreach(var start in startingNodes)
            priorityQueue.Enqueue(new DijkstraNode<T>{ Node = start, IsRoot = true}, 0);

        if(action != null)
            foreach (var node in startingNodes)
                action(node);
            
        var distance = new Dictionary<T, int>();
        comesFrom = new Dictionary<T, T>();
        
        while (priorityQueue.TryDequeue(out var top, out int priority))
        {
            if(distance.ContainsKey(top.Node))
                continue;
            distance.Add(top.Node, priority);
            if(!top.IsRoot)
                comesFrom.Add(top.Node, top.Parent!);
            if(action != null)
                action(top.Node);
            if (stop != null && stop(top.Node))
                return distance;
            var neighbours = getNeighbours(top.Node).Where(neighbour => !distance.ContainsKey(neighbour.Item1));
            foreach (var (neighbour, dist) in neighbours)
                priorityQueue.Enqueue(new DijkstraNode<T>{Node =neighbour, Parent = top.Node, IsRoot = false}, priority + dist);
            
        }

        return distance;
    }

    public static Dictionary<T, int> Dijkstra<T>(
        T startingNode,
        Func<T, Tuple<T, int>[]> getNeighbours,
        out Dictionary<T, T> comesFrom,
        Predicate<T>? stop = null,
        Action<T>? action = null
    ) where T : notnull =>
        Dijkstra(new [] {startingNode}, getNeighbours, out comesFrom, stop, action);
    
    public static Dictionary<T, int> Dijkstra<T>(
        T[] startingNodes, 
        Func<T, Tuple<T, int>[]> getNeighbours,
        Predicate<T>? stop = null,
        Action<T>? action = null
    ) where T : notnull =>
        Dijkstra(startingNodes, getNeighbours, out _, stop, action);
    
    public static Dictionary<T, int> Dijkstra<T>(
        T startingNode,
        Func<T, Tuple<T, int>[]> getNeighbours,
        Predicate<T>? stop = null,
        Action<T>? action = null
    ) where T : notnull =>
        Dijkstra(new [] {startingNode}, getNeighbours, out _, stop, action);

    public static List<ValueTuple<T, T, T>> FindAllTriangles<T>(List<T> nodes, Func<T, List<T>> connectedTo)
    {
        var result = new List<ValueTuple<T, T, T>>();
        
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            var node = nodes[i];
            var neighbours = connectedTo(node);
            foreach(var neighbour in neighbours)
            {
                var cousins = connectedTo(neighbour);

                foreach (var third in neighbours)
                    if (cousins.Contains(third))
                    {
                        if(!result.Contains((node, neighbour, third)) && 
                           !result.Contains((node, third, neighbour)) &&
                           !result.Contains((neighbour, node, third)) &&
                           !result.Contains((neighbour, third, node)) &&
                           !result.Contains((third, node, neighbour )) &&
                           !result.Contains((third, neighbour, node)) )
                            result.Add((node, neighbour, third));
                    }
            }    
        }

        return result;
    }

    private static void BackTrackFindMaximumClique<T>(
        int index, 
        bool[] isContained,
        bool[][] areConnected,
        int[] nextNeighbour,
        List<T> points,
        ref List<T> result)
    {
        var total = 1;
        for (int i = 0; i < index; i++)
            total += isContained[i] ? 1 : 0;
        if (total + points.Count - index <= result.Count)
            return;
        
        if (index == points.Count)
        {
            var list = new List<T>();
            for(int i =0; i<points.Count; i++)
                if(isContained[i])
                    list.Add(points[i]);
            result = list;
            return;
        }

        isContained[index] = false;
        BackTrackFindMaximumClique(
            index+1, 
            isContained, 
            areConnected, 
            nextNeighbour,
            points,
            ref result);

        bool isConnectedToAllPrevious = true;
        for (int i = 0; i < index && isConnectedToAllPrevious; i++)
            isConnectedToAllPrevious = !isContained[i] || areConnected[i][index];

        if (isConnectedToAllPrevious)
        {
            isContained[index] = true;
            var nextNeighbourIndex = nextNeighbour[index] != -1 ? nextNeighbour[index] : points.Count;
            BackTrackFindMaximumClique(
                nextNeighbourIndex, 
                isContained, 
                areConnected, 
                nextNeighbour,
                points,
                ref result);
            
            isContained[index] = false;
        }
    }

    public static List<T> FindMaximumClique<T>(List<T> points, Func<T, List<T>> connectedTo)
    {
        var result = new List<T>();
        bool[] isContained = new bool[points.Count];
        bool[][] isConnectedTo = new bool[points.Count][];
        int[] nextNeighbour = new int[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            isConnectedTo[i] = new bool[points.Count];
            nextNeighbour[i] = -1;
        }
            

        for (int i = 0; i < points.Count - 1; i++)
        {
            var localNeighbours = connectedTo(points[i]);
            for (int j = i + 1; j < points.Count; j++)
            {
                var areConnected = localNeighbours.Contains(points[j]);
                isConnectedTo[i][j] = areConnected;
                isConnectedTo[j][i] = areConnected;
                if (areConnected && nextNeighbour[i] == -1)
                    nextNeighbour[i] = j;
            }
        }

        BackTrackFindMaximumClique(0, isContained, isConnectedTo, nextNeighbour, points, ref result);

        return result;
    }


}