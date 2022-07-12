using UnityEngine;

public static class ArrayExtensions
{
    public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];

    public static T GetRandom<T>(this T[,] array) => array[Random.Range(0, array.Width()), Random.Range(0, array.Height())];

    public static bool IsValid<T>(this T[,] array, int xPos, int yPos) => xPos >= 0 && xPos < array.GetLength(0) && yPos >= 0 && yPos < array.GetLength(1);

    public static int Width<T>(this T[,] array) => array.GetLength(0);

    public static int Height<T>(this T[,] array) => array.GetLength(1);

    public static void ForEach<T>(this T[,] array,System.Action<int, int, T> action)
    {
        if (action == null)
            return;

        int xSize = array.Width();
        int ySize = array.Height();
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                action.Invoke(i, j, array[i,j]);
            }
        }
    }
}