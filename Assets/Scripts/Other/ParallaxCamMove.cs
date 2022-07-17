using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamMove : MonoBehaviour
{
    [SerializeField] float vel = 0.005f;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + vel, transform.position.y, transform.position.z);
    }
}
