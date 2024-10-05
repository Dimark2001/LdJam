using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}