using System;
using UnityEngine;

namespace AStar
{
    public class AStarNode : IComparable<AStarNode>
    {
        public int G; //到达该节点的代价
        public readonly int H; //到达目标点的预估代价
        public int F => G + H; //总代价
        public AStarNode Parent;
        public readonly Vector2Int Coordinate;
        public Vector2Int Up => new Vector2Int(Coordinate.x, Coordinate.y + 1);
        public Vector2Int Down => new Vector2Int(Coordinate.x, Coordinate.y - 1);
        public Vector2Int Left => new Vector2Int(Coordinate.x - 1, Coordinate.y);
        public Vector2Int Right => new Vector2Int(Coordinate.x + 1, Coordinate.y);

        public AStarNode(AStarNode parent, Vector2Int coordinate, int g, int h)
        {
            Parent = parent;
            Coordinate = coordinate;
            G = g;
            H = h;
        }
        
        public AStarNode(AStarNode parent, Vector2Int coordinate, int g, Vector2Int to)
        {
            Parent = parent;
            Coordinate = coordinate;
            G = g;
            H = CalcH(coordinate, to);
        }
        
        public static int CalcH(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
        }

        public int CompareTo(AStarNode other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return F.CompareTo(other.F);
        }
    }
}
