using Random = UnityEngine.Random;
using System.Threading;

public static class GenericExtensions
{
    /// <summary>
    /// Cancels de CancellationTokenSource and creates a new one.
    /// </summary>
    public static void CancelAndGenerateNew(ref CancellationTokenSource source)
    {
        source?.Cancel();
        source = new CancellationTokenSource();
    }
}
