using Signals;
using UnityEngine;

public class PieceCounter : MonoBehaviour
{
    CompositeDisposable disposables = new CompositeDisposable();
    [HideInInspector] public int numOfPieces = 0;

    void Awake()
    {
        SignalBus<SignalPieceAdded>.Subscribe((x) => numOfPieces++).AddTo(disposables);
        SignalBus<SignalEnemyDied>.Subscribe((x) =>
        {
            numOfPieces--;
            if (numOfPieces <= 0)
                SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { result = GameEndCondition.PieceWin });
        }).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}