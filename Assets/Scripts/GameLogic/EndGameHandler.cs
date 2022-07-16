using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class EndGameHandler : MonoBehaviour
    {
        CompositeDisposable disposables = new CompositeDisposable();
        [SerializeField] private GameObject gameOverOverlay;

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
                gameOverOverlay.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}