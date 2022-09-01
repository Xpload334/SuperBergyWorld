using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEnemy : MonoBehaviour
{
    public PartyCharacterManager characterManager;
    [SerializeField]
    private GameObject enemyEncounterPrefab;
    private bool _spawning = false;
    public Collider triggerCollider;
    void Start () 
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        characterManager = FindObjectOfType<PartyCharacterManager>();
    }
    
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) 
    {
        if(scene.name == Scenes.BattleScene) 
        {
            if(this._spawning) 
            {
                Instantiate(enemyEncounterPrefab);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }
    
    void OnTriggerEnter (Collider other) 
    {
        if(other.TryGetComponent(out PlayerStateMachine player) == characterManager.currentCharacter) 
        {
            this._spawning = true;
            SceneManager.LoadScene(Scenes.BattleScene);
        }
    }
}
