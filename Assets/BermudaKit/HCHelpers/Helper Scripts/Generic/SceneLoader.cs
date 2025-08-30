using System.Collections;
using Ali.Helper.UI;
using UnityEngine.SceneManagement;

namespace Ali.Helper
{
    public class SceneLoader : GenericSingleton<SceneLoader>
    {
        private bool _passingScene;
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneCo(sceneName));
        }
        public IEnumerator LoadSceneCo(string sceneName)
        {
            if (_passingScene)
            {
                yield break;
            }
            _passingScene = true;
            yield return FaderUI.Instance?.CloseTheater();
            SceneManager.LoadScene(sceneName);
            _passingScene = false;
        }
    }
}