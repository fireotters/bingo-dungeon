using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Signals;
using TMPro;

public class ButtonLevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string levelName, attachedLevel;
    private Button button;
    private string _highscoreBingo = "-", _highscorePiece = "-";
    private TextMeshProUGUI _textBingoScore, _textPieceScore;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadLevel);
        _textBingoScore = transform.Find("ScoreRecords").Find("TextBingo").GetComponent<TextMeshProUGUI>();
        _textPieceScore = transform.Find("ScoreRecords").Find("TextPiece").GetComponent<TextMeshProUGUI>();
    }

    private void LoadLevel()
    {
        SignalBus<SignalUiMainMenuStartGame>.Fire(new SignalUiMainMenuStartGame { levelToLoad = attachedLevel });
    }

    public void UpdateLevelHighscores(int bingoScore, int pieceScore)
    {
        _highscoreBingo = bingoScore == 0 ? "-" : bingoScore.ToString();
        _highscorePiece = pieceScore == 0 ? "-" : pieceScore.ToString();
        _textBingoScore.text = _highscoreBingo;
        _textPieceScore.text = _highscorePiece;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string levelNum = attachedLevel[^2..].StartsWith("0") ? attachedLevel[^1..] : attachedLevel[^2..];
        string tempLevelName = levelNum + ". " + levelName;
        string tempHighscoreBingo = _highscoreBingo != "-" ? _highscoreBingo : "...";
        string tempHighscorePiece = _highscorePiece != "-" ? _highscorePiece : "...";
        SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange {
            Showing = true, LevelName = tempLevelName, BingoScore = tempHighscoreBingo, PieceScore = tempHighscorePiece
        });
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange { Showing = false, LevelName = "", BingoScore = "", PieceScore = "" });
    }
}
