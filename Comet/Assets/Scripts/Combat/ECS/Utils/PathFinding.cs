using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace comet.combat
{
    public class SimplePathFinding
    {
        public static List<Vector2Int> FindPath(MapRecord mapRecord,Vector2Int from,Vector2Int to)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            Vector2Int next = from;
            result.Add(next);

            while (next.x != to.x)
            {
                int delta = (to.x - next.x);
                
                int x = delta > 0 ? next.x + 1 : next.x - 1;
                next.x = x;
                result.Add(next);
            }

            while (next.y != to.y)
            {
                int delta = (to.y - next.y);
                
                int y = delta > 0 ? next.y + 1 : next.y - 1;
                next.y = y;
                result.Add(next);
            }
            
            return result;
        }
    }

    public class AStar
    {
        class Node
        {
            public int X;
            public int Y;
            //public bool IsWall;
            public List<Node> Neighbors;
            public Node Parent;
            public float GCost;
            public float HCost;
            public float FCost => GCost + HCost;
            
            public Node(int x,int y)
            {
                X = x;
                Y = y;
                //IsWall = false;
                Neighbors = new List<Node>();
                Parent = null;
                GCost = 0;
                HCost = 0;
            }

            public void Reset()
            {
                Parent = null;
                GCost = 0;
                HCost = 0;
            }

            public bool IsWall(MapRecord mapRecord)
            {
                GridRecord gridRecord = mapRecord.GetGridAt(Y, X);
                if (gridRecord == null)
                    return true;
                return gridRecord.GridType == EGridType.Wall;
            }
        }

        private MapRecord _mapRecord = null;
        private Node[,] _nodes = null;

        public AStar(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
            _nodes = new Node[_mapRecord.GridRows,_mapRecord.GridCols];
            
            // Create all nodes
            for (int y = 0;y < _mapRecord.GridRows;y++)
            {
                for (int x = 0; x < _mapRecord.GridCols; x++)
                {
                    Node node = new Node(x,y);
                    _nodes[y, x] = node;
                    
                    GridRecord gridRecord = _mapRecord.GetGridAt(y, x);
                    //node.IsWall = (gridRecord.GridType != GridRecord.EGridType.Plane);
                }
            }
            
            // Record all neigbors
            for (int y = 0;y < _mapRecord.GridRows;y++)
            {
                for (int x = 0; x < _mapRecord.GridCols; x++)
                {
                    Node west = GetNode(x - 1, y);
                    Node north = GetNode(x, y + 1);
                    Node east = GetNode(x + 1, y);
                    Node south = GetNode(x, y-1);
                    Node westsouth = GetNode(x - 1, y-1);
                    Node westnorth = GetNode(x - 1, y+1);
                    Node eastnorth = GetNode(x + 1, y+1);
                    Node eastsouth = GetNode(x+1, y-1);
                    
                    if (westsouth != null) _nodes[y,x].Neighbors.Add(westsouth);
                    if (west != null) _nodes[y,x].Neighbors.Add(west);
                    if (westnorth != null) _nodes[y,x].Neighbors.Add(westnorth);
                    if (north != null) _nodes[y,x].Neighbors.Add(north);
                    if (eastnorth != null) _nodes[y,x].Neighbors.Add(eastnorth);
                    if (east != null) _nodes[y,x].Neighbors.Add(east);
                    if (eastsouth != null) _nodes[y,x].Neighbors.Add(eastsouth);
                    if (south != null) _nodes[y,x].Neighbors.Add(south);
                }
            }
        }

        public void Reset()
        {
            for (int y = 0;y < _mapRecord.GridRows;y++)
            {
                for (int x = 0; x < _mapRecord.GridCols; x++)
                {
                    _nodes[y,x].Reset();           
                }
            }
        }

        private Node GetNode(int x,int y)
        {
            if (!CheckValid(x, y))
                return null;
            return _nodes[y, x];
        }

        bool CheckValid(int x,int y)
        {
            return !(x < 0 || x >= _mapRecord.GridCols || y < 0 || y >= _mapRecord.GridRows);
        }

        public List<Vector2Int> FindPath(Vector2Int from,Vector2Int to)
        {
            Reset();

            Node startNode = GetNode(from.x, from.y);
            Node targetNode = GetNode(to.x, to.y);
            
            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);
            
            while (openSet.Count > 0)
            {
                Node curNode = GetMinCostNode(openSet);
                openSet.Remove(curNode);
                closeSet.Add(curNode);
                if (curNode == targetNode)
                {
                    return GeneratePath(startNode,targetNode);
                }
                
                foreach (Node neighbor in curNode.Neighbors)
                {
                    if (closeSet.Contains(neighbor) || neighbor.IsWall(_mapRecord))
                    {
                        continue;
                    }

                    float movementCost = curNode.GCost + GetDistance(curNode,neighbor);
                    if (movementCost < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = movementCost;
                        neighbor.HCost = GetDistance(neighbor,targetNode);
                        neighbor.Parent = curNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }

        private Node GetMinCostNode(List<Node> openSet)
        {
            Node curNode = openSet[0];
            for (int i = 0;i < openSet.Count;i++)
            {
                Node theNode = openSet[i];
                if (openSet[i].FCost < curNode.FCost
                    || (theNode.FCost == curNode.FCost && theNode.HCost < curNode.FCost))
                {
                    curNode = theNode;
                }
            }
            return curNode;
        }

        private float GetDistance(Node n1,Node n2)
        {
            float disX = System.Math.Abs(n1.X - n2.X);
            float disY = System.Math.Abs(n1.Y - n2.Y);
            // return Mathf.Sqrt(disX * disX + disY * disY);
            return disX + disY;
        }

        private List<Vector2Int> GeneratePath(Node startNode,Node targetNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Node currentNode = targetNode;
            while (currentNode != startNode)
            {
                path.Add(new Vector2Int(currentNode.X,currentNode.Y));
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}