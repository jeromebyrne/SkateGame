using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    [SerializeField] private string _gameSettingsAddress;

    private void Awake()
    {
        GameSettingsManager.Initialize(_gameSettingsAddress);
    }

    private void Start()
    {
        LoadTestScene();
    }

    public void LoadTestScene()
    {
        Addressables.LoadSceneAsync("Assets/Scenes/TestScene.unity", LoadSceneMode.Single).Completed += OnTestSceneLoaded;
    }

    private void OnTestSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene loaded successfully!");
        }
        else
        {
            Debug.LogError("Failed to load scene.");
        }
    }
}
