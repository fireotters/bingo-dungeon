using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckForBingo : MonoBehaviour
{
    public TextMeshProUGUI debugBingoDisplay;

    // TODO After Jam: The FloorGridGenerator fills out numbers from bottom to top, left to right. Instead of top to bottom.
    // Not important right now. The Bingo logic will still work, even upside down.
    private int[] board =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    public void UpdateTokenPlacement(int notation, bool addOrRemove)
    {
        if (addOrRemove == true)
        {
            board[notation] = 1;
        }
        else
        {
            board[notation] = 0;
        }

        print("Is there Bingo? " + CheckForBingoResult());
    }

    private void Update()
    {
        string b = string.Join(", ", board);
        if (debugBingoDisplay != null)
        {
            debugBingoDisplay.text = b;
        }
    }

    private bool CheckForBingoResult()
    {
        // Horizontal
        for (int notation = 0; notation < board.Length; notation++)
        {
            if (board.Length < (notation + 1) + 4)
                break; // Impossible to be in a row from now

            string check =
                board[notation].ToString() +
                board[notation + 1].ToString() +
                board[notation + 2].ToString() +
                board[notation + 3].ToString() +
                board[notation + 4].ToString();
            if (check == "11111")
                return true;
        }

        // Vertical
        for (int notation = 0; notation < board.Length; notation++)
        {
            if (board.Length < (notation + 1) + 72)
                break; // Impossible to be in a column from now

            string check =
                board[notation].ToString() +
                board[notation + 18].ToString() +
                board[notation + 36].ToString() +
                board[notation + 54].ToString() +
                board[notation + 72].ToString();
            if (check == "11111")
                return true;
        }

        // Dia Right
        for (int notation = 0; notation < board.Length; notation++)
        {
            if (board.Length < (notation + 1) + 76)
                break; // Impossible to be in a dia right from now

            string check =
                board[notation].ToString() +
                board[notation + 19].ToString() +
                board[notation + 38].ToString() +
                board[notation + 57].ToString() +
                board[notation + 76].ToString();
            if (check == "11111")
                return true;
        }

        // Dia Left
        for (int notation = 0; notation < board.Length; notation++)
        {
            if (board.Length < (notation + 1) + 68)
                break; // Impossible to be in a dia left from now

            string check =
                board[notation].ToString() +
                board[notation + 17].ToString() +
                board[notation + 34].ToString() +
                board[notation + 51].ToString() +
                board[notation + 68].ToString();
            if (check == "11111")
                return true;
        }

        return false;
    }
}