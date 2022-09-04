using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public static class HighScores
{
    public static bool IsThisAHighScore(int newScore)
    {
        return false;
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
    public int level;
    public int tokenScore;
    public int pieceScore;
}
