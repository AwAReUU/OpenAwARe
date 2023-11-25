using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathData
{
    public List<(Vector3, Vector3)> edges;
    public float radius;
}

public class BranchingPathData
{
    //list of nodes?
    //node knows all its neighbours
    //how to determine startpoint? is it even important or is this something for pre-refinement?
    //behavior for adding and removing nodes?
    //just a big ol list? pre-refinement makes a (line) pathdata from this?
    //yes, big ol list. finding start and endpoint(s) is a problem for pre-refinement (as well as finding the best single-line path)
    //have this class keep track of (potential) start/endpoint and intersections

    //all nodes on the path (includes junctions and endpoints)
    List<PathNode> nodes = new();
    //all nodes with more than 2 neighbours on the path
    List<PathNode> junctions = new();
    //all nodes with exactly 1 neighbour on the path
    List<PathNode> endpoints = new();



    //todo:
    //convert bool grid into branchingpathdata
    //convert List<PathNode> result form possible startstate into PathData
    //make heuristic evaluation function

    //erosion
    //path endpoint extension

    //for me (marco) todo:
    //update on git
    //convert bool array into branchingpathdata
    //extend endpoint vector shoot to wall thingy (from pathdata or from bool array? from pathdata)
    
    //addnode can (probably) be used to create / initialize branchingpathdata

    void addNode(int x, int y, List<PathNode> neighbours)
    {
        //create a new node
        //add it to all neighbours, add all neighbours to it
        //if neighbours count is one, add this one to endpoints
        //if neighbours count is more than 2, add this one to junctions

        PathNode node = new();
        node.position = (x, y);
        node.neighbours = neighbours;

        //add the new node to all neighbours
        for(int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].neighbours.Add(node);
        }

        if (neighbours.Count == 1) endpoints.Add(node);
        if (neighbours.Count > 2) junctions.Add(node);
        nodes.Add(node);
    }

    List<List<(int x, int y)>> PossibleOneLinePaths()
    {
        List<List<PathNode>> paths = new();

        //consider each endpoint. after this loop the paths list should be filled
        //what if there are no endpoints on the path? start at random point? todo
        for(int i = 0; i < endpoints.Count; i++)
        {
            List <PathNode> newpath = new();
            newpath.Add(endpoints[i]);
            FindPaths(newpath, ref paths);
        }

        //filter out duplicate paths
        for(int i = 0; i < paths.Count; i++)
        {
            List<PathNode> points = paths[i];
            //consider all other paths
            for (int j = i + 1; j < paths.Count; j++)
            {
                //if the number of points is different, it is not a duplicate. continue on
                if(points.Count != paths[j].Count)
                {
                    continue;
                }

                //compare all points
                bool duplicate = true;
                for(int z = 0; z < paths[j].Count; z++)
                {
                    //if a points is not in both lists, it is not a duplicate
                    if(!points.Contains(paths[j][z]))
                    {
                        duplicate = false;
                        break;
                    }
                }

                //if a path is a duplicate, remove it
                if(duplicate)
                {
                    paths.RemoveAt(j);
                    j--;
                }
            }
        }

        //temp, fix error
        return new();
    }

    void FindPaths(List<PathNode> incompletepath, ref List<List<PathNode>> paths)
    {
        List<PathNode> path = new List<PathNode>(incompletepath);

        //improve condition
        bool continueloop = true;
        bool completedpath = false;
        while(continueloop)
        {
            PathNode latestpoint = incompletepath[incompletepath.Count - 1];

            //we have found an end node
            if(latestpoint.neighbours.Count == 1)
            {
                if (path.Contains(latestpoint.neighbours[0]))
                {
                    continueloop = false;
                    completedpath = true;
                }
                else
                {
                    path.Add(latestpoint.neighbours[0]);
                    continueloop = false;
                    completedpath = true;
                }
            }
            //we are at a regular point. add the next point, and ensure we are not adding the previous point again
            else if(latestpoint.neighbours.Count == 2)
            {
                if (path.Contains(latestpoint.neighbours[0]))
                {
                    path.Add(latestpoint.neighbours[1]);
                }
                else if (path.Contains(latestpoint.neighbours[1]))
                {
                    path.Add(latestpoint.neighbours[0]);
                }
                //we are at an (inescapable) circle
                else
                {
                    completedpath = true;
                    continueloop = false;
                } 
            }
            //we have found a junction. 'split' further         
            else
            {
                for(int i = 0; i < latestpoint.neighbours.Count; i++)
                {
                    if (path.Contains(latestpoint.neighbours[i])) continue;
                    List<PathNode> pathclone = new List<PathNode>(incompletepath);
                    pathclone.Add(latestpoint.neighbours[i]);
                    FindPaths(pathclone, ref paths);
                    continueloop = false;
                }
            }
        }

        if(completedpath)
        {
            paths.Add(path);
        }
    } 
}

class PathNode
{
    public (int x, int y) position;
    public List<PathNode> neighbours = new();
}
