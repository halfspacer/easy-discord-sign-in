using UnityEngine;

namespace Plugins.EasyDiscord.Demo.Scripts {
    public class SceneController : MonoBehaviour {
        public void GoToNextScene() {
            var currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
            } else {
                Debug.Log("<color=#7289DA>EasyDiscord:</color> There is no next scene to go to.");
            }
        }
    }
}