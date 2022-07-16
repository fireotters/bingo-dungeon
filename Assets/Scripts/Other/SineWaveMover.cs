using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaveMover : MonoBehaviour
{
    [SerializeField] float oscilationSpeed;
    [SerializeField] float amplitude;
    Vector3 initPos;
    float timer = 0;

    private void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * oscilationSpeed;
        transform.localPosition = Mathf.Sin(timer) * Vector3.up * amplitude + initPos;
    }
}
