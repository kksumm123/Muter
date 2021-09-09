using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 moveBackValue;
    void Start()
    {
        moveBackValue = new Vector3(0, 30, -25);
        rotateDegree = 0;
    }


    void Update()
    {
        RelativeRotation();
    }

    [SerializeField] float rotateDegreeValue = 5;
    float rotateDegree;
    float forwardDegree;
    float dirRadian;
    Vector3 dir;
    void RelativeRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            rotateDegree += rotateDegreeValue;
        if (Input.GetKey(KeyCode.E))
            rotateDegree -= rotateDegreeValue;

        forwardDegree = transform.forward.VectorToDegree();
        dirRadian = (rotateDegree + 90) * Mathf.PI / 180;
        dir.x = Mathf.Cos(dirRadian);
        dir.z = Mathf.Sin(dirRadian);

        transform.forward = dir;
        transform.position = target.position;
        transform.Translate(moveBackValue);
        transform.LookAt(target);
    }
}
static public class MyExtension
{
    public static float VectorToDegree(this Vector3 v)
    {
        float radian = Mathf.Atan2(v.z, v.x);
        return (radian * Mathf.Rad2Deg);
    }
}