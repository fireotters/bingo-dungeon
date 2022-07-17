using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class EndGameHandler : MonoBehaviour
    {
        CompositeDisposable disposables = new CompositeDisposable();
        [SerializeField] private GameObject gameOverOverlay;
        [SerializeField] private GameObject gameWinOverlay;
        [SerializeField] private StudioEventEmitter gameSong;

        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(disposables);
        }

        private void HandleEndGame(SignalGameEnded context)
        {
            if (context.winCondition)
            {
                gameSong.SetParameter("Win", 1);
                gameWinOverlay.SetActive(true);
            }
            else
            {
                gameSong.SetParameter("Dead", 1);
                gameOverOverlay.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}