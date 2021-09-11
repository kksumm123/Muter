using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Animator animator;
    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    #region Methods
    protected float fastAimingDistance = 0.2f;
    protected float lookatRotationValue = 0.05f;
    protected void RotationSlerp(Vector3 dir)
    {
        if (Vector3.Distance(transform.forward, dir) < fastAimingDistance)
            transform.forward = Vector3.Slerp(transform.forward, dir, lookatRotationValue * 10);
        else
            transform.forward = Vector3.Slerp(transform.forward, dir, lookatRotationValue);
    }
    #endregion Methods
}
