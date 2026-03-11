using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3Game.Application.Services
{
    /// <summary>
    /// Sahne geçişlerini yönetir.
    /// Build Settings'deki sahne sıralaması:
    /// 0 = MainMenu, 1+ = Level Sahneleri
    /// </summary>
    public class SceneService
    {
        private const int MainMenuSceneIndex = 0;

        public void LoadMainMenu() =>
            SceneManager.LoadScene(MainMenuSceneIndex);

        public void ReloadCurrentLevel() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadNextLevel()
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;

            if(next >= SceneManager.sceneCountInBuildSettings)
                LoadMainMenu();   // Son level -- menu'ye dön
            else
                SceneManager.LoadScene(next);
        }

        public void LoadLevel(int index) =>
            SceneManager.LoadScene(Mathf.Clamp(index, 0, SceneManager.sceneCountInBuildSettings - 1));
    }

}
