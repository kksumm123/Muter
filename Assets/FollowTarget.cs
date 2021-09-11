using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 moveBackValue;
    float originSize;
    void Start()
    {
        moveBackValue = new Vector3(0, 300, -250);
        rotateDegree = 0;
        originSize = Camera.main.orthographicSize;
    }


    void Update()
    {
        RelativeRotation();
        ZoomInOut();
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

        //삼각함수 회전변환 https://m.blog.naver.com/junhyuk7272/220140840595
        forwardDegree = transform.forward.VectorToDegree();
        dirRadian = (rotateDegree + 90) * Mathf.PI / 180;
        dir.x = Mathf.Cos(dirRadian);
        dir.z = Mathf.Sin(dirRadian);

        transform.forward = dir;
        transform.position = target.position;
        transform.Translate(moveBackValue);
        transform.LookAt(target);
    }

    float scrollMult = 1;
    float minScrollMultValue = 0.5f;
    float maxScrollMultValue = 2;
    float tempScrollDeltaY;
    void ZoomInOut()
    {
        tempScrollDeltaY = Input.mouseScrollDelta.y;
        if (tempScrollDeltaY != 0)
        {
            scrollMult -= tempScrollDeltaY * 0.1f;
            scrollMult = Mathf.Min(scrollMult, maxScrollMultValue);
            scrollMult = Mathf.Max(scrollMult, minScrollMultValue);
            Camera.main.orthographicSize = originSize * scrollMult;
        }
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