using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HyperBuilding
{
    public class AgentManager : MonoBehaviour
    {
        [FormerlySerializedAs("bodiEndPoints")] [FormerlySerializedAs("_agentsEndPoints")] [SerializeField] private BodyEndPoints bodyEndPoints;
        [SerializeField] private Agent AgentPrefab;
        [SerializeField] private Grid GridRef;
        [SerializeField] public Text Input;

        private int count = 0;
        private Queue<string> addAgentQueue = new Queue<string>();

        private Dictionary<string, Agent> _agentsRefs = new();
        //
        // public void AddAgentBody(string agentId, string movesEndPoint, string statusEndPoint,
        //     string useCardReaderEndPoint)
        // {
        //     GridNode firstFreeNodeFound = GridRef.SectionNameToSectionObjectDic["reception"]
        //         .NodesList.FirstOrDefault(node => node.isFree);
        //     if (firstFreeNodeFound == null)
        //     {
        //         throw new Exception("Can't add agent because lobby is full.");
        //     }
        //
        //     Agent newAgent = Instantiate(AgentPrefab);
        //     newAgent.id = agentId;
        //     bool isAdded = GridRef.AddAgent(newAgent, firstFreeNodeFound.GetPosition());
        //     Debug.Log("new Agent added id=" + agentId);
        //     _agentsRefs.Add(agentId, newAgent);
        //
        //     if (isAdded)
        //     {
        //         bodyEndPoints.RegisterAgent(agentId, newAgent, movesEndPoint, statusEndPoint, useCardReaderEndPoint);
        //     }
        // }

        // public void AddAgent()
        // {
        //     GridNode firstFreeNodeFound = GridRef.SectionNameToSectionObjectDic["reception"]
        //         .NodesList.FirstOrDefault(node => node.isFree);
        //
        //     GridNode sp = GridRef.SectionNameToSectionObjectDic["reception"]
        //         .NodesList.FirstOrDefault(node => node.GetX() == -27 && node.GetY() == 6);
        //     if (firstFreeNodeFound == null)
        //     {
        //         throw new Exception("Can't add agent because lobby is full.");
        //     }
        //
        //     string agentId = Input.text;
        //     Agent newAgent = Instantiate(AgentPrefab);
        //     newAgent.id = agentId;
        //     bool isAdded = GridRef.AddAgent(newAgent, sp.GetPosition());
        //     if (isAdded)
        //     {
        //         _agentsEndPoints.RegisterAgent(agentId, newAgent);
        //     }
        // }

        // public Agent getAgentRef(string agentId)
        // {
        //     
        //     CheckForAgentWithTimeout(agentId, agent =>
        //     {
        //         if (agent != null)
        //         {
        //             return agent;
        //         }
        //         else
        //         {
        //             Debug.LogWarning("Agent not found within the timeout.");
        //         }
        //     });
        // }

        public void CheckForAgentWithTimeout(string agentId, Action<Agent> callback)
        {
            StartCoroutine(GetAgentWithTimeout(agentId, callback));
        }

        private float timeout = 30f;

        private IEnumerator GetAgentWithTimeout(string agentId, Action<Agent> callback)
        {
            float startTime = Time.time;

            while (Time.time - startTime < timeout)
            {
                if (_agentsRefs.ContainsKey(agentId))
                {
                    callback.Invoke(_agentsRefs[agentId]);
                    yield break;
                }

                yield return null;
            }

            callback.Invoke(null);
        }
    }
}