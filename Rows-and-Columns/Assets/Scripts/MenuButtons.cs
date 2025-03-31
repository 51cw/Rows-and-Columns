using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    // Runs when the script instance is being loaded
    private void Awake()
    {
        // Disable debug logs in non-editor builds for cleaner output
        if (!Application.isEditor)
        {
            Debug.unityLogger.logEnabled = false;
        }
    }

    // Loads a specified scene by name
    // Parameters:
    //   name - The name of the scene to load
    public void LoadScene(string name)
    {
        // Use Unity's SceneManager to load the requested scene
        SceneManager.LoadScene(name);
    }
}