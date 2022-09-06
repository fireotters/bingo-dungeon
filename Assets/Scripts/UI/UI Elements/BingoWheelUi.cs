using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Tokens;
using System;
using FMODUnity;
using TMPro;
using UnityEngine.UI;

public class BingoWheelUi : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] Image _ballImage;
    [SerializeField] TextMeshProUGUI _ballText;
    [SerializeField] private StudioEventEmitter bingoRollSfx;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void RunBingoWheelUi(int numChosen, Color colorChosen)
    {
        _animator.SetBool("Appear", true);
        _ballImage.color = colorChosen;
        _ballText.text = numChosen.ToString();
        StartCoroutine(BingoSequence());
    }

    private IEnumerator BingoSequence()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("Sequence", true);
        bingoRollSfx.Play();
        yield return new WaitForSeconds(1.5f);
        _animator.SetBool("Appear", false);
        _animator.SetBool("Sequence", false);
    }
}
