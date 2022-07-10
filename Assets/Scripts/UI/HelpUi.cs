using UnityEngine;

namespace UI
{
    public class HelpUi : BaseUi
    {
        [SerializeField] private GameObject helpPage, creditsPage;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // exit menu
            }
        }

        public void VisitSite(string who)
        {
            switch (who)
            {
                case "bench":
                    Application.OpenURL("");
                    break;
                case "cross":
                    Application.OpenURL("");
                    break;
                case "rioni":
                    Application.OpenURL("");
                    break;
                case "darelt":
                    Application.OpenURL("");
                    break;
                case "tesla":
                    Application.OpenURL("");
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
            
        }
    }
}