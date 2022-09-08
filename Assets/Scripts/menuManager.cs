using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    [SerializeField]
    GameObject mainmenu,optionmenu;

    [SerializeField]
    AudioSource audioSource;

    public void startgame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void activate_optionsmenu()
    {
        mainmenu.SetActive(false);
        optionmenu.SetActive(true);
    }

    public void deactivate_optionsmenu()
    {
        optionmenu.SetActive(false);
        mainmenu.SetActive(true);
    }

    public void onslidervaluechange(float value)
    {
        audioSource.volume = value;
    }

    public void exitgame()
    {
        Debug.Log("Exitting");
        Application.Quit();
    }
}
