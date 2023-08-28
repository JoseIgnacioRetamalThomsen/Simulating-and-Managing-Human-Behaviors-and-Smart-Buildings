using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyperBuilding.JsonObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperBuilding
{
    public class Agent : MonoBehaviour
    {

        public string id;
        public int floorHeight = 1;
        private const float MIN_DISTANCE = 0.1f;
        public GridNode actualNode;

        private GridNode _targetNode;
        private bool _isMovingTo = false;
        private Vector3 _targetPosition;
        private float _speed = 1;

        private Queue<IAgentAction> actions = new Queue<IAgentAction>();

        public void AddAction(IAgentAction action)
        {
            actions.Enqueue(action);
        }


        public List<GridNodeData> GetPosibleMoves()
        {
            var n = actualNode.GetNeighbours();
            return actualNode.GetNeighbours();
        }

        public string GetStatus()
        {
            return _isMovingTo ? "moving" : "idl";
        }

        public AgentView GetShortView()
        {
            return new AgentView()
            {
                agentPosition = actualNode.asData(),
                possibleMoves = actualNode.GetNeighbours(),
                fullGridView = actualNode.GetSectionNodes()
            };
        }

        private void Update()
        {
            if (actions.Count > 0 && !_isMovingTo)
            {
                IAgentAction iaction = actions.Dequeue();
                if (iaction.GetType() == typeof(MoveToPointAction))
                {
                    MoveToPointAction action = (MoveToPointAction)iaction;
                    bool isNode = actualNode.GetNeighbour((action.pointToMove.x, action.pointToMove.y),
                        out var targetNode);

                    if (!isNode)
                    {
                        print("fail to complete action , continue here");
                    }
                    else
                    {
                        TryMoveToNode(targetNode);
                    }
                }
                else if (iaction.GetType() == typeof(CardReaderAction))
                {
                    CardReaderAction action = (CardReaderAction)iaction;
                    UseCardReaderAction(action.id, action.code);
                }

                ///MoveToPointAction action = (MoveToPointAction



            }

            if (_isMovingTo)
            {
                MoveToTarget();
            }
        }

        private void UseCardReaderAction(string actionID, string actionCode)
        {
            Debug.Log("USING CARD READER id=" + id + ", code=" + actionCode);
            actualNode.Action_Card_Reader_OPEN(actionCode);
        }

        private void TryMoveToNode(GridNode nextNode)
        {
            var canMove = nextNode.RequestToMoveTo();
            if (canMove == null) return;
            _targetNode = nextNode;
            _isMovingTo = true;
            _targetPosition = new Vector3(canMove.Value.x, floorHeight, canMove.Value.y);
        }

        private void MoveToTarget()
        {
            if (Vector3.Distance(transform.position, _targetPosition) > MIN_DISTANCE)
            {
                //Debug.Log("agent="+id + " moving to=" + _targetNode);
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
            }
            else
            {
                _isMovingTo = false;
                actualNode.Free();
                actualNode = _targetNode;
                _targetNode = null;
            }
        }

        public void setPosition(Vector3 newPosition, GridNode targetNode)
        {
            this.transform.position = newPosition;
            this.actualNode = targetNode;
        }

     
    }

    public interface IMoveStrategy
    {
        GridNode GetNextMove(List<GridNode> possibleMoves);
    }

    public interface IAgentAction
    {

    }

    [SerializeField]
    public class GridPoint
    {
        public int x;
        public int y;

        public static GridPoint FromTuple((int, int) tuple)
        {
            return new GridPoint()
            {
                x = tuple.Item1,
                y = tuple.Item2
            };
        }
    }

    [SerializeField]
    public class MoveToPointAction : IAgentAction
    {
        public string id;
        public GridPoint pointToMove;
    }

    public class CardReaderAction : IAgentAction
    {
        public string id;
        public string code;
        public string action;
    }

    [Serializable]
    public class AgentView
    {
        public GridNodeData agentPosition;
        public List<GridNodeData> possibleMoves;
        public List<GridNodeData> fullGridView;

    }
}