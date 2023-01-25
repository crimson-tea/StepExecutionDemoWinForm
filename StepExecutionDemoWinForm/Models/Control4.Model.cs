﻿using StepExecutionWinForm;

namespace AnimationWinForm.Control4;

internal class Model
{
    public IEnumerator<Operation> Dfs(MazeGenerator.Cell[][] cells, int startX, int startY, bool reverse = false)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        Stack<(int x, int y, int length)> stack = new();
        stack.Push((startX, startY, 0));

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (stack.Count > 0)
        {
            var (x, y, length) = stack.Pop();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, length);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            var directions = reverse ? MazeGenerator.Directions.Reverse() : MazeGenerator.Directions;
            foreach (var dir in directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int xx = x + vx;
                int yy = y + vy;

                if (cells[yy][xx] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                stack.Push((xx, yy, length + 1));
            }
        }

        yield return new Operation(OperationType.Complete, previous: prev);
    }

    public IEnumerator<Operation> Dfs2(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        List<Operation> list = new List<Operation>();
        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        Dfs(startX, startY, 0);

        foreach (var item in list)
        {
            yield return item;
        }

        yield return new Operation(OperationType.Complete, previous: prev);

        void Dfs(int x, int y, int pathLength)
        {
            if (visited[y][x])
            {
                return;
            }

            visited[y][x] = true;
            list.Add(new Operation(OperationType.Open, new Point(x, y), prev, pathLength));

            prev = new Point(x, y);

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int xx = x + vx;
                int yy = y + vy;

                if (cells[yy][xx] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                Dfs(xx, yy, pathLength + 1);
            }
        }
    }

    public IEnumerator<Operation> Bfs(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        Queue<(int x, int y, int length)> queue = new();
        queue.Enqueue((startX, startY, 0));

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, length) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, length);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int xx = x + vx;
                int yy = y + vy;

                if (cells[yy][xx] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                queue.Enqueue((xx, yy, length + 1));
            }
        }

        yield return new Operation(OperationType.Complete, previous: prev);
    }

    public IEnumerator<Operation> Dijkstra(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        const int INF = int.MaxValue >> 2;

        int height = cells.Length;
        int width = cells[0].Length;

        int[][] costs = new int[height][];
        for (int i = 0; i < costs.Length; i++)
        {
            costs[i] = new int[width];
            for (int k = 0; k < costs[i].Length; k++)
            {
                costs[i][k] = INF;
            }
        }

        costs[1][1] = 0;

        PriorityQueue<(int x, int y, int cost), int> queue = new();
        queue.Enqueue((startX, startY, 0), 0);

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, cost) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            costs[y][x] = cost;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, cost);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int xx = x + vx;
                int yy = y + vy;

                if (cells[yy][xx] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                if (visited[yy][xx])
                {
                    continue;
                }

                if (costs[yy][xx] <= costs[y][x] + 1)
                {
                    // 各辺の長さが1なのでここには来ない。
                    throw new NotImplementedException();
                }

                queue.Enqueue((xx, yy, cost + 1), cost + 1);
            }
        }

        yield return new Operation(OperationType.Complete, previous: prev);
    }

    public IEnumerator<Operation> AStar(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        int goalX = width - 2;
        int goalY = height - 2;

        PriorityQueue<(int x, int y, int cost), int> queue = new();
        queue.Enqueue((startX, startY, 0), 0);

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, cost) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, cost);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int xx = x + vx;
                int yy = y + vy;

                if (cells[yy][xx] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                queue.Enqueue((xx, yy, cost + 1), cost + 1 + CalcHeulisticCost(xx, yy));
            }
        }

        yield return new Operation(OperationType.Complete, previous: prev);

        int CalcHeulisticCost(int x, int y)
        {
            return Math.Abs(goalX - x) + Math.Abs(goalY - y);
        }
    }
}

internal enum OperationType
{
    None, Complete,
    Open,
}

internal class Operation
{
    public OperationType Type { get; set; }
    public Point Current { get; }
    public Point Previous { get; }
    public int PathLength { get; }

    public Operation(OperationType type, Point current = default, Point previous = default, int pathLength = 0)
    {
        Type = type;
        Current = current;
        Previous = previous;
        PathLength = pathLength;
    }

    public void Deconstruct(out OperationType type, out Point current, out Point previous, out int pathLength) => (type, current, previous, pathLength) = (Type, Current, Previous, PathLength);
    public override string ToString()
    {
        return $"{Type} (x, y) = ({Current}, {Previous}) length: {PathLength}";
    }
}

public static class PointExtensions
{
    public static void Deconstruct(this Point point, out int x, out int y) => (x, y) = (point.X, point.Y);
}