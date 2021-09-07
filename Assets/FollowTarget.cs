using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    Vector3 posisionOffset;
    void Start()
    {
        posisionOffset = transform.position - target.position;
    }
    void Update()
    {
        transform.position = target.position + posisionOffset;
    }
}
