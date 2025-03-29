using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Helicopter's helix animation script
public class HelixMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * speed);
    }
}
