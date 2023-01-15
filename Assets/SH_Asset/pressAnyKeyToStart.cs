using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pressAnyKeyToStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Opening");
        asyncOperation.allowSceneActivation = false;

        float t = 0;
        float duration = 2.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            print(t / duration);
            GetComponent<CanvasRenderer>().SetAlpha(1.0f - t/ duration);

            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }
}
