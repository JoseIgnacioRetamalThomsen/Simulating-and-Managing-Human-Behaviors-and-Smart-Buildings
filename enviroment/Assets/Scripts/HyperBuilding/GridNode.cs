using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace HyperBuilding
{
    public class GridNode : MonoBehaviour
    {

        private Grid grid;
        [SerializeField] public string[] Actions;
        [SerializeField] public List<string> actionString = new();
        public string[] PerfomedActions = new string[10];
        [SerializeField] public string section;
        [SerializeField] private Vector2 position;
        [SerializeField] public List<GridNode> Neighbors = new();
        [SerializeField] public List<GridNode> BlockedNeights = new();
        [SerializeField] public GridNode GateNode;
        [SerializeField] public TurnstileThing TurnstileThing;
        public bool isGateOpen = false;
        public readonly Dictionary<(int, int), GridNode> PositionToNodeNeighboursDic = new();
        private int x;
        private int y;

        public string[] getActios()
        {
            List<String> actionsList = new List<string>();
            if (TurnstileThing != null)
            {
                isGateOpen = TurnstileThing.isOPen();
                actionsList.AddRange(TurnstileThing.GetActions());
            }

            actionsList.AddRange(this.actionString);
            return actionsList.ToArray();
        }

        public bool isFree;

        public bool Action_Card_Reader_OPEN(string cardId)
        {
            if (TurnstileThing == null)
            {
                return false;
            }

            Debug.Log("TRYING TO OPEN");
            TurnstileThing.Action_Card_Reader(cardId);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            GridNode otherNode = (GridNode)obj;
            return x == otherNode.x && y == otherNode.y;
        }

        public override string ToString()
        {
            return "x=" + GetX() + ", y =" + GetY();
        }

        private void Awake()
        {
            grid = GetComponentInParent<Grid>();


        }

        public string GetSection()
        {
            return section;
        }

        public (int, int) GetPosition()
        {
            return (x, y);
        }

        private void Start()
        {
            isFree = true;
            x = (int)transform.position.x;
            y = (int)transform.position.z;

            // if (GateNode != null && !BlockedNeights.Contains(GateNode))
            // {
            //     BlockedNeights.Add(GateNode);
            // }
            //
            actionString.AddRange(Actions.Where(s => s != "Card_Reader"));


        }

        public List<GridNodeData> GetNeighbours()
        {

            List<GridNodeData> list = Neighbors.Except(BlockedNeights).Select(gridNode => gridNode.asData()).ToList();
            Debug.Log("Getting Neighbours isGateOpen=" + isGateOpen + ", GateNode=" + GateNode);
            string formattedList = string.Join(", ", list.Select(x => x.ToString()));
            Debug.Log("List before removing gate=" + list);
            if (GateNode != null)
            {
                if (!isGateOpen)
                {
                    list.Remove(GateNode.asData());
                }
            }

            return list;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        /// <summary>
        /// A agent request to move to this Node.
        /// </summary>
        /// <returns>If agent can move will return the position for the agent to move, returns null if is not free</returns>
        public Vector2? RequestToMoveTo()
        {
            if (!isFree) return null;
            isFree = false;
            return new Vector2(GetX(), GetY());
        }

        public void Free()
        {
            isFree = true;
        }

        public GridNodeData asData()
        {
            return new GridNodeData(this);
        }

        public bool GetNeighbour((int x, int y) valueTuple, out GridNode targetNode)
        {
            if (PositionToNodeNeighboursDic.TryGetValue(valueTuple, out var gridNode))
            {
                targetNode = gridNode;
                return true;
            }

            targetNode = null;
            return false;
        }

        public List<GridNodeData> GetSectionNodes()
        {
            return grid.getSectionNodes(this.section);
        }
    }

    [Serializable]
    public class GridNodeData
    {
        public int x { get; set; }

        public int y { get; set; }

        public bool IsFree { get; set; }

        public string section { get; set; }

        public List<string> actions;

        public List<(int, int)> blockedNeightbours;


        public GridNodeData(GridNode node)
        {
            x = node.GetX();
            y = node.GetY();

            IsFree = node.isFree;
            section = node.section;
            // Debug.Log("ACCTIOSNXXXXXXXXXXXXXXXXXXXXXXX node.getActios()" + node.getActios());
            actions = new List<string>(node.getActios());
            blockedNeightbours = node.BlockedNeights.Select(v => (v.GetX(), v.GetY())).ToList();



        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            GridNodeData other = (GridNodeData)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }


    }
}