using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Scaler : MonoBehaviour
{
    [SerializeField] float scaleSpeed;
    [SerializeField] float initialScaleSpeed;
    [SerializeField] float amplitude;
    public AnimationCurve curve;
    Vector3 initPos;

    private void Start()
    {
        initPos = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, initialScaleSpeed).OnComplete(() => StartCoroutine(Scale()));
    }

    IEnumerator Scale()
    {
        float insideTimer = 0;
        while (true)
        {
            transform.localScale = curve.Evaluate(insideTimer) * Vector3.up * amplitude + initPos;
            insideTimer += Time.deltaTime;

            if (insideTimer > scaleSpeed)
                insideTimer -= scaleSpeed;

            yield return null;
        }
    }
}
