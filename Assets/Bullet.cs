using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 40;
    [SerializeField] float destroyTime = 7;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * transform.forward, Space.World);
    }
}
