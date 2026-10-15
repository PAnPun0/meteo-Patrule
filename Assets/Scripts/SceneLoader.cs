using UnityEngine;
using UnityEngine.SceneManagement;

// Повесь на любой объект в сцене и вызывай методы из OnClick() у кнопок.
public class SceneLoader : MonoBehaviour
{
    // Имена сцен должны совпадать с файлами и быть добавлены в Build Settings
    public const string MenuScene = "MainMenu";
    public const string LabScene = "Laboratory";
    public const string GameScene = "SampleScene";

    public void NewGame()
    {
        GameSession.Reset();
        LoadScene(LabScene);
    }

    public void LoadMenu() => LoadScene(MenuScene);
    public void LoadLab() => LoadScene(LabScene);
    public void LoadGame() => LoadScene(GameScene);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void LoadScene(string sceneName)
    {
        // На случай, если переходим из паузы
        Time.timeScale = 1f;
        PauseMenu.IsPaused = false;
        SceneManager.LoadScene(sceneName);
    }
}
