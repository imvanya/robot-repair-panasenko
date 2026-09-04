using System;
using UnityEngine;

namespace Beginner2D
{
    /// <summary>
    /// This class define a NonPlayerCharacter. It is only used to "mark" a GameObject as NPC, the raycast for dialog in
    /// the player controller will check if the object hit have that script to define if it can be talked to.
    /// </summary>
    public class NonPlayerCharacter : MonoBehaviour
    {
        public GameObject dialogueBubble;

        private void Awake()
        {
            //DEMO purpose only. In the tutorial, this is set in the inspector on the prefab, but to keep the layer
            //settings empty, we have to do it dynamically in the demo
            Helpers.RecursiveLayerSet(transform, Helpers.NPCLayer);
        }

        void Start()
        {
            dialogueBubble.SetActive(false);
        }
    }
}