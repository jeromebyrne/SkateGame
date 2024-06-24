using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayModeStartScene
{
    static PlayModeStartScene()
    {
        Debug.Log("PlayModeStartScene Initialized");
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("Exiting Edit Mode");
            // Change this to the path of your start scene
            string startScenePath = "Assets/Scenes/GameStart.unity";

            if (EditorSceneManager.GetActiveScene().path != startScenePath)
            {
                bool saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (saved)
                {
                    EditorSceneManager.OpenScene(startScenePath);
                    Debug.Log("Loading Bootstrap Scene");
                }
                else
                {
                    // User canceled the save operation
                    Debug.LogWarning("Scene save was canceled, remaining in the current scene.");
                    // To prevent entering play mode without switching scene, we can exit play mode.
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}