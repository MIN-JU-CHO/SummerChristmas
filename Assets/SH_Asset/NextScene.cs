using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField] GameObject op1;
    [SerializeField] GameObject op2;
    [SerializeField] GameObject op3;
    int openingNum = 1;
    bool keyPressed = false;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!keyPressed)
            {
                keyPressed = true;
                switch (openingNum)
                {
                    case 1:
                        op1.SetActive(false);
                        op2.SetActive(true);
                        break;
                    case 2:
                        op2.SetActive(false);
                        op3.SetActive(true);
                        break;
                    case 3:
                        StartCoroutine(LoadAsyncScene());
                        break;
                }
                openingNum++;
            }
        }
        else
        {
            keyPressed = false;
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Surfing");
        asyncOperation.allowSceneActivation = false;

        float t = 0;
        float duration = 2.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            op3.GetComponent<CanvasRenderer>().SetAlpha(1.0f - t / duration);
            
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }

}
