using Asteroids.Game.Data;
using Asteroids.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Game
{
    /**
     * Spawns asteroids 
     **/
    public class AsteroidManager : MonoBehaviour
    {
        #region Events
        public event System.Action OnAsteroidDestroy;
        #endregion

        #region Properties
        [Tooltip("Determines the starting speed of an asteroid")]
        public float startForce;
        [Tooltip("The minimum speed an asteroid gets after it is shattered")]
        public float minSplitForce = 100f;
        [Tooltip("The maximum speed an asteroid gets after it is shattered")]
        public float maxSplitForce = 200f;

        // The absolute screensize.
        private Vector2 screenSize;
        #endregion

        #region References
        // reference to the ship
        private Ship ship;

        [Tooltip("Reference to the base asteroid wrapper")]
        public Asteroid asteroidPrefab;

        // List of all available asteroid models.
        private IDictionary<AsteroidFamily, IDictionary<AsteroidColour, List<AsteroidModel>>> models;
        // Pool of asteroids.
        private IObjectPool<Asteroid> asteroidPool;
        // List of pools for each asteroid model.
        private IDictionary<AsteroidModel, IObjectPool<AsteroidModel>> asteroidModelPools;
        #endregion

        #region Methods
        /**
         * Load the waves for a level.
         */
        public void Load(Level level)
        {
            // Schedule each wave to spawn.
            level.waves.ForEach(delegate (Wave wave)
            {
                StartCoroutine(SpawnWave(wave));
            });
        }

        IEnumerator SpawnWave(Wave wave)
        {
            // make the wave wait until it is time.
            yield return new WaitForSeconds(wave.delay);
            
            // Create asteroids at random locations
            for (int i = 0; i < wave.quantity; i++)
            {
                Vector3 position = RandomPosition();
                // Ensure some distance from the player ship if it is there.
                while(ship != null && (position - ship.Position).magnitude < screenSize.magnitude / 4)
                {
                    position = RandomPosition();   
                }

                // Spawn asteroid.
                Spawn(3, position, wave.family, wave.colour, wave.speed);
            }
        }

        /**
         * Get a random position on the playing flield
         */
        private Vector3 RandomPosition()
        {
            Vector2 position = Random.onUnitSphere;
            position.Scale(screenSize);
            return new Vector3(position.x, 0, position.y);
        }

        /**
         * Get the asteroid models associated with a family and colour.
         */
        private List<AsteroidModel> getModels(AsteroidFamily familyName, AsteroidColour colour)
        {
            // Ensure the family exists in the dictionary
            IDictionary<AsteroidColour, List<AsteroidModel>> family;
            if (!models.TryGetValue(familyName, out family))
            {
                // instantiate a new Dictionary for the new family.
                family = new Dictionary<AsteroidColour, List<AsteroidModel>>();
                // add to the models dictionary.
                models.Add(familyName, family);
            }

            // Ensure the colour exists in the dictionary
            List<AsteroidModel> asteroidModels;
            if (!models[familyName].TryGetValue(colour, out asteroidModels))
            {
                // instantiate a List for the new colour.
                asteroidModels = new List<AsteroidModel>();
                // add to the family.
                models[familyName].Add(colour, asteroidModels);
            }
            return asteroidModels;
        }

        /**
         * get the object pool for a model.
         **/
        private IObjectPool<AsteroidModel> GetAsteroidPool(AsteroidModel model)
        {
            IObjectPool<AsteroidModel> pool;
            if (!asteroidModelPools.TryGetValue(model, out pool))
            {
                pool = ObjectPoolFactory.Get(model);
                asteroidModelPools.Add(model, pool);
            }
            return pool;
        }

        // Spawn an asteroid
        private void Spawn(int size, Vector3 position, AsteroidFamily family, AsteroidColour colour, float force)
        {                            
            // Create angle
            int angle = Random.Range(0, 11);

            // Calculate direction
            Vector3 direction = Quaternion.AngleAxis(angle * 30, Vector3.up) * (Vector3.forward + Vector3.right);

            // Get a random model of the same family and colour.
            AsteroidModel model = models[family][colour][Random.Range(0, models[family][colour].Count)];

            // Create the asteroid.
            CreateAsteroid(size, position, model, direction * force);
        }

        // Creat the asteroid
        private void CreateAsteroid(int size, Vector3 position, AsteroidModel model, Vector3 direction)
        {
            // get an available asteroid instance
            Asteroid asteroid = asteroidPool.Get(position, transform);

            // get an available model instance
            AsteroidModel modelInstance = GetAsteroidPool(model).Get(Vector3.zero, asteroid.transform);

            // update size and direction
            asteroid.Init(size, direction);

            // define event listeners
            // Notify observers that the asteroid has been destroyed.
            System.Action<Asteroid> notifyAction = delegate (Asteroid instance) {
                OnAsteroidDestroy?.Invoke();
            };
            // Spawn new smaller asteroids in its place.
            System.Action<Asteroid> splitAction = delegate (Asteroid instance) {
                float speed = asteroid.Velocity;
                Spawn(size - 1, asteroid.transform.position, model.family, model.colour, speed + Random.Range(minSplitForce, maxSplitForce));
                Spawn(size - 1, asteroid.transform.position, model.family, model.colour, speed + Random.Range(minSplitForce, maxSplitForce));
            };
            // tear down event listeners
            System.Action<Asteroid> cleanUpAction = null;
            cleanUpAction = delegate (Asteroid instance) {
                modelInstance.Detach();
                asteroid.OnRemove -= notifyAction;
                asteroid.OnRemove -= cleanUpAction;
                if (size > 1) asteroid.OnRemove -= splitAction;
            };

            // set up event listeners
            asteroid.OnRemove += notifyAction;
            if (size > 1)
            {
                asteroid.OnRemove += splitAction;
            }
            asteroid.OnRemove += cleanUpAction;   
        }        
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // Setup the asteroid pool
            asteroidPool = ObjectPoolFactory.Get(asteroidPrefab);
            // setup the model pool dictionary
            asteroidModelPools = new Dictionary<AsteroidModel, IObjectPool<AsteroidModel>>();
        }

        void Start()
        {
            ship = FindObjectOfType<Ship>();

            // Determine the screen size
            Vector3 size = Camera.main.ViewportToWorldPoint(Vector2.zero);
            screenSize = new Vector2(size.x, size.z) * 2;

            // Initialize Dictionary
            models = new Dictionary<AsteroidFamily, IDictionary<AsteroidColour, List<AsteroidModel>>>();

            // Load all asteroid models from resources
            AsteroidModel[] resources = Resources.LoadAll<AsteroidModel>("Asteroids");

            // index the models
            foreach (AsteroidModel model in resources)
            {
                getModels(model.family, model.colour).Add(model);
            }
        }
        #endregion
    }
}