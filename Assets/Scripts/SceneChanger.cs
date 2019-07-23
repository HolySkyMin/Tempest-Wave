using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TempestWave.Core.UI;

namespace TempestWave
{
    public class SceneChanger : MonoBehaviour
    {
        public Text loadingText, loadingValue;
        private float presetTime = 0.5f;

        IEnumerator AnimatingChange(string sceneName, float time)
        {
            yield return new WaitForSeconds(time);
            loadingText.gameObject.SetActive(true);
            loadingValue.gameObject.SetActive(true);
            
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
            loading.allowSceneActivation = false;
            
            while(!loading.isDone)
            {
                float value = Mathf.Clamp01(loading.progress / 0.9f);
                loadingValue.text = (value * 100).ToString("N0") + "%";

                if (loading.progress.Equals(0.9f))
                {
                    loading.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        public void ChangeToScene(string sceneName, float time)
        {
            ThemeApplier.Instances.Clear();
            StartCoroutine(AnimatingChange(sceneName, time));
        }

        public void ChangeToScene(string sceneName)
        {
            ThemeApplier.Instances.Clear();
            StartCoroutine(AnimatingChange(sceneName, presetTime));
        }

        public void SetTime(float value)
        {
            presetTime = value;
        }

        IEnumerator NA_Animate(string sceneName)
        {
            yield return new WaitForSeconds(presetTime);
            loadingText.gameObject.SetActive(true);
            SceneManager.LoadScene(sceneName);
        }

        public void NoAsyncChange(string sceneName)
        {
            ThemeApplier.Instances.Clear();
            StartCoroutine(NA_Animate(sceneName));
        }
    }
}

