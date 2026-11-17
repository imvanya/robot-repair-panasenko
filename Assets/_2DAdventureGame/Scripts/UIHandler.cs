using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    // НОВЕ (крок 12): поля репліки
    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private float m_TimerDisplay;

    private VisualElement m_Healthbar;

    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;
    private Label m_RobotCounter;
    private int m_RepairedRobots;
    private int m_TotalRobots;
    private Button m_RestartButton;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        m_Healthbar = uiDocument.rootVisualElement
            .Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        // НОВЕ (крок 12): ховаємо репліку на старті
        m_NonPlayerDialogue = uiDocument.rootVisualElement
            .Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        m_LoseScreen = uiDocument.rootVisualElement
            .Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = uiDocument.rootVisualElement
            .Q<VisualElement>("WinScreenContainer");
        m_RobotCounter = uiDocument.rootVisualElement.Q<Label>("CounterLabel");
        m_RestartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        if (m_RestartButton != null)
        {
            m_RestartButton.style.display = DisplayStyle.None;
            m_RestartButton.clicked += RestartLevel;
        }
        SetCounter(m_RepairedRobots, m_TotalRobots);
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    // НОВЕ (крок 12): таймер показу репліки
    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }

    }

    // НОВЕ (крок 12): показати репліку
    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
        ShowRestartButton();
    }

    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
        ShowRestartButton();
    }

    private void ShowRestartButton()
    {
        if (m_RestartButton != null)
            m_RestartButton.style.display = DisplayStyle.Flex;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetCounter(int repaired, int total)
    {
        m_RepairedRobots = repaired;
        m_TotalRobots = total;
        if (m_RobotCounter != null)
            m_RobotCounter.text = $"{repaired}/{total}";
    }

}
