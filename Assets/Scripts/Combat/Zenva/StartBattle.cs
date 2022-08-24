using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattle : MonoBehaviour
{
    void Start () 
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.gameObject.SetActive(false);
    }
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) 
    {
        if (scene.name == Scenes.TitleScene) 
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("Object destroyed");
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(scene.name == Scenes.BattleScene);
            Debug.Log("Not title scene");
        }
    }
}
