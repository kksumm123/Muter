using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    CharacterController controller;

    [SerializeField] float speed = 25f;

    #region InitGravity
    float gravityAcceleration = 9.81f;
    float gravityVelocity;
    float s;
    void InitGravity()
    {
        gravityAcceleration = 9.81f;
        gravityVelocity = 0;
        s = 0;
        jumpVelo = Vector3.zero;
    }
    #endregion InitGravity
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mapLayer = 1 << LayerMask.NameToLayer("Map");
        InitGravity();
    }

    void Update()
    {
        StateUpdate();

        if (IsGround() == true)
            InitGravity(); //���� �������

        LookAtMouse();
        if (State != StateType.Dodge)
        {
            Move();
            Jump();
            Dodge();
            Attack();
        }
        UseGravity();
    }


    #region StateUpdate
    void StateUpdate()
    {
        if (IsGround() == true)
        {
            if (State == StateType.Dodge)
                return;

            if (move == Vector3.zero)
            {
                switch (State)
                {
                    case StateType.Run:
                        State = StateType.Idle;
                        break;
                    case StateType.Jump:
                        State = StateType.Land;
                        break;
                    case StateType.Land:
                        if (isLanded == false)
                            StartCoroutine(LandToIdleCo());
                        break;
                }
            }
            else
                State = StateType.Run;
        }
        else
        {
            if (jumpVelo.y > 0)
                State = StateType.Jump;
        }
    }

    bool isLanded = false;
    float landTime = 0.64f;
    IEnumerator LandToIdleCo()
    {
        isLanded = true;
        yield return new WaitForSeconds(landTime);
        State = StateType.Idle;
        isLanded = false;
    }

    #region IsGround
    LayerMask mapLayer;
    bool IsGround()
    {
        return IsHitRay(transform.position, Vector3.down, 0.1f);
        //return controller.isGrounded;
    }

    bool IsHitRay(Vector3 pos, Vector3 dir, float distance)
    {
        return Physics.Raycast(pos, dir, distance, mapLayer);
    }
    #endregion IsGround
    #endregion StateUpdate

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
            controller.Move(speed * Time.deltaTime * relateMove);
        }
    }
    #endregion Move

    #region Jump
    [SerializeField] float jumpForce = 2f;
    Vector3 jumpVelo;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGround())
        {
            State = StateType.Jump;
            jumpVelo.y += jumpForce * 0.1f;
        }
    }
    #endregion Jump

    #region Dodge
    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsAbleDodge())
        {
            StartCoroutine(DodgeCo());
        }
    }

    bool IsAbleDodge()
    {
        if (State == StateType.Dodge)
            return false;
        if (IsGround() != true)
            return false;

        return true;
    }

    float dodgeMoveTime = 0.54f;
    float dodgeSpeed = 35;
    IEnumerator DodgeCo()
    {
        // dodge anim speed is 2
        State = StateType.Dodge;
        var dodgeDir = transform.forward;
        var endTime = Time.time + dodgeMoveTime;
        while (Time.time < endTime)
        {
            controller.Move(dodgeSpeed * Time.deltaTime * dodgeDir);
            yield return null;
        }
        State = StateType.Idle;
    }
    #endregion Dodge

    #region Attack
    void Attack()
    {

    }
    #endregion Attack

    #region UseGravity
    void UseGravity()
    {
        gravityAccelerationMove();
    }
    float t;
    void gravityAccelerationMove()
    {
        t = Time.deltaTime;
        s = gravityVelocity + (0.5f * gravityAcceleration * Mathf.Pow(t, 2));
        gravityVelocity += gravityAcceleration * t;

        jumpVelo.y -= s * t;
        controller.Move(jumpVelo);
    }
    #endregion UseGravity



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