using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadFirstScene()
    {
        SceneManager.LoadScene("Level");
    }
    
    public static void LoadNextScene()
    {
        var index = SceneManager.GetActiveScene().buildIndex+1;
        if (index >= SceneManager.sceneCountInBuildSettings)
            index = 0;
        SceneManager.LoadScene(index);
    }

    public static void Menu()
    {
        SceneManager.LoadScene("Main");
    }
}