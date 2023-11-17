using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class BitmapToPath
{
    public static List<Vector2Int> Convert(bool[,] bitmap)
    {
        var links = new Dictionary<Vector2Int, List<Vector2Int>>();
        for (int x = 0; x < bitmap.GetLength(0); x++)
        {
            for (int y = 0; y < bitmap.GetLength(1); y++)
            {
                var neighbours = new List<Vector2Int>();
                for (int u = 0; u < 3; u++)
                {
                    for (int v = 0; v < 3; v++)
                    {
                        if (u == 1 && v == 1) continue;
                        int p = x + u;
                        int r = y + v;

                        if (p >= 0 && p < bitmap.GetLength(0) && v >= 0 && v < bitmap.GetLength(1))
                        {
                            if (bitmap[p, r])
                            {
                                neighbours.Add(new Vector2Int(p, r));
                            }
                        }
                    }
                }
                if (bitmap[x, y])
                {
                    links.TryGetValue(new Vector2Int(x, y), out var list);
                    foreach (var nb in neighbours)
                    {
                        list.Add(nb);
                    }
                }
            }
        }

        var path = new LinkedList<Vector2Int>();
        var stack = new Stack<Vector2Int>();

        while (stack.Count != 0)
        {
            var insert = stack.Pop();

            if (links[path.First.Value].Contains(insert))
            {
                path.AddFirst(insert);
                foreach (var nb in links[insert])
                {
                    stack.Push(nb);
                }
            }
            else if (links[path.Last.Value].Contains(insert))
            {
                path.AddLast(insert);
                foreach (var nb in links[insert])
                {
                    stack.Push(nb);
                }
            }
        }

        return path.ToList();
    }
}
