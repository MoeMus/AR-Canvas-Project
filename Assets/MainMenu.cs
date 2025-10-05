using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string menuScene = "Menu";
    [SerializeField] string DrawingScene   = "SampleScene";

    public void GoToAR()   => SceneManager.LoadScene(arScene);
    public void GoToMenu() => SceneManager.LoadScene(menuScene);

    // Optional: a generic helper you can call from any button with a parameter
    public void LoadByName(string sceneName) => SceneManager.LoadScene(sceneName);
}
