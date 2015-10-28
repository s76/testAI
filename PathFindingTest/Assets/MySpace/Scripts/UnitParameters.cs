
[System.Serializable] 
public class UnitParameters2 {
	public int hp_max;
	public int hp_current;
	public float sight_angle;
	public float sight_range;
	public int attack_damage;
	public float attack_cd;
	public float attack_range;
	public float move_speed;
	public float barier_search_range;
}

public enum UnitSide:int { Side01=0, Side02=1 }

public enum UnitType { Melee, Ranger } 

public enum BehaviorState { MoveAlongPath, Attack, Approach, TryMoveAround, WaitInPosition }