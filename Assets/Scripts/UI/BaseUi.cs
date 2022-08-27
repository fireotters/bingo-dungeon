using UnityEngine;

namespace UI
{
    public class BaseUi : MonoBehaviour
    {
        public void SwapFullscreen()
        {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
        }
    }    
}