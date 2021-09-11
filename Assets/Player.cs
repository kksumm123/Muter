using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    [SerializeField] float speed = 25f;
    void Start()
    {
        mapLayer = 1 << LayerMask.NameToLayer("Map");
    }

    void Update()
    {
        StateUpdate();
        Move();
        LookAtMouse();

    }

    #region StateUpdate
    void StateUpdate()
    {
        if (IsGround() == true)
        {
            if (move == Vector3.zero)
                State = StateType.Idle;
            else
                State = StateType.Run;
        }
    }

    #region IsGround
    LayerMask mapLayer;
    bool IsGround()
    {
        return IsHitRay(transform.position, Vector3.down, 0.1f);
    }

    bool IsHitRay(Vector3 pos, Vector3 dir, float distance)
    {
        return Physics.Raycast(pos, dir, distance, mapLayer);
    }
    #endregion IsGround
    #endregion StateUpdate

    #region Move
    Vector3 move;
    Vector3 relateMove;
    void Move()
    {
        move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move.z = 1;
        if (Input.GetKey(KeyCode.S)) move.z = -1;
        if (Input.GetKey(KeyCode.A)) move.x = -1;
        if (Input.GetKey(KeyCode.D)) move.x = 1;

        if (move != Vector3.zero)
        {
            move.Normalize();

            relateMove = Vector3.zero;
            relateMove = Camera.main.transform.forward * move.z;
            relateMove += Camera.main.transform.right * move.x;
            relateMove.y = 0;
            transform.Translate(speed * Time.deltaTime * relateMove, Space.World);
        }
    }
    #endregion Move

    #region LookAtMouse
    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void LookAtMouse()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0;
            dir.Normalize();
            RotationSlerp(dir);
        }
    }
    #endregion LookAtMouse

    #region StateType
    enum StateType
    {
        Idle,
        Run,
        Jump,
        Land,
        Dodge,
        Shot,
        Reload,
        Throw,
        Swap,
        Swing,
    }
    [SerializeField] StateType m_state;
    StateType State
    {
        get => m_state;
        set
        {
            if (m_state == value)
                return;

            print($"PlayerState : {m_state} -> {value}");
            m_state = value;
            PlayAnimation(m_state.ToString());
        }
    }
    #endregion StateType

    #region Methods

    #endregion Methods
}
