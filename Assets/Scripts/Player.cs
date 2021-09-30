using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
TodoList
모든 탄약은 Player에서 관리할 것.

무기 교체할 때마다 새로이 Instantiate함, 비활성/활성으로 바꿔야함.
왜? 교체할 때마다 탄이 꽉차게 될테니

총알 넉백등에 대한 구현 해줘야 함

UI 만들자

귀찬타ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
 */
public class Player : Actor
{
    CharacterController controller;

    [SerializeField] float speed = 25f;

    [SerializeField] Transform weaponPosition;
    WeaponInfo currentWeapon;
    [SerializeField] WeaponInfo mainWeapon;
    [SerializeField] WeaponInfo subWeapon;
    [SerializeField] WeaponInfo meleeWeapon;
    [SerializeField] WeaponInfo throwWeapon;

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

        ChangeWeapon(mainWeapon);
    }


    WeaponInfo InitWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo != null)
        {
            var newWeaponGo = Instantiate(weaponInfo, weaponPosition.position, Quaternion.identity);
            newWeaponGo.transform.localScale = weaponPosition.lossyScale;
            newWeaponGo.transform.parent = weaponPosition;
            newWeaponGo.transform.localRotation = Quaternion.identity;
            newWeaponGo.Init();
            return newWeaponGo;
        }
        Debug.LogError($"무기 비어있음 {weaponInfo}");
        return null;
    }
    void ChangeWeapon(WeaponInfo weaponInfo)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        currentWeapon = InitWeapon(weaponInfo);

        if (currentWeapon == null)
            return; // 비었을 경우 방어코드
    }

    void Update()
    {
        StateUpdate();

        if (IsGround() == true)
            InitGravity(); //땅에 닿았으면

        LookAtMouse();
        if (State != StateType.Dodge)
        {
            if (IsBattleState() == false)
            {
                Move();
                Jump();
                Dodge();
                Attack();
                SelectWeapon();
            }
        }
        UseGravity();
    }

    #region SelectWeapon
    private void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeapon(mainWeapon);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeapon(subWeapon);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeapon(meleeWeapon);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeWeapon(throwWeapon);
    }
    #endregion SelectWeapon


    #region StateUpdate
    void StateUpdate()
    {
        if (IsBattleState() == true)
            return;

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

    bool IsBattleState()
    {
        switch (State)
        {
            case StateType.Shot:
            case StateType.Reload:
            case StateType.Throw:
            case StateType.Swap:
            case StateType.Swing:
                return true;
        }
        return false;
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
    GameObject Bullet => currentWeapon.bullet;
    Transform BulletPosition => currentWeapon.bulletPosition;
    GameObject BulletCase => currentWeapon.bulletCase;
    Transform BulletCasePosition => currentWeapon.bulletCasePosition;
    float attackableTime;
    Coroutine attackShotCoHandle;
    void Attack()
    {
        if (Input.GetMouseButton(0))
        {
            if (attackableTime < Time.time)
            {
                switch (currentWeapon.weaponType)
                {
                    case WeaponInfo.WeaponType.None:
                        Debug.LogError($"오류 : 무기 타입 None, {currentWeapon}");
                        return;
                    case WeaponInfo.WeaponType.Gun:
                        attackShotCoHandle = StopAndStartCo(attackShotCoHandle, AttackShotCo());
                        break;
                    case WeaponInfo.WeaponType.Melee:
                        break;
                    case WeaponInfo.WeaponType.Throw:
                        break;
                }
                attackableTime = Time.time + currentWeapon.delay;
            }
        }
    }

    float waitShotEndTime = 0.5f;
    IEnumerator AttackShotCo()
    {
        // Shot Animation Speed 3
        State = StateType.Shot;
        yield return null;

        var bulletGo = Instantiate(Bullet, BulletPosition.position, transform.rotation);
        //Instantiate(BulletCase, BulletCasePosition.position, Quaternion.identity);
        // 총알 넉백 작성해야함
        // bulletGo.knockBackForce = currentWeapon.knockBackForce;

        yield return new WaitForSeconds(currentWeapon.delay);
        State = StateType.ShotEnd;
        yield return new WaitForSeconds(waitShotEndTime);
        PlayAnimation("Idle", 0);
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
        ShotEnd,
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

            switch (m_state)
            {
                case StateType.ShotEnd:
                    break;
                case StateType.Shot:
                    PlayAnimation(m_state.ToString(), 0);
                    break;
                case StateType.Idle:
                case StateType.Run:
                case StateType.Jump:
                case StateType.Land:
                case StateType.Dodge:
                case StateType.Reload:
                case StateType.Throw:
                case StateType.Swap:
                case StateType.Swing:
                    PlayAnimation(m_state.ToString());
                    break;
            }
        }
    }
    #endregion StateType

    #region Methods
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);

        return;
    }
    Coroutine StopAndStartCo(Coroutine handle, IEnumerator Fn)
    {
        StopCo(handle);
        return StartCoroutine(Fn);
    }
    #endregion Methods
}
