using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{

    public void ChangeSceneBtn()
    {
        switch (this.gameObject.name)
        {
            case "op3": 
                SceneManager.LoadScene("Example");
                break;
        }
    }

}
