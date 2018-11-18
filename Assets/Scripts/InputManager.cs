using Asteroids.Shared.Inputs;
using System;
using UnityEngine;

namespace Asteroids
{
    public class InputManager : MonoBehaviour
    {
        #region Static Properties
        public static IInputManager Instance { get; private set; }
        #endregion

        #region References
        IInputManager inputManager;
        #endregion

        #region Unity Methods
        void Awake()
        {
            // ensure there is only 1 input manager.
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            // Set the value
#if UNITY_ANDROID
            Instance = gameObject.AddComponent<MobileInputManager>();
#endif
#if UNITY_STANDALONE || UNITY_EDITOR
            Instance = gameObject.AddComponent<KeyboardInputManager>();
#endif
            // Persist object across scenes
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}
