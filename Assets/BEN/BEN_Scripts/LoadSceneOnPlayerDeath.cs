using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public enum Scenes { MainMenu, LevelOne, DeathScreen }

public class LoadSceneOnPlayerDeath : MonoBehaviour
{
    [SerializeField, Range(0f, 15f)] private float delayBeforeLoad = 5f;
    [SerializeField] private AgentGameplayData _playerHP;
    [SerializeField] private Scenes sceneToLoad = Scenes.DeathScreen;
    public static bool playerIsDead; 

    private void OnEnable()
    {
        Health.OnPlayerDeath += LoadNewSceneDecorator; 
    }

    private void OnDisable()
    {
        Health.OnPlayerDeath -= LoadNewSceneDecorator; 
    }

    private void Start()
    {
        playerIsDead = false; 
    }

    private void LoadNewSceneDecorator()
    {
        playerIsDead = true; 
        StartCoroutine(nameof(LoadNewScene)); 
    } 

    private IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad); 
        SceneManager.LoadSceneAsync((int)sceneToLoad, LoadSceneMode.Single);
    }
}
