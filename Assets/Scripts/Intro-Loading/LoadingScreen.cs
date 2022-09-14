using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    private int loadingTextNext = 0;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(LoadingTextAnim), 0, 0.3f);
        Invoke(nameof(LoadMainMenu), 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("2_Menu");
    }

    public void LoadingTextAnim()
    {
        switch(loadingTextNext)
        {
            case 0:
                loadingText.text = "Loading";
                break;
            case 1:
                loadingText.text = "Loading.";
                break;
            case 2:
                loadingText.text = "Loading..";
                break;
            case 3:
                loadingText.text = "Loading...";
                break;
            default:
                loadingText.text = "Loading";
                break;
        }
        if(loadingTextNext < 3)
        {
            loadingTextNext++;
        }
        else
        {
            loadingTextNext = 0;
        }
        
    }
}
