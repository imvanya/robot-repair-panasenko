using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Beginner2D
{
    public class StartScreen : MonoBehaviour
    {
        private void OnEnable()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            Button startButton = uiDocument.rootVisualElement.Q<Button>("StartButton");
            if (startButton != null)
                startButton.clicked += StartGame;
        }

        private void OnDisable()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.rootVisualElement == null)
                return;

            Button startButton = uiDocument.rootVisualElement.Q<Button>("StartButton");
            if (startButton != null)
                startButton.clicked -= StartGame;
        }

        private void StartGame()
        {
            SceneManager.LoadScene("MyLevel");
        }
    }
}
