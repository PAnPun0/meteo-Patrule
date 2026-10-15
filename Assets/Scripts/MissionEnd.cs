using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Конец миссии: зона-триггер в конце уровня. Когда игрок в неё входит,
// игра останавливается и показывается оценка экипировки.
// Можно также вызвать FinishMission() из другого скрипта (например, после спасения NPC).
public class MissionEnd : MonoBehaviour
{
    [Header("Требования уровня к одежде")]
    public OutfitRequirements requirements = new OutfitRequirements();

    [Header("Игрок (если пусто — найдётся при входе в триггер)")]
    public EquipmentSystem equipmentSystem;
    public PlayerColdSystem coldSystem;

    [Header("UI результата")]
    public GameObject resultPanel;          // по умолчанию выключена
    public TextMeshProUGUI scoreText;       // "85 / 100"
    public TextMeshProUGUI gradeText;       // "Отлично"
    public TextMeshProUGUI feedbackText;    // список замечаний
    public Image[] stars;                   // 3 картинки звёзд (необязательно)
    public Color starOnColor = Color.yellow;
    public Color starOffColor = new Color(0.3f, 0.3f, 0.3f);

    private bool finished = false;

    void Start()
    {
        if (resultPanel) resultPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished || !other.CompareTag("Player")) return;

        if (equipmentSystem == null) equipmentSystem = other.GetComponentInParent<EquipmentSystem>();
        if (coldSystem == null) coldSystem = other.GetComponentInParent<PlayerColdSystem>();
        FinishMission();
    }

    public void FinishMission()
    {
        if (finished) return;
        finished = true;

        OutfitResult result = OutfitRating.Evaluate(equipmentSystem, coldSystem, requirements);
        ShowResult(result);

        // Останавливаем игру и не даём открыть паузу поверх результата
        var pause = FindFirstObjectByType<PauseMenu>();
        if (pause) pause.enabled = false;
        PauseMenu.IsPaused = true;
        Time.timeScale = 0f;
    }

    void ShowResult(OutfitResult result)
    {
        if (resultPanel) resultPanel.SetActive(true);
        if (scoreText) scoreText.text = result.score + " / 100";
        if (gradeText) gradeText.text = result.grade;
        if (feedbackText) feedbackText.text = "• " + string.Join("\n• ", result.feedback);

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i]) stars[i].color = i < result.stars ? starOnColor : starOffColor;
            }
        }

        Debug.Log($"Оценка экипировки: {result.score}/100 ({result.grade})");
    }

    // Кнопки панели результата
    public void Restart() => SceneLoader.LoadScene(SceneLoader.GameScene);
    public void GoToLab() => SceneLoader.LoadScene(SceneLoader.LabScene);
    public void GoToMenu() => SceneLoader.LoadScene(SceneLoader.MenuScene);
}
