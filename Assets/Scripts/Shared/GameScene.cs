using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids.Shared

{
    /**
     * Internal representation of a scene.
     **/
    internal class GameScene
    {
        // Name of the scene
        public string name;
        // build Index of the scene
        private int? index = null;
        // is the scene loaded.
        private bool isLoaded = false;

        public GameScene(string name)
        {
            this.name = name;
        }
        
        /**
         * Get the build index of the scene.
         **/
        public int Index
        {
            get
            {
                if (index == null)
                {
                    index = SceneUtility.GetBuildIndexByScenePath(name);
                }
                return (int) index;
            }
        }

        /**
         * Toggle the scene additively
         **/
        public AsyncOperation Toggle()
        {
            if (isLoaded)
            {
                return Unload();
            }
            
            return Load(LoadSceneMode.Additive);
        }

        /**
         * Load and replace the  scene 
         **/
        public AsyncOperation Load()
        {
            return Load(LoadSceneMode.Single);
        }

        /**
         * Load the scene
         **/
        public AsyncOperation Load(LoadSceneMode mode)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(Index, mode);
            
            operation.completed += delegate(AsyncOperation op) {
                isLoaded = true;
            };

            return operation;
        }

        /**
         * Unload the scene
         **/
        public AsyncOperation Unload()
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(Index);
            
            operation.completed += delegate (AsyncOperation op) {
                isLoaded = false;
            };

            return operation;
        }
    }
}
