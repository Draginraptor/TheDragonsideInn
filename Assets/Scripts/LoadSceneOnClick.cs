using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class LoadSceneOnClick : MonoBehaviour {

    // Ref: https://www.youtube.com/watch?v=OWtQnZsSdEU
    
    public void LoadByIndex(int sceneIndex)
    {
        // Uses UnityEngine.SceneManagement
        SceneManager.LoadScene(sceneIndex);

        // Reset the timeScale to 1, may have been left at 0 from a previous level
        Time.timeScale = 1;
    }
}
