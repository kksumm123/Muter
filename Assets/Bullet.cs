using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    readonly string bulletCaseString = "BulletCase";
    GameObject bulletCase;

    [SerializeField] float speed = 40;
    [SerializeField] float destroyTime = 7;
    void Start()
    {
        bulletCase = (GameObject)Resources.Load(bulletCaseString);
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * transform.forward);
    }
}
