using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TestSteer : MonoBehaviour {
    private Vector3 normal_steering;
    public float radius;
    private GameObject[] other_entities;

    public Transform target;


    GameObject[] entites;
    private bool isStuck;
    private float check_interval = 0.4f;
    private Vector3 last_recorded_pos;
    private Vector3 adv_steering;

    public bool dynamic;
    private float turn_speed = 1f;
    private float speed = 1f;

    void Start()
    {
        entites = GameObject.FindGameObjectsWithTag("Entity");
        StartCoroutine(StuckCheck());
        last_recorded_pos = transform.position;
    }

    private IEnumerator StuckCheck()
    {
        while ( true )
        {
            yield return new WaitForSeconds(check_interval);
            
            if ((transform.position - last_recorded_pos).magnitude < 0.2f)
            {
                isStuck = true;
            }
            last_recorded_pos = transform.position;

            if (isStuck) yield return new WaitForSeconds(2);
        }
    }

    void OnDrawGizmosSelected ()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, radius);

    }
    void Update()
    {
        if (!dynamic) return;

        normal_steering = Vector3.zero;
        var pos = transform.position;
        var dir = transform.forward;

        other_entities = GetEntitiesInRadius(pos, radius);

        normal_steering = BasicSteering(pos, other_entities);

        adv_steering = Vector3.zero;
        if ( isStuck )
        {
            adv_steering = AdvancedSteering(dir,pos, other_entities);
        }

        var total = normal_steering + adv_steering;
        total.Normalize();
        if (total != Vector3.zero)
        {
            var q = Quaternion.LookRotation(total);

            transform.rotation = Quaternion.Lerp(transform.rotation, q, turn_speed * Time.deltaTime);
            transform.position += total * speed * Time.deltaTime;
        }

    }

    private Vector3 AdvancedSteering(Vector3 dir_to_target, Vector3 pos, GameObject[] other_entities)
    {
        Vector3 q = Vector3.zero;
        Transform closest = null;
        float min_dis = float.MaxValue;
        foreach (var r in entites)
        {
            if ((r.transform.position - pos).magnitude < min_dis)
            {
                min_dis = (r.transform.position - pos).magnitude;
                closest = r.transform;
            }
        }

        var pos_e = closest.position;
        List<GameObject> l = new List<GameObject>();
        float sqrRadius = radius * radius;
        foreach (var r in entites)
        {
            if (r == this.gameObject) continue;
            if ((r.transform.position - pos_e).sqrMagnitude > sqrRadius) continue;
            l.Add(r);
        }

        var e_ster =  BasicSteering(pos_e, l.ToArray());
        var e_90 = new Vector3(-e_ster.y, e_ster.x);
        var e_90second = new Vector3(e_ster.y, -e_ster.x);
        if (Vector3.Dot(e_90, dir_to_target) > 0) return e_90;
        else return e_90second;
    }

    private GameObject[] GetEntitiesInRadius(Vector3 pos, float radius)
    {
        List<GameObject> l = new List<GameObject>();
        float sqrRadius = radius * radius;
        foreach ( var r in entites )
        {
            if ((r.transform.position - pos).sqrMagnitude > sqrRadius) continue;
            l.Add(r);
        }
        return l.ToArray();
    }

    private Vector3 BasicSteering(Vector3 pos , GameObject[] others)
    {
        var r = Vector3.zero;
        foreach ( var e in others )
        {
            r += pos - e.transform.position;
        }
        return r;
    }
}
