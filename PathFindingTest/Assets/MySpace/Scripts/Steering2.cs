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

	Vector3 dir;

	bool keep_side;		

	Vector3 turn_side;

	Vector3 Vec90 ( Vector3 first , Vector3 normal ) {
		Vector3 left90 = Quaternion.Euler (0, -90, 0) * first;
		left90.Normalize ();
		if (Vector3.Dot (left90, normal) > 0)
			return left90;
		return (Quaternion.Euler (0, 90, 0) * first).normalized;
	}
	public void Process (float delta ) {
		dir = transform.forward;
		
		Vector3 left45 = Quaternion.Euler(0, -45, 0) * dir* detect_range*0.75f;
		Vector3 right45 = Quaternion.Euler(0, 45, 0) * dir* detect_range*0.75f;
		
		
		Vector3 detect_ray = dir * detect_range;
		
		RaycastHit hit, left_hit, right_hit;
		
		Debug.DrawLine (transform.position, transform.position + detect_ray, Color.magenta);

		Vector3 main_steer = Vector3.zero;
		Vector3 left_steer = Vector3.zero;
		Vector3 right_steer = Vector3.zero;

		turn_side = transform.right;

		bool r = false, l= false;
		if (l = Physics.Linecast (transform.position, transform.position + left45, out left_hit)) {
			if (!keep_side) { 
				keep_side = true;
				turn_side = transform.right;
			}

			left_steer = Vec90(left_hit.point - transform.position,transform.forward) * 0.75f * strength ;
			
			Debug.DrawLine (transform.position,left_hit.point , Color.red );
			Debug.DrawLine (transform.position,transform.position + Vec90(left_hit.point - transform.position,transform.forward));
		} else if ( r = Physics.Linecast (transform.position, transform.position + right45, out right_hit)) {
			if (!keep_side) {
				keep_side = true;
				turn_side = -transform.right;	
			}
			right_steer = ReflectLineCast(right_hit.point - transform.position,transform.forward).normalized * 0.75f * strength ;
		} 

		if (Physics.Linecast (transform.position, transform.position + detect_ray, out hit)) {
			main_steer = turn_side;
		} 

		if ( !r & !l) {
			keep_side = false;
		}

		//Debug.DrawLine (transform.position,transform.position+ dir);
		//Debug.DrawLine (transform.position,transform.position+ main_steer);
		//Debug.DrawLine (transform.position,transform.position+ left_steer);
		//Debug.DrawLine (transform.position,transform.position+ right_steer);

		dir = (dir+ main_steer + left_steer + right_steer).normalized;

	//	Debug.DrawLine (transform.position, transform.position + dir, Color.yellow);
	
		var q = Quaternion.LookRotation(dir);

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

	float AngleOfNormalized (Vector3 normalized ) {
		return Mathf.Acos(normalized.x)*Mathf.Rad2Deg;
	}
}

