using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverTextMeshProUGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color baseColor, highlightColor, clickColor;
    private TextMeshProUGUI _text;
    private bool _highlighted, _clicked;

    private void Start()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        _highlighted = true;
        if (!_clicked)
            _text.color = highlightColor;
        else if (_clicked)
            _text.color = clickColor;
    }
    public void OnPointerExit(PointerEventData data)
    {
        _highlighted = false;
        _text.color = baseColor;
    }
    public void OnPointerDown(PointerEventData data)
    {
        _clicked = true;
        _text.color = clickColor;
    }
    public void OnPointerUp(PointerEventData data)
    {
        _clicked = false;
        if (_highlighted)
            _text.color = highlightColor;
        else if (!_highlighted)
            _text.color = baseColor;
    }
}
