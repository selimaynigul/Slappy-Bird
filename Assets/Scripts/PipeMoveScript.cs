using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeMoveScript : MonoBehaviour
{

    public float moveSpeed = 10f;

    void Start()
    {
        Destroy(gameObject, 10f);

    }

    void Update()
    {
        transform.position = transform.position + (Vector3.left * moveSpeed) * Time.deltaTime;
    }
}
