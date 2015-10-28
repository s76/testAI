using UnityEngine;
using System.Collections;

public class UnitManualSteering : MonoBehaviour
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

	
	bool conner;
	RaycastHit conner_anchor;
	Vector3 dir = Vector3.zero;
	
	RaycastHit2D[] hits =new RaycastHit2D[1];

	void OnDrawGizmos () {
		dir = transform.forward;
		
		Vector3 close = dir * detect_range;
		Vector3 far = dir* detect_range * 2;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(left_point.position,left_point.position + close);

		Gizmos.color = Color.grey;
		Gizmos.DrawLine(left_point.position,left_point.position + far);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(right_point.position,right_point.position + close);
		
		Gizmos.color = Color.grey;
		Gizmos.DrawLine(right_point.position,right_point.position + far);
	}

	void OnGUI () {
		if ( GUILayout.Button("Play") ) {
			
			UnityEditor.EditorApplication.isPaused = false;
		}
	}

	void CornerCheck (Vector3 tmp01, RaycastHit hit) {
		RaycastHit _hit;
		if ( !conner & Physics.Linecast(hit.point, hit.point + tmp01.normalized * conner_detect_range, out _hit) ) {
			conner = true;
			conner_anchor = _hit;
			Debug.DrawLine(_hit.point, _hit.point + tmp01* conner_detect_range, Color.green);
		}
		if( conner ) {
			if ( Vector3.Dot(conner_anchor.normal, tmp01 ) < 0 ) 
				tmp01 = ReflectLineCast(tmp01,conner_anchor.normal);
			Debug.DrawLine(conner_anchor.point,conner_anchor.point+tmp01, Color.red);
			Debug.DrawLine(conner_anchor.point,conner_anchor.point+conner_anchor.normal, Color.white);
		}
		
		dir = (conner_anchor.point+ tmp01 - transform.position).normalized;
	}

	public void Process (float delta ) {
		dir = transform.forward;

		Vector3 side_left = Quaternion.Euler(0, -50, 0) * dir;
		Vector3 size_right = Quaternion.Euler(0, 50, 0) * dir;


		Vector3 close = dir * detect_range;
		Vector3 far = dir* detect_range * 2;
		
	
		Vector3 tmp01 = Vector3.zero;
		Vector3 tmp02 = Vector3.zero;

		bool _is_left_hit, _is_right_hit;
		RaycastHit main_left, main_right, hit;

		_is_left_hit = Physics.Linecast (left_point.position, left_point.position + close, out main_left);
		_is_right_hit = Physics.Linecast (right_point.position, right_point.position + close, out main_right);

		if (_is_left_hit & _is_right_hit) {
			tmp01 = Degree90LineCast (left_point.position - main_left.point, main_left.normal);
			tmp02 = Degree90LineCast (right_point.position - main_right.point, main_right.normal);
			var m = Vector3.Dot (tmp01, tmp02);
			if ( Mathf.Abs(m + 1) < 0.1f) {
				//2 different directions , need to use side check
				// need improvement later
				bool side_l;//, side_r;
				side_l = Physics.Linecast (transform.position, transform.position + side_left*detect_range, out main_left);
			//	side_r = Physics.Linecast (transform.position, transform.position + size_right*detect_range, out main_right);
				tmp01 = side_l == true ? tmp02 : tmp01;
				hit = side_l == true ? main_right : main_left;
			} else if ( Mathf.Abs(m - 1 ) < 0.1f) {
				// 2 vectors point in the same direction 
				hit = main_left;
			} else {
				throw new UnityException("error , m = " + m );
			}
			CornerCheck(tmp01, hit);
		} else if (_is_left_hit) {
			// only ray left hit , need a side check to determine to turn left or not
			tmp01 = Degree90LineCast (left_point.position - main_left.point, main_left.normal);
			hit = main_left;
			
			CornerCheck(tmp01, hit);
		} else if (_is_right_hit) {
			// only ray right hit , need a side check to determine to turn right or not
			tmp01 = Degree90LineCast (right_point.position - main_right.point, main_right.normal);
			hit = main_right;

			
			CornerCheck(tmp01, hit);
		} else {
			// nothing detected at close range

			_is_left_hit = Physics.Linecast (left_point.position, left_point.position + far, out main_left);
			_is_right_hit = Physics.Linecast (right_point.position, right_point.position + far, out main_right);
			
			
			if (Physics.Linecast (transform.position, transform.position + far, out hit)) {
				tmp01 = Degree90LineCast (transform.position - hit.point, hit.normal) * 0.5f * strength;
				
				dir = (hit.point + tmp01 - transform.position).normalized;
				Debug.DrawLine (hit.point, hit.point + tmp01, Color.yellow);
			}

			if (_is_left_hit & _is_right_hit) {
				tmp01 = Degree90LineCast (left_point.position - main_left.point, main_left.normal);
				tmp02 = Degree90LineCast (right_point.position - main_right.point, main_right.normal);
				var m = Vector3.Dot (tmp01, tmp02);
				if ( Mathf.Abs(m + 1) < 0.1f) { 
					//2 different directions , need to use side check
					// need improvement later
					bool side_l;//, side_r;
					side_l = Physics.Linecast (transform.position, transform.position + side_left*detect_range*2, out main_left);
					//	side_r = Physics.Linecast (transform.position, transform.position + size_right*detect_range, out main_right);
					tmp01 = side_l == true ? tmp02 : tmp01;
					hit = side_l == true ? main_right : main_left;
				} else if ( Mathf.Abs(m - 1 ) < 0.1f) {
					// 2 vectors point in the same direction 
					hit = main_left;
				} else {
					throw new UnityException ("should be 1/-1 here : m = " + m);
				}
				CornerCheck(tmp01, hit);
			} else if (_is_left_hit) {
				// only ray left hit , need a side check to determine to turn left or not
				tmp01 = Degree90LineCast (left_point.position - main_left.point, main_left.normal);
				
				
				CornerCheck( tmp01, hit);
			} else if (_is_right_hit) {
				// only ray right hit , need a side check to determine to turn right or not
				tmp01 = Degree90LineCast (right_point.position - main_right.point, main_right.normal);
				
				
				CornerCheck(tmp01, hit);
			} else {
				if ( conner) conner = false;
			}
		}
		
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

