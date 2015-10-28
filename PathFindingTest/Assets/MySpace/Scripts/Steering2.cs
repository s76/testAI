using UnityEngine;
using System.Collections;

public class Steering2 : MonoBehaviour
{
	void Update (){
		Process(Time.deltaTime);
	}
	
	public float speed=5;
	public float turn_speed=4;
	public float detect_range;
	public float conner_detect_range;
	public float strength = 2;
	
	public Transform left_point, right_point;
	
	Vector3 dir;

	void CornerCheck (Vector3 tmp01, RaycastHit hit) {

	}

	bool keep_side;		

	Vector3 turn_side;

	public void Process (float delta ) {
		dir = transform.forward;
		
		Vector3 left45 = Quaternion.Euler(0, -45, 0) * dir* detect_range;
		Vector3 right45 = Quaternion.Euler(0, 45, 0) * dir* detect_range;
		
		
		Vector3 detect_ray = dir * detect_range;
		
		RaycastHit hit, left_hit, right_hit;

		if (Physics.Linecast (transform.position, transform.position + detect_ray, out hit)) {
	
			
			Vector3 steer_des = Vector3.zero;

			bool r, l;
			if (l = Physics.Linecast (transform.position, transform.position + left45, out left_hit)) {
				if (!keep_side)
					turn_side = -transform.right;
				steer_des = hit.point + (left_hit.point - hit.point).normalized * strength;
				
				Debug.DrawLine (hit.point, hit.point + steer_des, Color.green);
			} 
			if (r = Physics.Linecast (transform.position, transform.position + right45, out right_hit)) {
				if (!keep_side)
					turn_side = transform.right;			
				steer_des = hit.point + (left_hit.point - hit.point).normalized * strength;
				Debug.DrawLine (hit.point, hit.point + steer_des, Color.green);
			} 

			if ( !r & !l) {
				steer_des = hit.point + Degree90LineCast (transform.position - hit.point, turn_side).normalized * strength;
				Debug.DrawLine (hit.point, hit.point + steer_des, Color.green);
			}

			dir = (steer_des - transform.position).normalized;
		} else {
			keep_side = false;
		}
		
		var q = Quaternion.LookRotation(dir);
		
		Debug.DrawLine (transform.position, transform.position + dir, Color.yellow);
		transform.rotation = Quaternion.Slerp(transform.rotation,q,turn_speed*Time.deltaTime);
		transform.position += dir*speed*Time.deltaTime;
		
		return;
	}
	
	Vector3 SetAngle (float angleDegree ) {
		return new Vector3 ( Mathf.Cos(angleDegree*Mathf.Deg2Rad),0,Mathf.Sin(angleDegree*Mathf.Deg2Rad));
	}
	
	Vector3 ReflectLineCast (Vector3 first , Vector3 normal ) {
		var u = first.normalized;
		if ( Vector3.Dot(u,normal ) == 1 ) return normal;
		var v1 = new Vector3(u.x,-u.y);
		float k = Vector3.Dot(v1,normal);
		if ( k > 0 ) return v1;
		else return new Vector3(-u.x,first.y,u.y);
	}
	
	Vector3 Degree90LineCast ( Vector3 first, Vector3 normal ) {
		Vector3 u = first.normalized;
		float angle = AngleOfNormalized(u);
		
		if (  u.y >=0 ) {
			var r= SetAngle(angle + 90);
			if ( Vector3.Dot(normal,r ) > 0 ) return r;
			else return SetAngle(angle-90);
		}else {
			var r= SetAngle(-angle + 90);
			if ( Vector3.Dot(normal,r ) > 0 ) return r;
			else return SetAngle(-angle-90);
		}
	}
	
	float AngleOfNormalized (Vector3 normalized ) {
		return Mathf.Acos(normalized.x)*Mathf.Rad2Deg;
	}
}

