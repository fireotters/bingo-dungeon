using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Signals;

public static class HighScoreManagement
{
    public static void ResetLevelScores()
    {
        PlayerPrefs.SetString("LevelScores", "");
        PlayerPrefs.Save();
    }

    public static bool TryAddLevelScore(string levelName, GameEndCondition scoreType, int score)
    {
        bool isNewHighscore;
        string jsonString = PlayerPrefs.GetString("LevelScores");

        // Load the list of level scores from JSON
        Highscores levelScores;
        if (jsonString != "")
            levelScores = JsonUtility.FromJson<Highscores>(jsonString);
        else
            levelScores = new Highscores { highscoreEntryList = new List<HighscoreEntry>() };

        // When adding a high score, take everything from the old list EXCEPT the high score for the current level.
        List<HighscoreEntry> listOfLevelScores = levelScores.highscoreEntryList.Where(hs => hs.levelName != levelName).ToList();

        // Find the high score for the current level. If one or zero exist, then overwrite or create an entry respectively. If two or more exist, flag an error.
        int numOfCurrentLevelEntries = levelScores.highscoreEntryList.Where(hs => hs.levelName == levelName).Count();
        HighscoreEntry currentLevelScore;
        if (numOfCurrentLevelEntries == 1)
        {
            currentLevelScore = levelScores.highscoreEntryList.Where(hs => hs.levelName == levelName).ToList()[0];
            isNewHighscore = true;
            if (scoreType == GameEndCondition.BingoWin && currentLevelScore.tokenScore < score)
                currentLevelScore.tokenScore = score;
            else if (scoreType == GameEndCondition.PieceWin && currentLevelScore.pieceScore < score)
                currentLevelScore.pieceScore = score;
            else
                return false;
        }
        else if (numOfCurrentLevelEntries == 0)
        {
            isNewHighscore = true;
            if (scoreType == GameEndCondition.BingoWin)
                currentLevelScore = new HighscoreEntry { levelName = levelName, tokenScore = score, pieceScore = 0 };
            else if (scoreType == GameEndCondition.PieceWin)
                currentLevelScore = new HighscoreEntry { levelName = levelName, tokenScore = 0, pieceScore = score };
            else
                return false;
        }
        else
        {
            Debug.LogError($"Level '{levelName}': Multiple HighscoreEntry entries in PlayerPrefs. This isn't expected.");
            return false;
        }

        listOfLevelScores.Add(currentLevelScore);
        Highscores fullListOfScores = new Highscores { highscoreEntryList = listOfLevelScores };

        string json = JsonUtility.ToJson(fullListOfScores);
        Debug.Log(json);
        PlayerPrefs.SetString("LevelScores", json);
        PlayerPrefs.Save();
        return isNewHighscore;
    }
}

/* ------------------------------------------------------------------------------------------------------------------
 * Highscores & HighscoreEntry - Public classes for an entire list of highscores, and a single highscore entry
 * ------------------------------------------------------------------------------------------------------------------ */

[Serializable]
public class Highscores
{
    public List<HighscoreEntry> highscoreEntryList;
}

// Represents a single Highscore Entry
[Serializable]
public class HighscoreEntry
{
    public string levelName;
    public int tokenScore;
    public int pieceScore;
}
