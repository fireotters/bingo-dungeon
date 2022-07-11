using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class HelpUi : BaseUi
    {
        [SerializeField] private GameObject helpPage, creditsPage;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToMainMenu();
            }
        }

        public void VisitSite(string who)
        {
            switch (who)
            {
                case "bench":
                    Application.OpenURL("https://about.rubenbermejoromero.com/");
                    break;
                case "cross":
                    Application.OpenURL("https://crossfirecam.itch.io/");
                    break;
                case "rioni":
                    Application.OpenURL("https://rioni.itch.io/");
                    break;
                case "darelt":
                    Application.OpenURL("https://darelt.itch.io/");
                    break;
                case "tesla":
                    Application.OpenURL("https://teslasp2.itch.io/");
                    break;
            }
        }

        public void TogglePage()
        {
            helpPage.SetActive(!helpPage.activeInHierarchy);
            creditsPage.SetActive(!creditsPage.activeInHierarchy);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }
    }
}