using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Beginner2D
{
    /// <summary>
    /// Handle the game UI, changing health bar, enemies counter etc.
    /// </summary>
    public class UIHandler : MonoBehaviour
    {
        public static UIHandler instance { get; private set; }

        public float displayTime = 4.0f;
        private VisualElement m_NonPlayerDialogue;
        private VisualElement m_WinScreen; 
        private VisualElement m_LoseScreen;

        private Label m_RobotCounter;
        private float m_TimerDisplay;

        private VisualElement m_Healthbar;

        private Button m_ReturnToMain;
    
        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("Healthbar");
            SetHealthValue(1.0f);

            m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
            m_NonPlayerDialogue.style.display = DisplayStyle.None;
            m_TimerDisplay = -1.0f;

            m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
            m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

            m_RobotCounter = uiDocument.rootVisualElement.Q<Label>("CounterLabel");
            m_ReturnToMain = uiDocument.rootVisualElement.Q<Button>("ReturnToMain");
            
            m_ReturnToMain.clicked += () => { SceneManager.LoadScene(0); };
        }

        public void SetHealthValue(float percentage)
        {
            m_Healthbar.style.width = Length.Percent(100 * percentage);
        }

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

        public void DisplayDialogue()
        {
            m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
            m_TimerDisplay = displayTime;
        }

        public void DisplayWinScreen()
        {
            m_WinScreen.style.opacity = 1.0f;
        }

        public void DisplayLoseScreen()
        {
            m_LoseScreen.style.opacity = 1.0f;
        }
    
        public void SetCounter(int current, int enemies)
        {
            m_RobotCounter.text = $"{current} / {enemies}";
        }

    }
}