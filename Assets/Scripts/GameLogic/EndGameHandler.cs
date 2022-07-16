using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHandler : MonoBehaviour
{
    CompositeDisposable disposables = new CompositeDisposable(); 

    void Start()
    {
        SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(disposables);
    }

    void HandleEndGame(SignalGameEnded context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
