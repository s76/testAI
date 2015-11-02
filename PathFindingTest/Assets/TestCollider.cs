using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class TestCollider : MonoBehaviour {
    Vector3[] p = new Vector3[0];
    public float radius;

    void Start() {
        UnityEngine.Debug.Log("hahaha");
        int i = 0;
        int max = 100000;
        var t = transform.position;
        var r = new Ray(t, transform.forward);
        Stopwatch sw = new Stopwatch();

        for  (i = 0; i < max; i ++ )
        {
            var v = new Vector3(i, i, i).magnitude;
        }

        for (i = 0; i < max; i++)
        {
            var d = Physics.SphereCastAll(r, 0.2f);
        }


        sw.Start();


        for (i = 0; i < max; i++)
        {
            var v = new Vector3(i, i, i);
        }

        sw.Stop();

        UnityEngine.Debug.Log("Elapsed= "+ sw.Elapsed);

        var t1 = sw.Elapsed;

        sw.Start();


        for (i = 0; i < max; i++)
        {
            var d = Physics.SphereCastAll(r, 0.2f);
        }


        sw.Stop();

        UnityEngine.Debug.Log("Elapsed = " + sw.Elapsed);

        UnityEngine.Debug.Log("Diff = "+ sw.Elapsed.TotalMilliseconds/t1.TotalMilliseconds);

    }

    void OnDrawGizmos () {
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, radius);
		foreach (var k in p) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, k);
		}
	}

	void OnGUI () {
		if ( GUILayout.Button ("Cast")){
			var u = Physics.OverlapSphere(transform.position,radius);
			p = new Vector3[u.Length];
			for(int i=0; i < u.Length; i ++ ) {
				p[i] = u[i].ClosestPointOnBounds(transform.position);
				
			}
		}
	}
}
