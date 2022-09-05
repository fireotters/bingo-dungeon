using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class HighScoreManagement
{
    public static void ResetLevelScores()
    {
        PlayerPrefs.SetString("levelScores", "");
        PlayerPrefs.Save();
    }



    public static bool TryAddLevelScore(string levelName, string scoreType, int score)
    {
        bool isNewHighscore;
        string jsonString = PlayerPrefs.GetString("levelScores");

        // Load the list of level scores from JSON
        Highscores levelScores;
        if (jsonString != "")
            levelScores = JsonUtility.FromJson<Highscores>(jsonString);
        else
            levelScores = new Highscores { highscoreEntryList = new List<HighscoreEntry>() };

        // When adding a high score, take everything from the old list EXCEPT the high score for the current level.
        List<HighscoreEntry> fullListOfScores = levelScores.highscoreEntryList.Where(hs => hs.levelName != levelName).ToList();

        // Find the high score for the current level. If one or zero exist, then overwrite or create an entry respectively. If two or more exist, flag an error.
        int numOfCurrentLevelEntries = levelScores.highscoreEntryList.Where(hs => hs.levelName != levelName).Count();
        HighscoreEntry currentLevelScore;
        if (numOfCurrentLevelEntries == 1)
        {
            currentLevelScore = levelScores.highscoreEntryList.Where(hs => hs.levelName != levelName).ToList()[0];
            isNewHighscore = true;
            if (scoreType == "token" && currentLevelScore.tokenScore < score)
                currentLevelScore.tokenScore = score;
            else if (scoreType == "piece" && currentLevelScore.pieceScore < score)
                currentLevelScore.pieceScore = score;
            else
            {
                Debug.LogError($"Level '{levelName}': Given an invalid victory type.");
                return false;
            }
        }
        else if (numOfCurrentLevelEntries == 0)
        {
            isNewHighscore = true;
            if (scoreType == "token")
                currentLevelScore = new HighscoreEntry { levelName = levelName, tokenScore = score, pieceScore = 0 };
            else if (scoreType == "piece")
                currentLevelScore = new HighscoreEntry { levelName = levelName, tokenScore = 0, pieceScore = score };
            else
            {
                Debug.LogError($"Level '{levelName}': Given an invalid victory type.");
                return false;
            }
        }
        else
        {
            Debug.LogError($"Level '{levelName}': Multiple HighscoreEntry entries in PlayerPrefs. This isn't expected.");
            return false;
        }

        fullListOfScores.Add(currentLevelScore);

        string json = JsonUtility.ToJson(fullListOfScores);
        PlayerPrefs.SetString("levelScores", json);
        PlayerPrefs.Save();
        return isNewHighscore;
    }
}

/* ------------------------------------------------------------------------------------------------------------------
 * Highscores & HighscoreEntry - Public classes for an entire list of highscores, and a single highscore entry
 * ------------------------------------------------------------------------------------------------------------------ */
public class Highscores
{
    public List<HighscoreEntry> highscoreEntryList;
}

// Represents a single Highscore Entry
[System.Serializable]
public class HighscoreEntry
{
    public string levelName;
    public int tokenScore;
    public int pieceScore;
}
