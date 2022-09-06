using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Signals;
using TMPro;

public class ButtonLevelSelect : MonoBehaviour
{
    public string attachedLevel;
    private Button button;
    private string _highscoreBingo, _highscorePiece;
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
        SignalBus<SignalMainMenuStartGame>.Fire(new SignalMainMenuStartGame { levelToLoad = attachedLevel }) ;
    }

    public void UpdateLevelHighscores(int bingoScore, int pieceScore)
    {
        _highscoreBingo = bingoScore == 0 ? "-" : bingoScore.ToString();
        _highscorePiece = pieceScore == 0 ? "-" : pieceScore.ToString();
        _textBingoScore.text = _highscoreBingo;
        _textPieceScore.text = _highscorePiece;
    }
}
