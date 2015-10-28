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
	
	bool conner;
	RaycastHit conner_anchor;
	Vector3 dir = Vector3.zero;
	
	RaycastHit2D[] hits =new RaycastHit2D[1];

	void OnDrawGizmos () {
		dir = transform.forward;
		
		Vector3 close = dir * detect_range;
		Vector3 far = dir* detect_range * 2;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position,transform.position + close);

		Gizmos.color = Color.grey;
		Gizmos.DrawLine(transform.position,transform.position + far);
	}

	void OnGUI () {
		if ( GUILayout.Button("Play") ) {
			
			UnityEditor.EditorApplication.isPaused = false;
		}
	}
	public void Process (float delta ) {
		dir = transform.forward;

		Vector3 close = dir * detect_range;
		Vector3 far = dir* detect_range * 2;
		
		Vector3 k = Vector3.zero;

		RaycastHit hit;
		if ( Physics.Linecast(transform.position, transform.position + close, out hit) ) {
			//!!! CAUTION !!! // hit.normal in case of contact with complex surfaces
			k = Degree90LineCast(transform.position- hit.point,hit.normal)*strength;
			
			if ( !conner & Physics.Linecast(hit.point, hit.point + k, out hit) ) {
				conner = true;
				conner_anchor = hit;
				Debug.DrawLine(hit.point, hit.point + k, Color.green);
			}
			if( conner ) {
				if ( Vector3.Dot(conner_anchor.normal, k ) < 0 ) 
					k = ReflectLineCast(k,hit.normal);
				Debug.DrawLine(hit.point,hit.point+k, Color.red);
				Debug.DrawLine(conner_anchor.point,conner_anchor.point+conner_anchor.normal, Color.white);
			}
			
			dir = (hit.point+ k - transform.position).normalized;

			Debug.DrawLine(hit.point,hit.point+k, Color.grey);
			
			UnityEditor.EditorApplication.isPaused = true;
		} else if ( Physics.Linecast( transform.position, transform.position + far, out hit)  ) {
			k = Degree90LineCast(transform.position- hit.point,hit.normal)*0.5f * strength;
			
			dir = (hit.point+ k - transform.position).normalized;
			Debug.DrawLine(hit.point,hit.point+k, Color.yellow);
			
			UnityEditor.EditorApplication.isPaused = false;
		} else {
			if ( conner) conner = false;
		}
		
		float abs_angle = AngleOfNormalized(dir);
		
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

