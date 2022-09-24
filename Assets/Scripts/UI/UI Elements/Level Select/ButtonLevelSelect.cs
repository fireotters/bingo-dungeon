using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Signals;
using TMPro;

public class ButtonLevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string levelNum, levelName;
    [HideInInspector] public string attachedLevel;
    private Button button;
    private string _highscoreBingo = "-", _highscorePiece = "-";
    private TextMeshProUGUI _textLevelNum, _textBingoScore, _textPieceScore;

    private void Awake()
    {
        attachedLevel = levelNum.Length == 1 ? "Level0" + levelNum : "Level" + levelNum;
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(LoadLevel);
        _textLevelNum = transform.Find("TextLevelNum").GetComponent<TextMeshProUGUI>();
        _textLevelNum.text = levelNum;
        _textBingoScore = transform.Find("ScoreRecords").Find("TextBingo").GetComponent<TextMeshProUGUI>();
        _textPieceScore = transform.Find("ScoreRecords").Find("TextPiece").GetComponent<TextMeshProUGUI>();
    }

    private void LoadLevel()
    {
        SignalBus<SignalUiMainMenuStartGame>.Fire(new SignalUiMainMenuStartGame { levelToLoad = attachedLevel });
    }

    public void UpdateLevelHighscores(int bingoScore, int pieceScore)
    {
        _highscoreBingo = bingoScore == -1 ? "-" : bingoScore.ToString();
        _highscorePiece = pieceScore == -1 ? "-" : pieceScore.ToString();
        _textBingoScore.text = _highscoreBingo;
        _textPieceScore.text = _highscorePiece;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show the tooltip if the button is interactible
        if (button.interactable)
        {
            string tempLevelName = levelNum + ". " + levelName;
            string tempHighscoreBingo = _highscoreBingo != "-" ? _highscoreBingo : "...";
            string tempHighscorePiece = _highscorePiece != "-" ? _highscorePiece : "...";
            SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange
            {
                Showing = true,
                LevelName = tempLevelName,
                BingoScore = tempHighscoreBingo,
                PieceScore = tempHighscorePiece
            });
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange { Showing = false, LevelName = "", BingoScore = "", PieceScore = "" });
    }
}
