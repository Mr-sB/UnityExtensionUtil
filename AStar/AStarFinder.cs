using System.Collections.Generic;
using System.Collections.Generic.PriorityQueue;
using UnityEngine;

namespace AStar
{
    public class AStarFinder
    {
        public int MapWidth { private set; get; }
        public int MapHeight { private set; get; }
        public HashSet<int> Obstacles { private set; get; }
        
        //开列表
        private PriorityQueue<AStarNode> openList = new PriorityQueue<AStarNode>(16, false);
        private Dictionary<int, AStarNode> openDict = new Dictionary<int, AStarNode>(16);
        //闭列表
        private HashSet<int> closeList = new HashSet<int>();

        public AStarFinder()
        {
            MapWidth = 0;
            MapHeight = 0;
            Obstacles = new HashSet<int>();
        }
        
        public AStarFinder(int mapWidth, int mapHeight, IEnumerable<int> obstacles) : this()
        {
            Set(mapWidth, mapHeight, obstacles);
        }

        public void Set(int mapWidth, int mapHeight, IEnumerable<int> obstacles)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            Obstacles.Clear();
            if (obstacles != null)
            {
                foreach (var obstacle in obstacles)
                    Obstacles.Add(obstacle);
            }
        }

        public Vector2Int GetCoordinate(int index)
        {
            return new Vector2Int(index % MapWidth, index / MapWidth);
        }
        
        public int GetIndex(Vector2Int coordinate)
        {
            return coordinate.x + coordinate.y * MapWidth;
        }

        public bool IsPassable(int index)
        {
            if (index < 0 || index >= MapWidth * MapHeight) return false;
            return !Obstacles.Contains(index);
        }
        
        public bool IsPassable(Vector2Int coordinate)
        {
            if (coordinate.x < 0 || coordinate.x >= MapWidth || coordinate.y < 0 || coordinate.y >= MapHeight) return false;
            return !Obstacles.Contains(GetIndex(coordinate));
        }
        
        public List<Vector2Int> FindPath(int from, int to, List<Vector2Int> path = null)
        {
            return FindPath(GetCoordinate(from), GetCoordinate(to), path);
        }

        public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to, List<Vector2Int> path)
        {
            openList.Clear();
            openDict.Clear();
            closeList.Clear();
            path?.Clear();
            if (!IsPassable(from) || !IsPassable(to)) return path;

            OpenEnqueue(new AStarNode(null , from, 0, to));

            bool find = false;
            AStarNode curNode = null;
            while (openList.Count > 0)
            {
                curNode = OpenDequeue();
                if (curNode.Coordinate == to)
                {
                    //到达终点
                    find = true;
                    break;
                }

                closeList.Add(GetIndex(curNode.Coordinate));
                bool updateNode = AddNodeNeighbor(curNode, curNode.Up, to);
                updateNode |= AddNodeNeighbor(curNode, curNode.Down, to);
                updateNode |= AddNodeNeighbor(curNode, curNode.Left, to);
                updateNode |= AddNodeNeighbor(curNode, curNode.Right, to);
                //更新了原本就在开列表里的节点，刷新最小堆
                if (updateNode)
                    openList.Refresh();
            }
            
            //搜索结束
            if (!find) return path;
            
            path ??= new List<Vector2Int>();
            //倒序
            while (curNode != null)
            {
                path.Add(curNode.Coordinate);
                curNode = curNode.Parent;
            }
            //反序
            path.Reverse();
            
            openList.Clear();
            openDict.Clear();
            closeList.Clear();
            return path;
        }
        
        private bool AddNodeNeighbor(AStarNode node, Vector2Int coordinate, Vector2Int to)
        {
            int index = GetIndex(coordinate);
            //在闭列表 或者 不可通行
            //这里必须使用coordinate去判断是否能通行。否则不知道有没有超出宽度界限
            if (closeList.Contains(index) || !IsPassable(coordinate)) return false;
                
            bool updateNode = false;
            if (!openDict.TryGetValue(index, out var nextNode))
            {
                //不在开列表，直接添加
                OpenEnqueue(new AStarNode(node, coordinate, node.G + 1, to));
            }
            else if (node.G + 1 < nextNode.G)
            {
                //更小的G值，更新
                nextNode.G = node.G + 1;
                nextNode.Parent = node;
                updateNode = true;
            }
            return updateNode;
        }

        private void OpenEnqueue(AStarNode node)
        {
            openList.Enqueue(node);
            openDict.Add(GetIndex(node.Coordinate), node);
        }

        private AStarNode OpenDequeue()
        {
            var node = openList.Dequeue();
            openDict.Remove(GetIndex(node.Coordinate));
            return node;
        }
    }
}
