using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Tokens;
using System;
using TMPro;
using UnityEngine.UI;

public class BingoWheelUi : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] Image _ballImage;
    [SerializeField] TextMeshProUGUI _ballText;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void RunBingoWheelUi(int numChosen, TokenType tokenChosen)
    {
        Color colorChosen;
        switch (tokenChosen)
        {
            case TokenType.NOTHING:
                colorChosen = Color.white;
                break;
            case TokenType.SHIELD:
                colorChosen = Color.green;
                break;
            case TokenType.WATER:
                colorChosen = Color.cyan;
                break;
            case TokenType.METEOR:
                colorChosen = Color.yellow;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _animator.SetBool("Appear", true);
        _ballImage.color = colorChosen;
        _ballText.text = numChosen.ToString();
        StartCoroutine(BingoSequence());
    }

    private IEnumerator BingoSequence()
    {
        yield return new WaitForSeconds(1f);
        _animator.SetBool("Sequence", true);
        yield return new WaitForSeconds(3f);
        _animator.SetBool("Appear", false);
        _animator.SetBool("Sequence", false);
    }
}
