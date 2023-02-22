using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevelSelectParent : MonoBehaviour
{
    private ButtonLevelSelect[] levelButtons;
    void Start()
    {
        levelButtons = GetComponentsInChildren<ButtonLevelSelect>();
        int numOfUnlocks = 1;

        if (PlayerPrefs.HasKey("LevelScores"))
        {
            string jsonString = PlayerPrefs.GetString("LevelScores");
            //Debug.Log(jsonString);
            Highscores levelScores = JsonUtility.FromJson<Highscores>(jsonString);
            foreach (HighscoreEntry entry in levelScores.highscoreEntryList)
            {
                foreach (ButtonLevelSelect button in levelButtons)
                {
                    if (entry.levelName == button.attachedLevel)
                    {
                        button.UpdateLevelHighscores(entry.bingoScore, entry.pieceScore);
                        numOfUnlocks++;
                        break;
                    }
                }
            }
        }

        // Only enable the buttons for completed levels, and the next uncomplete level
        for (int i = 0; i < numOfUnlocks; i++)
        {
            levelButtons[i].GetComponentInChildren<Button>().interactable = true;
        }
    }
}
