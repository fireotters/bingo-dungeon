using FMODUnity;
using Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class EndGameHandler : MonoBehaviour
    {
        CompositeDisposable disposables = new CompositeDisposable();
        private GameObject _gameOverOverlay, _gameWinOverlay;
        private StudioEventEmitter _gameSong;

        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(disposables);
            Transform _canvasTransf = FindObjectOfType<Canvas>().transform;
            _gameOverOverlay = _canvasTransf.Find("OverlayGameOver").gameObject;
            _gameWinOverlay = _canvasTransf.transform.Find("OverlayGameWin").gameObject;
            _gameSong = _canvasTransf.GetComponent<StudioEventEmitter>();
            if (_gameSong == null || _gameOverOverlay == null || _gameWinOverlay == null)
            {
                Debug.LogError("EndGameHandler has missing references. These are assigned automatically by script. Investigate why the references aren't being found.");
            }
        }

        private void HandleEndGame(SignalGameEnded context)
        {
            // At times when exiting a level, HandleEndGame will trigge while the overlays are no longer available. Check for null first.
            if (_gameWinOverlay != null && _gameOverOverlay != null)
            {
                if (context.WinCondition)
                {
                    _gameSong.SetParameter("Win", 1);
                    _gameWinOverlay.SetActive(true);
                }
                else
                {
                    _gameSong.SetParameter("Dead", 1);
                    _gameOverOverlay.SetActive(true);
                }
            }
            else
            {
                print("EndGameHandler: Overlays are not present. They are either unassigned, or this call was made just before a scene change & there's no concern.");
            }
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}