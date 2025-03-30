using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuButtons : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isEditor)
        {
            Debug.unityLogger.logEnabled = false;
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
