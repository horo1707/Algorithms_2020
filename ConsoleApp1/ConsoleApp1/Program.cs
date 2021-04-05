using System;
using System.Collections.Generic;

namespace del
{
    class Program
    {
        static void Main()
        {
            Graph graph;

            Console.WriteLine("Вы хотите ввести свой список соседей, или использовать пример? " +
                              "1 - ввести свой" +
                              " 2 - использовать пример");

            var choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Console.WriteLine("Введите количество вершин графа");
                var vertexCount = int.Parse(Console.ReadLine());

                var vertexes = new List<GraphVertex>(vertexCount);

                for (int i = 0; i < vertexCount; i++)
                {
                    vertexes.Add(new GraphVertex(i));
                }

                Console.WriteLine("Введите количество ребер");
                var edgeCount = int.Parse(Console.ReadLine());
                Console.WriteLine("Если вы хотите получить взвешенный граф - то далее дополнительно указывайте вес ребра через пробел");

                var edges = new List<GraphEdge>(edgeCount);
                for (var i = 0; i < edgeCount; i++)
                {
                    var temp = i + 1;
                    Console.WriteLine("Введите номера вершин через пробел, которые хотетие связать ребром " + temp);
                    var vertexNumber = Console.ReadLine().Split();
                    if (vertexNumber.Length == 2)
                    {
                        var first = int.Parse(vertexNumber[0]);
                        var second = int.Parse(vertexNumber[1]);
                        edges.Add(new GraphEdge(vertexes[first], vertexes[second]));
                    }
                    else
                    {
                        var first = int.Parse(vertexNumber[0]);
                        var second = int.Parse(vertexNumber[1]);
                        var third = int.Parse(vertexNumber[2]);

                        edges.Add(new GraphEdge(vertexes[first], vertexes[second], third));
                    }
                }

                graph = new Graph(vertexes, edges);
            }
            else
            {
                var vertexs = new List<GraphVertex>();
                for (var i = 0; i < 6; i++)
                {
                    vertexs.Add(new GraphVertex(i));
                }
                var edges = new List<GraphEdge>();
                edges.Add(new GraphEdge(vertexs[0], vertexs[1]));
                edges.Add(new GraphEdge(vertexs[1], vertexs[2]));
                edges.Add(new GraphEdge(vertexs[2], vertexs[3]));
                edges.Add(new GraphEdge(vertexs[3], vertexs[0]));
                edges.Add(new GraphEdge(vertexs[1], vertexs[3]));
                edges.Add(new GraphEdge(vertexs[3], vertexs[4]));
                edges.Add(new GraphEdge(vertexs[3], vertexs[5]));
                edges.Add(new GraphEdge(vertexs[4], vertexs[5]));

                graph = new Graph(vertexs, edges);
            }

            Console.WriteLine(
                "Какую операцию вы хотите сделать с графом?\n" +
                "1 - Посмотреть матрицу смежности\n" + 
                "2 - Произвести обход в ширину\n" +
                "3 - Волновой алгоритм\n" +
                "4 - Алгоритм Дейкстры(Если вы выбрали использовать пример - то все ребра имеют вес = 1)\n" +
                "5 - Поиск шарниров\n" +
                "6 - Поиск Эйлерова цикла");

            var choise = int.Parse(Console.ReadLine());
            switch (choise)
            {
                case 1:
                    AdjacencyMatrix(graph);
                    break;
                case 2:
                    var result = BFS(graph);
                    foreach (var item in result)
                    {
                        Console.Write(item + " ");
                    }
                    Console.WriteLine("\n--------------------");
                    break;
                case 3:
                    WaveAlgorithm(graph);
                    break;
                case 4:
                    Dijkstra(graph);
                    break;
                case 5:
                    var hringe = GetHringeCount(graph);
                    Console.WriteLine("Количество шарниров равно " + hringe + "\n------------------");
                    break;
                case 6:
                    GetEulerianCycle(graph);
                    break;
                default:
                    break;
            }
        }

        static void AdjacencyMatrix(Graph graph)
        {
            
            

            var matrix = graph.GetAdjacencyMatrix();
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                Console.WriteLine();
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }

            Console.WriteLine("\n-------------------------------------");

