using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{    
    public void StartGame(){
        SceneManager.LoadScene("FarmHub");
    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }
    public void CalculatorButton(){
        SceneManager.LoadScene("Calculator");
    }
}