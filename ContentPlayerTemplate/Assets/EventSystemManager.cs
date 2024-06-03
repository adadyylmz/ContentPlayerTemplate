using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("EventSystemManager: Instance created and DontDestroyOnLoad applied.");
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            Debug.Log("EventSystemManager: Duplicate instance destroyed.");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("EventSystemManager: Scene loaded: " + scene.name);
        var vrUISystem = FindObjectOfType<BNG.VRUISystem>();
        if (vrUISystem != null)
        {
            Debug.Log("EventSystemManager: VRUISystem found, reinitializing components.");
            //vrUISystem.ReinitializeComponents();
        }
        else
        {
            Debug.LogWarning("EventSystemManager: VRUISystem not found in the scene.");
        }
    }
}
