using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCounter : MonoBehaviour
{
    CompositeDisposable disposables = new CompositeDisposable();
    int numOfPieces = 0;

    void Awake()
    {
        SignalBus<SignalPieceAdded>.Subscribe((x) => numOfPieces++).AddTo(disposables);
        SignalBus<SignalEnemyDied>.Subscribe((x) =>
        {
            numOfPieces--;
            if (numOfPieces <= 0)
                SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { winCondition = true });
        }).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
