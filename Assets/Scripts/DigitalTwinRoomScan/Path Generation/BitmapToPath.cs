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
                //volgensmij is dit fout, hij kijkt nu niet om zich heen maar 'rechts onder' zich. offset van -1 is nodig,
                //dus for loops aanpassen dat ze op -1 beginnen en tot 2 eindigen, if u==0 en v==0 aanpassen in de clause
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
                    //what does this to achieve?
                    //i think this is supposed add something to the neighbour and also create a new entry
                    //but i dont think thats how the method works
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

        //this while loop will never be entered as no items are put into the stack before the loop
        //i think this is supposed to construct the path but i dont think it is done very well?
        //it does not seem to take into account how 'branches' or 'forks' of the path are filtered
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

    //the idea here is good and can be reused, but i think the implementation has a few problems
}
