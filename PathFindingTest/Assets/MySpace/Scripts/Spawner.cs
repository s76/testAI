using UnityEngine;
using System.Collections;

public class Spawner : Pool {
	public UnitSide side;
	public Node startNode;

	int id=0;

	[System.Serializable]
	public class SpawnSample {
		public enum UnitType { Melee, Ranger }
		public UnitParameters2 parameters;
		public UnitType type;
	}

	public SpawnSample[] samples;

	public GameObject SpawnMelee () {
		var g = Get ();
		return SetParams (g, SpawnSample.UnitType.Melee);
	}

	public GameObject SpawnRanger () {
		var g = Get ();
		return SetParams (g, SpawnSample.UnitType.Ranger);

	}

	GameObject SetParams ( GameObject g ,  Spawner.SpawnSample.UnitType type ) {
		foreach (var s in samples) {
			if ( s.type == type ) {
				g.GetComponent<MeshRenderer>().material.color = type == SpawnSample.UnitType.Melee ? Color.red : Color.green;
				//int priority = type == SpawnSample.UnitType.Melee ? 30 : 50;

				var c = g.GetComponent<UnitCore>();
			
				c.parameters.hp_max = s.parameters.hp_max;
				c.parameters.hp_current = s.parameters.hp_max;
				c.parameters.attack_cd = s.parameters.attack_cd;
				c.parameters.attack_range = s.parameters.attack_range;
				c.parameters.attack_damage = s.parameters.attack_damage;
				c.parameters.move_speed = s.parameters.move_speed;
				c.agent.speed = s.parameters.move_speed; 
				c.parameters.sight_angle = s.parameters.sight_angle;
				c.parameters.sight_range = s.parameters.sight_range;

				c.side = side;
				c.id = id++;

				c.currentNode = startNode;
				c.agent.Warp (transform.position);
				c.agent.SetDestination(startNode.real_transform.position);
				//c.agent.avoidancePriority = priority;

				return g;
			}
		}
		throw new UnityException("no valid sample ??");
	}

}
