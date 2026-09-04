using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Beginner2D
{
    /// <summary>
    /// Setup the start screen by adding clicking manipulator to each theme images, so when clicking on them it load the
    /// proper scene
    /// </summary>
    public class StartScreen : MonoBehaviour
    {
        private UIDocument m_UIDocument;

        private VisualElement m_RubySelector;
        private VisualElement m_DuckoSelector;
        private VisualElement m_CandySelector;

        private void OnEnable()
        {
            m_UIDocument = GetComponent<UIDocument>();

            m_RubySelector = m_UIDocument.rootVisualElement.Q<VisualElement>("RubySelector");
            m_DuckoSelector = m_UIDocument.rootVisualElement.Q<VisualElement>("DuckoSelector");
            m_CandySelector = m_UIDocument.rootVisualElement.Q<VisualElement>("CandySelector");
        
            m_RubySelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(1); }));
            m_DuckoSelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(2); }));
            m_CandySelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(3); }));
        }
    }
}