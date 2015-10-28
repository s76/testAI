using UnityEngine;
using System.Collections;

public class Steering2
{
	public float speed=2;
	public float turn_speed=4;
	public float detect_range = 3;
	public float strength = 2;

	Vector3 dir;

	bool keep_side;		

	Vector3 turn_side;

	static Vector3 Vec90 ( Vector3 first , Vector3 normal ) {
		Vector3 left90 = Quaternion.Euler (0, -90, 0) * first;
		left90.Normalize ();
		if (Vector3.Dot (left90, normal) > 0)
			return left90;
		return (Quaternion.Euler (0, 90, 0) * first).normalized;
	}

	public void Process (Transform unit, float delta ) {
		dir = unit.forward;
		
		Vector3 left45 = Quaternion.Euler(0, -45, 0) * dir* detect_range*0.75f;
		Vector3 right45 = Quaternion.Euler(0, 45, 0) * dir* detect_range*0.75f;
		
		
		Vector3 detect_ray = dir * detect_range;
		
		RaycastHit hit, left_hit, right_hit;

		Vector3 main_steer = Vector3.zero;
		Vector3 left_steer = Vector3.zero;
		Vector3 right_steer = Vector3.zero;

		turn_side = unit.right;

		bool r = false, l= false;
		if (l = Physics.Linecast (unit.position, unit.position + left45, out left_hit)) {
			if (!keep_side) { 
				keep_side = true;
				turn_side = unit.right;
			}

			left_steer = Vec90(left_hit.point - unit.position,unit.forward) * 0.75f * strength ;

		} else if ( r = Physics.Linecast (unit.position, unit.position + right45, out right_hit)) {
			if (!keep_side) {
				keep_side = true;
				turn_side = -unit.right;	
			}
			right_steer = ReflectLineCast(right_hit.point - unit.position,unit.forward).normalized * 0.75f * strength ;
		} 

		if (Physics.Linecast (unit.position, unit.position + detect_ray, out hit)) {
			main_steer = turn_side;
		} 

		if ( !r & !l) {
			keep_side = false;
		}


		dir = (dir+ main_steer + left_steer + right_steer).normalized;
	
		var q = Quaternion.LookRotation(dir);

		unit.rotation = Quaternion.Slerp(unit.rotation,q,turn_speed*Time.deltaTime);
		unit.position += dir*speed*Time.deltaTime;
		
		return;
	}

	static Vector3 ReflectLineCast (Vector3 first , Vector3 normal ) {
		var u = first.normalized;
		if ( Vector3.Dot(u,normal ) == 1 ) return normal;
		var v1 = new Vector3(u.x,-u.y);
		float k = Vector3.Dot(v1,normal);
		if ( k > 0 ) return v1;
		else return new Vector3(-u.x,first.y,u.y);
	}


}

