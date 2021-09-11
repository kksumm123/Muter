using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOrbit : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 moveBackValue;
    float originSize;
    void Start()
    {
        moveBackValue = new Vector3(0, 12, -10);
        rotateDegree = 0;
        originSize = Camera.main.orthographicSize;
    }


    void Update()
    {
        RelativeRotation();
        ZoomInOut();
    }

    [SerializeField] float rotateDegreeValue = 2;
    [SerializeField] float rotateDegree;
    float forwardDegree;
    float dirRadian;
    Vector3 dir;
    void RelativeRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            rotateDegree += rotateDegreeValue;
        if (Input.GetKey(KeyCode.E))
            rotateDegree -= rotateDegreeValue;
        //RotationMatrix();
        //PositionToTarget();
        //MoveBack();
        //Lookat();
    }

    [ContextMenu("방향설정")]
    private void RotationMatrix()
    {
        //삼각함수 회전변환 https://m.blog.naver.com/junhyuk7272/220140840595
        forwardDegree = transform.forward.VectorToDegree();
        dirRadian = (rotateDegree + 90) * Mathf.PI / 180;
        dir.x = Mathf.Cos(dirRadian);
        dir.z = Mathf.Sin(dirRadian);

        transform.forward = dir;
    }
    [ContextMenu("타겟 포지션으로 이동")]
    private void PositionToTarget()
    {
        transform.position = target.position;
    }
    [ContextMenu("회전 값에 대한 상대적 이동")]
    private void MoveBack()
    {
        transform.Translate(moveBackValue);
    }
    [ContextMenu("타겟 바라보기")]
    private void Lookat()
    {
        transform.LookAt(target);
    }

    float scrollMult = 1;
    float minScrollMultValue = 1;
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
