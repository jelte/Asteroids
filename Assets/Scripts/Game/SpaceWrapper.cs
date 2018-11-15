using UnityEngine;

namespace Asteroids.Game
{
    /**
     * Keep all child objects within the viewport
     **/
    public class SpaceWrapper : MonoBehaviour
    {
        #region Properties
        private Rect viewPort;

        private bool isWrappingX = false;
        private bool isWrappingY = false;
        #endregion

        #region References
        private new Camera camera;
        #endregion

        #region Methods
        private void CheckOnScreen(Transform transform)
        {
            // Determine position of the game object in the viewport
            Vector3 viewportPosition = camera.WorldToViewportPoint(transform.position);

            // Still in viewport
            if (viewPort.Contains(viewportPosition))
            {
                isWrappingX = false;
                isWrappingY = false;
                return;
            }
            
            // Check horizontal position
            if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
            {
                viewportPosition.x = viewportPosition.x > 1 ? 0 : 1;
                // Prevent horizontal rubberbanding
                isWrappingX = true;
            }
            
            // Check vertical position
            if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0))
            {
                viewportPosition.y = viewportPosition.y > 1 ? 0 : 1;
                // Prevent vertical rubberbanding
                isWrappingY = true;
            }
            
            // Teleport to corresponding position
            transform.position = camera.ViewportToWorldPoint(viewportPosition);
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            camera = Camera.main;
            viewPort.size = Vector2.one;
        }

        void FixedUpdate()
        {
            // Check if all children are still within the view port
            for (int i = 0; i < transform.childCount; i++)
            {
                CheckOnScreen(transform.GetChild(i));
            }
        }

        #endregion
    }
}