            Program.Main();
        }

        static List<int> BFS(Graph graph)
        {
            var result = new List<int>();
            var queue = new Queue<int>();
            var current = 0;

            var containsZero = false;
            foreach (var vertex in graph.Vertexs)
            {
                if (vertex.Name == 0)
                {
                    containsZero = true;
                }
            }
            if (containsZero)
            {
                queue.Enqueue(0);
                result.Add(0);
            }
            else
            {
                queue.Enqueue(1);
                result.Add(1);
            }

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                foreach (var edge in graph.Edges)
                {
                    if (edge.Vertexs.Item1.Name == current && !result.Contains(edge.Vertexs.Item2.Name))
                    {
                        result.Add(edge.Vertexs.Item2.Name);
                        queue.Enqueue(edge.Vertexs.Item2.Name);
                    }
                    else if (edge.Vertexs.Item2.Name == current && !result.Contains(edge.Vertexs.Item1.Name))
                    {
                        result.Add(edge.Vertexs.Item1.Name);
                        queue.Enqueue(edge.Vertexs.Item1.Name);
                    }
                }
            }

            return result;
        }

        static void WaveAlgorithm(Graph graph)
        {
            Console.WriteLine("Укажите номера старотовой и конечной вершины через пробел");
            var vertexNumber = Console.ReadLine().Split();
            var start = int.Parse(vertexNumber[0]);
            var finish = int.Parse(vertexNumber[1]);


            var result = new Dictionary<int, int>();
            var queue = new Queue<int>();
            queue.Enqueue(start);
            result[start] = 0;
            var current = 0;

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                foreach (var edge in graph.Edges)
                {
                    if (edge.Vertexs.Item1.Name == current && !result.ContainsKey(edge.Vertexs.Item2.Name))
                    {
                        result[edge.Vertexs.Item2.Name] = result[current] + 1;
                        queue.Enqueue(edge.Vertexs.Item2.Name);
                    }
                    else if (edge.Vertexs.Item2.Name == current && !result.ContainsKey(edge.Vertexs.Item1.Name))
                    {
                        result[edge.Vertexs.Item1.Name] = result[current] + 1;
                        queue.Enqueue(edge.Vertexs.Item1.Name);
                    }
                }

                if (result.ContainsKey(finish))
                {
                    break;
                }

            }

            Console.WriteLine("Длина искомого маршрута равна " + result[finish] + "\n---------------------");
            Main();
        }

        static void Dijkstra(Graph graph)
        {
            Console.WriteLine("Укажите номера старотовой вершины");
            var start = int.Parse(Console.ReadLine());


            var result = new Dictionary<int, int>();
            var complete = new List<int>();
            var queue = new Queue<int>();
            queue.Enqueue(start);
            result[start] = 0;
            var current = 0;

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                foreach (var edge in graph.Edges)
                {
                    if (edge.Vertexs.Item1.Name == current && !complete.Contains(edge.Vertexs.Item2.Name))
                    {
                        if ((result.ContainsKey(edge.Vertexs.Item2.Name) && result[current] + edge.Cost < result[edge.Vertexs.Item2.Name]) ||
                            !result.ContainsKey(edge.Vertexs.Item2.Name))
                        {
                            result[edge.Vertexs.Item2.Name] = result[current] + edge.Cost;
                        }
                        queue.Enqueue(edge.Vertexs.Item2.Name);
                    }
                    else if (edge.Vertexs.Item2.Name == current && !complete.Contains(edge.Vertexs.Item1.Name))
                    {
                        if ((result.ContainsKey(edge.Vertexs.Item1.Name) && result[current] + edge.Cost < result[edge.Vertexs.Item1.Name]) ||
                           !result.ContainsKey(edge.Vertexs.Item1.Name))
                        {
                            result[edge.Vertexs.Item1.Name] = result[current] + edge.Cost;
                        }
                        queue.Enqueue(edge.Vertexs.Item1.Name);
                    }
                }

                complete.Add(current);
            }

            Console.WriteLine("Длина мартшрута, с началом в вершине " + start + " до всех вершин графа:");
            var firstLine = result.Keys;
            var secondLine = result.Values;

            foreach (var i in firstLine)
            {
                Console.Write(i + " ");
            }
            Console.Write("\n");
            foreach (var i in secondLine)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine("\n---------------------");
            Main();
        }

        static int GetHringeCount(Graph graph)
        {
            var result = 0;
            var startLen = BFS(graph).Count;

            foreach (var vertex in graph.Vertexs)
            {
                var vertexsCopy = new List<GraphVertex>();
                for (var i = 0; i < graph.Vertexs.Count; i++)
                {
                    vertexsCopy.Add(new GraphVertex(i));
                }

                var edgesCopy = new List<GraphEdge>();
                foreach (var edge in graph.Edges)
                {
                    if (!(edge.Vertexs.Item1.Name == vertex.Name || edge.Vertexs.Item2.Name == vertex.Name))
                    {
                        edgesCopy.Add(new GraphEdge(vertexsCopy[edge.Vertexs.Item1.Name], vertexsCopy[edge.Vertexs.Item2.Name]));
                    }
                }

                vertexsCopy.RemoveAt(vertex.Name);

                var copyGraph = new Graph(vertexsCopy, edgesCopy);
                var currentLen = BFS(copyGraph).Count;

                if (currentLen + 1 < startLen)
                {
                    result++;
                }
            }

            return result;
        }

        static void GetEulerianCycle (Graph graph)
        {
            var dict = new Dictionary<int, int>();
            foreach (var edge in graph.Edges)
            {
                if (!dict.ContainsKey(edge.Vertexs.Item1.Name))
                    dict[edge.Vertexs.Item1.Name] = 1;
                else
                    dict[edge.Vertexs.Item1.Name]++;
                if (!dict.ContainsKey(edge.Vertexs.Item2.Name))
                    dict[edge.Vertexs.Item2.Name] = 1;
                else
                    dict[edge.Vertexs.Item2.Name]++;
            }

            var OddVertex = 0;
            foreach (var count in dict.Values)
            {
                if (count % 2 == 1)
                {
                    OddVertex++;
                }
            }

            if (OddVertex > 1 || BFS(graph).Count != graph.Vertexs.Count)
            {
                Console.WriteLine("Граф не содержит Эйлеров цикл");
                Main();
            }

            Console.WriteLine("Введите номер вершины, с которой хотите начать эйлеров цикл");
            var start = int.Parse(Console.ReadLine());

            var result = new List<int>();
            var visited = new List<GraphEdge>();
            var stack = new Stack<int>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var edgeExcist = false;

                foreach (var edge in graph.Edges)
                {
                    var isNotVisited = !visited.Contains(edge);
                    if (edge.Vertexs.Item1.Name == current && isNotVisited)
                    {
                        stack.Push(edge.Vertexs.Item2.Name);
                        visited.Add(edge);
                        edgeExcist = true;
                        break;
                    }
                    else if (edge.Vertexs.Item2.Name == current && isNotVisited)
                    {
                        stack.Push(edge.Vertexs.Item1.Name);
                        visited.Add(edge);
                        edgeExcist = true;
                        break;
                    }
                }

                if (!edgeExcist)
                {
                    stack.Pop();
                    result.Add(current);
                }
            }

            if (visited.Count != graph.Edges.Count)
            {
                Console.WriteLine("Эйлеров цикл из данной вершины не найден");
            }
            else
            {
                Console.WriteLine("Эйлеров цикл начиная с первой вершины имеет следующий вид:");
                foreach (var temp in result)
                {
                    Console.Write(temp + " ");
                }
            }

            Console.WriteLine("\n----------------------");
            Main();
        }
    }

    public class GraphVertex
    {

         public GraphVertex(int name)
         {
            Name = name;
         }

        public int Name { get; }
    }

    public class GraphEdge
    {
        public GraphEdge(GraphVertex first, GraphVertex second, int cost = 1)
        {
            Vertexs = new Tuple<GraphVertex, GraphVertex>(first, second);
            Cost = cost;
        }

        public int Cost;

        public Tuple<GraphVertex, GraphVertex> Vertexs;
    }


    public class Graph
    {
        public Graph(List<GraphVertex> vertexs, List<GraphEdge> edges)
        {
            Vertexs = vertexs;
            Edges = edges;
        }

        public List<GraphVertex> Vertexs;

        public List<GraphEdge> Edges;

        public int[,] GetAdjacencyMatrix()
        {
            var result = new int[Vertexs.Count, Vertexs.Count];

            foreach (var edge in Edges)
            {
                var first = edge.Vertexs.Item1.Name;
                var second = edge.Vertexs.Item2.Name;

                result[first, second]++;
                result[second, first]++;
            }


            return result;
        }
    }
}
