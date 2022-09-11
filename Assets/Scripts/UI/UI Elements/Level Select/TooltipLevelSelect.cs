using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Signals;
using TMPro;

public class TooltipLevelSelect : MonoBehaviour
{
    public bool _showing;
    private GameObject _tooltipBg;
    private TextMeshProUGUI _textLevelName, _textBingoScore, _textPieceScore;
    private RectTransform _rectTransform, _canvasRectTransform;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        _tooltipBg = transform.Find("TooltipBg").gameObject;
        _textLevelName = _tooltipBg.transform.Find("TextLevelName").GetComponent<TextMeshProUGUI>();
        _textBingoScore = _tooltipBg.transform.Find("TextBingoScore").GetComponent<TextMeshProUGUI>();
        _textPieceScore = _tooltipBg.transform.Find("TextPieceScore").GetComponent<TextMeshProUGUI>();
        SignalBus<SignalUiMainMenuTooltipChange>.Subscribe(ChangeState).AddTo(_disposables);
    }

    private void Update()
    {
        Vector2 anchoredPosition = Input.mousePosition / _canvasRectTransform.localScale.x;
        if (anchoredPosition.x + _rectTransform.rect.width > _canvasRectTransform.rect.width)
            anchoredPosition.x = _canvasRectTransform.rect.width - _rectTransform.rect.width;
        if (anchoredPosition.y > _canvasRectTransform.rect.height)
            anchoredPosition.y = _canvasRectTransform.rect.height - _rectTransform.rect.height;
        _rectTransform.anchoredPosition = anchoredPosition;
    }

    private void ChangeState(SignalUiMainMenuTooltipChange signal)
    {
        _textLevelName.text = signal.LevelName;
        _textBingoScore.text = signal.BingoScore switch
        {
            "..." => "...",
            "1" => "1 turn",
            _ => signal.BingoScore + " turns",
        };
        _textPieceScore.text = signal.PieceScore switch
        {
            "..." => "...",
            "1" => "1 turn",
            _ => signal.PieceScore + " turns",
        };
        _tooltipBg.SetActive(signal.Showing);
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
