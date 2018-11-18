using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.UI
{
    public class MobileDetection : MonoBehaviour
    {
        public bool showOnMobile = false;

        // Use this for initialization
        void Awake()
        {
#if UNITY_ANDROID
            gameObject.SetActive(showOnMobile);
#endif
#if UNITY_STANDALONE || UNITY_EDITOR
            gameObject.SetActive(!showOnMobile);
#endif
        }
    }
}