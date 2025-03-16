using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Signals;

public static class HighScoreManagement
{
    public static void ResetLevelScores()
    {
        PlayerPrefs.SetString("LevelScores", "{}");
        PlayerPrefs.SetInt("tutorialUpTo", 0);
        PlayerPrefs.Save();
    }

    // Check new score against existing highscore for a level. Return the original highscore or new highscore as int.
    public static (bool, int) TryAddScoreThenReturnHighscore(string levelName, GameEndCondition scoreType, int score)
    {
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
            Debug.Log("A highscore already exists. Determining what to do...");
            currentLevelScore = levelScores.highscoreEntryList.Where(hs => hs.levelName == levelName).ToList()[0];
            if (scoreType == GameEndCondition.BingoWin)
            {
                bool scoreShouldBeOverwritten = score < currentLevelScore.bingoScore || currentLevelScore.bingoScore == -1;
                if (scoreShouldBeOverwritten)
                    currentLevelScore.bingoScore = score;
                else
                    return (false, currentLevelScore.bingoScore);
            }
            else if (scoreType == GameEndCondition.PieceWin)
            {
                bool scoreShouldBeOverwritten = score < currentLevelScore.pieceScore || currentLevelScore.pieceScore == -1;
                if (scoreShouldBeOverwritten)
                    currentLevelScore.pieceScore = score;
                else
                    return (false, currentLevelScore.pieceScore);
            }
        }
        else
        {
            Debug.Log("A highscore doesn't already exist. Determining what to do...");
            if (numOfCurrentLevelEntries != 0)
                Debug.LogError($"Level '{levelName}': Multiple HighscoreEntry entries in PlayerPrefs. This isn't expected. Overwrite them all with latest score.");

            if (scoreType == GameEndCondition.BingoWin)
                currentLevelScore = new HighscoreEntry { levelName = levelName, bingoScore = score, pieceScore = -1 };
            else if (scoreType == GameEndCondition.PieceWin)
                currentLevelScore = new HighscoreEntry { levelName = levelName, bingoScore = -1, pieceScore = score };
            else
                throw new InvalidEnumArgumentException();
        }

        listOfLevelScores.Add(currentLevelScore);
        Highscores fullListOfScores = new Highscores { highscoreEntryList = listOfLevelScores };

        string json = JsonUtility.ToJson(fullListOfScores);
        Debug.Log(json);
        PlayerPrefs.SetString("LevelScores", json);
        PlayerPrefs.Save();
        return (true, score);
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
    public int bingoScore;
    public int pieceScore;
}
