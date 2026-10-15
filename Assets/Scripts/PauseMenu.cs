using UnityEngine;

// Пауза на Esc. Time.timeScale = 0 останавливает замерзание (PlayerColdSystem
// работает через Time.deltaTime), физику и анимацию кристалла.
public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    [Tooltip("Панель паузы на Canvas (по умолчанию выключена)")]
    public GameObject pausePanel;

    void Start()
    {
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!IsPaused);
        }
    }

    // Кнопка "Продолжить"
    public void Resume() => SetPaused(false);

    // Кнопка "В лабораторию" (переодеться)
    public void GoToLab() => SceneLoader.LoadScene(SceneLoader.LabScene);

    // Кнопка "В меню"
    public void GoToMenu() => SceneLoader.LoadScene(SceneLoader.MenuScene);

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(paused);
    }

    void OnDestroy()
    {
        // Не оставляем игру замороженной при выгрузке сцены
        IsPaused = false;
        Time.timeScale = 1f;
    }
}
