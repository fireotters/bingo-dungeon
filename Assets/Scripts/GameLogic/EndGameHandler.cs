using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class EndGameHandler : MonoBehaviour
    {
        CompositeDisposable disposables = new CompositeDisposable();
        [SerializeField] private GameObject gameOverOverlay;
        [SerializeField] private StudioEventEmitter gameSong;
        
        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(disposables);
        }

        private void HandleEndGame(SignalGameEnded context)
        {
            if (context.winCondition)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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