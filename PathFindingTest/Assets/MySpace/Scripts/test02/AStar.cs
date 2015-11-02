using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStar : MonoBehaviour {
    Node goal;

    public void SetGoal(Vector3 pos ) {
        goal = new Node();
    }

    Graph graph;

    void f()
    {
        var frontier = new Queue<Node>(); // should be priority queue
        Node start = new Node(); // start node
        start.priority = 0;
        frontier.Enqueue(start) ;
        
        Dictionary<Node,int> cost_so_far = new Dictionary<Node, int>();
        cost_so_far.Add(start,0);

        Node current = null;
        while ( (current = frontier.Dequeue() ) != null )
        {
            if (current == goal) break;
            foreach ( var neighb in graph.Neighbors(current) )
            {
                int current_cost;
                cost_so_far.TryGetValue(current, out current_cost);
                int new_cost = current_cost + graph.Cost(current, neighb); // actual cost

                var alreayd_in_list = cost_so_far.ContainsKey(neighb);
                int r_cost = int.MaxValue;
                if ( alreayd_in_list )
                {
                    cost_so_far.TryGetValue(neighb, out r_cost);
                }
                
                if ( !alreayd_in_list )
                {
                    cost_so_far.Add(neighb, new_cost);
                    var priority = new_cost + graph.Heuristic(goal, neighb); // f = g + h , g= actual cost, h = heuristic
                    neighb.priority = priority;
                    frontier.Enqueue(neighb);

                    neighb.previous = current;
                }
                else if ( new_cost < r_cost )
                {
                    cost_so_far[neighb] = new_cost;
                    var priority = new_cost + graph.Heuristic(goal, neighb); // f = g + h , g= actual cost, h = heuristic
                    neighb.priority = priority;
                    frontier.Enqueue(neighb);

                    neighb.previous = current;
                }
            }
        }
    }

    

}


public class Cell
{

}
/*
frontier = PriorityQueue()
frontier.put(start, 0)
came_from = {}
cost_so_far = {}
came_from[start] = None
cost_so_far[start] = 0

while not frontier.empty():
   current = frontier.get()

   if current == goal:
      break
   
   for next in graph.neighbors(current):
      new_cost = cost_so_far[current] + graph.cost(current, next)
      if next not in cost_so_far or new_cost<cost_so_far[next]:
         cost_so_far[next] = new_cost
         priority = new_cost + heuristic(goal, next)
         frontier.put(next, priority)
         came_from[next] = current
*/