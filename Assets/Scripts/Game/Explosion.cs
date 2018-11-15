using System.Collections;
using UnityEngine;

namespace Asteroids.Game
{
    public class Explosion : MonoBehaviour
    {
        #region Properties
        [Tooltip("How long does the explosion last?")]
        public float explosionTime = 4f;
        #endregion

        #region Methods
        IEnumerator CleanUp()
        {
            yield return new WaitForSeconds(explosionTime);
            Destroy(gameObject);
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            StartCoroutine(CleanUp());
        }
        #endregion
    }
}