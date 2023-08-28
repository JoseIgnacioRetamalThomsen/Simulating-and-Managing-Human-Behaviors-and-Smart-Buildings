using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace HyperBuilding
{
    public class Grid : MonoBehaviour
    {
        private Dictionary<(int, int), GridNode> Nodes { get; set; }

        public Dictionary<string, Location> SectionNameToSectionObjectDic { get; set; } = new();
        public Location LobbyLocation = new Location();
        public Location ReceptionLocation = new Location();
        public List<(int, int)> LobbyNodesPositionList { get; set; }
        public List<(int, int)> ReceptionNodesPositionList { get; set; }

        public List<GridNode> LobbyNodesList { get; set; }
        public List<GridNode> ReceptionNodesList { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }
        private int xOffset;
        private int yOffset;

        private readonly (int, int)[] Directions = new[]
        {
            (0, 1), (0, -1), (1, 0), (-1, 0),
            (-1, -1), (-1, 1), (1, -1), (1, 1)
        };

        private void Start()
        {
            //todo: proper wait for grid to be loaded.
            Debug.Log("Delaying by 1 second the Grid loading.");
            Invoke(nameof(Delayed), 1);
        }

        public void Delayed()
        {
            IEnumerable<GridNode> nodes = transform.GetComponentsInChildren<GridNode>();
            Debug.Log("Nodes size =" + nodes.ToList().Count);
            MakeGrid(nodes);
        }


        public bool AddAgent(Agent agent, (int, int) position)
        {
            GridNode targetNode = Nodes[position];
            if (targetNode == null || !targetNode.isFree)
            {
                return false;
            }

            agent.setPosition(new Vector3(position.Item1, 1f, position.Item2), targetNode);
            targetNode.isFree = false;
            return true;
        }

        private void MakeGrid(IEnumerable<GridNode> unorderedNodes)
        {
            Nodes = new Dictionary<(int, int), GridNode>();
            //rmeove
            LobbyNodesPositionList = new List<(int, int)>();
            LobbyNodesList = new List<GridNode>();
            ReceptionNodesPositionList = new List<(int, int)>();
            ReceptionNodesList = new List<GridNode>();


            foreach (GridNode node in unorderedNodes)
            {
                Nodes[(node.GetX(), node.GetY())] = node;
                if (!SectionNameToSectionObjectDic.TryGetValue(node.GetSection(), out Location location))
                {
                    location = new Location();
                    if (node.GetSection() == "")
                    {
                        throw new Exception("Empty section in node=" + node);
                    }

                    location.SetLocationTypeFromString(node.GetSection());

                    SectionNameToSectionObjectDic[node.GetSection()] = location;
                }

                location.AddNode(node, node.GetX(), node.GetY());

            }

            CreateEdges();
        }

        private void CreateEdges()
        {
            foreach (var node in Nodes.Values)
            {
                UpdateEdges(node);
            }
        }

        private void UpdateEdges(GridNode node)
        {
            node.Neighbors.Clear();
            node.PositionToNodeNeighboursDic.Clear();
            foreach (var (dx, dy) in Directions)
            {
                int nx = node.GetX() + dx;
                int ny = node.GetY() + dy;

                if (!Nodes.TryGetValue((nx, ny), out GridNode neighbor)) continue;
                node.Neighbors.Add(neighbor);
                node.PositionToNodeNeighboursDic[(nx, ny)] = neighbor;
            }
        }

        public List<GridNodeData> GetSection(string getSection)
        {
            throw new NotImplementedException();
        }

        public List<GridNodeData> getSectionNodes(string section)
        {
            if (SectionNameToSectionObjectDic.TryGetValue(section, out Location sectionOut))
            {
                return sectionOut.NodesList.Select(grideNode => grideNode.asData()).ToList();
            }

            return null;
        }
    }

    public class Location
    {
        public LocationType type { get; set; }
        public List<(int, int)> PositionList { get; set; } = new();
        public List<GridNode> NodesList { get; set; } = new();

        public void AddNode(GridNode node, int x, int y)
        {
            PositionList.Add((x, y));
            NodesList.Add(node);
        }

        public void SetLocationTypeFromString(string locationTypeString)
        {
            if (Enum.TryParse<LocationType>(locationTypeString, ignoreCase: true, out LocationType locationType))
            {
                type = locationType;
            }
            else
            {
                Console.WriteLine($"Invalid location type: {locationTypeString}");

            }
        }

        public enum LocationType
        {
            Reception,
            Lobby,
            Hallway,
            Room1,
            Room2,
            Room3,
            Room4,
            Room5,
            SideEntrance,
            Transit
        }
    }
}
