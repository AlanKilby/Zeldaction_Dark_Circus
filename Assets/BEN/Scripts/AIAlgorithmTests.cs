using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeldaction.Utility; 

public enum AIArchitecture { StateMachine, GOAP } 
public enum AIBehaviour { FullSubmission, LoneWolf }
public enum AIDecisionMaking { Linear, Gaussian, Perlin }


public class AIAlgorithmTests : MonoBehaviour
{
    [Header("Architecture")]
    [SerializeField] private AIArchitecture m_Architecture;

    [Header("Behaviour")]
    [SerializeField] private AIBehaviour m_Behaviour; 

    [Header("Decision Making")]
    [SerializeField] private AIDecisionMaking m_DecisionMaking;


    private LinkedListNode<byte> neighbours;
    // list of possible states 

    private byte amountOfNodes = 10;
    private LinkedListNode<byte>[] graph;
    private bool[] visited; // of size

    // adjacency list of a grid
    // adjacency matrix of a grid

    private void InitialiseGraph() 
    {
        visited = new bool[amountOfNodes];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = false; 
        }
    }

    //very useful to find the shortest path on unweighted graphs
    // store all the neighbours of starting node in a Queue 
    void BFS(LinkedListNode<byte> start, LinkedListNode<byte> end)
    {

    }

    // you can label your nodes to check if they are forming independent groups with numbers (same number = same group)
    void DFS(int index)
    {
        /* if visited[index] return
         * visted[index] = true;    
         * 
         * neighbours = graph[index]
         * for next in neighbours
         * DFS(next)
         * 
         * start_node = 0; 
         * DFS(start_node); */
    }

    bool IsVisited() => Utility.IntToBool(neighbours.Value); 

}
