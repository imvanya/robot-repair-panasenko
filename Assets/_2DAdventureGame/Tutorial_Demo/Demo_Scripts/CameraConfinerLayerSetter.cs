using System;
using UnityEngine;


namespace Beginner2D
{
    /// <summary>
    /// DEMO purpose only, will set the confiner layer as we need to keep the layer setting empty for the tutorial
    /// </summary>
    public class CameraConfinerLayerSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            Helpers.RecursiveLayerSet(transform, Helpers.ConfinerLayer);
        }
    }
}