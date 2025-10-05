using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] string menuScene = "Menu";
    [SerializeField] string DrawingScene   = "Painting";
    [SerializeField] string BuildingScene = "BuildModel"

    public void GoToAR()
    {
        SceneManager.LoadScene(DrawingScene);
    }
    public void GoToBuildModel() {
        SceneManager.LoadScene(menuScene);
    } 

    // Optional: a generic helper you can call from any button with a parameter
    public void LoadByName(string sceneName) => SceneManager.LoadScene(sceneName);
}
