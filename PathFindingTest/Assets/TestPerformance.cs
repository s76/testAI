using UnityEngine;
using System.Collections;
using System;
    using System.Diagnostics;

public class TestPerformance : MonoBehaviour {
    class Test
    {
        public int k = 0;
        public object o = null;
    }
    Test test_ob;

    BoxCollider[] bc;

	// Use this for initialization
	void Start () {
        test_ob = new Test();
        StartCoroutine(DelayInit(test_ob));
        Stopwatch sw = new Stopwatch();

        bc = GameObject.FindObjectsOfType<BoxCollider>();

        int max = 1000000;
        int i;
        float k;
        for (i = 0; i < max; i ++ )
        {
            
        }

        Collider[] colliders;
        for (i = 0; i < max; i++)
        {
            colliders = Physics.OverlapSphere(transform.position, 2.445f);
        }

        sw.Start();
        for (i = 0; i < max; i++)
        {
            k = new Vector3(i * 2.3f, i * 121f, i - 22f).magnitude;

        }

        sw.Stop();
        var first = sw.ElapsedMilliseconds;

        sw.Start();

        for (i = 0; i < max; i++)
        {
            colliders = Physics.OverlapSphere(transform.position, 2.445f);
        }

        sw.Stop();
        var sec = sw.ElapsedMilliseconds;

        UnityEngine.Debug.Log("dif = "+(sec/first));
    }


    private IEnumerator DelayInit(Test to)
    {
        yield return new WaitForSeconds(2f);
        to.o = new object();
    }

    // Update is called once per frame
    void Update () {
        UnityEngine.Debug.Log("test ob = null ? " + (test_ob.o == null) );
	}
}
