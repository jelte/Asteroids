using Asteroids.Shared;
using Asteroids.Game;
using UnityEngine;

namespace Asteroids.UI
{
    public class ShipSelection : MonoBehaviour
    {
        #region Properties
        private int highlight = -99;

        private float timer = 0f;
        private float inputDelay = .5f;
        #endregion

        #region References
        [Tooltip("Sound to play when a ship is selected.")]
        public AudioClip selectionSound;

        // List of all ship models
        private ShipModel[] models;
        // References to all selectors
        private ShipSelector[] selectors;

        private GameAsyncOperation operation;

        // Which ship is highlighted
        protected int Highlight
        {
            get { return highlight < -1 ? -1 : highlight; }
            set
            {
                if (highlight != -99)
                {
                    selectors[highlight].StopHighlight();
                }
                highlight = highlight == -99 && value == -2 ? -1 : value;

                if (highlight == -99) return;

                if (highlight < 0)
                {
                    highlight += selectors.Length;
                }
                highlight %= selectors.Length;
                selectors[highlight].Highlight();
                timer = inputDelay;
            }
        }
        #endregion

        #region Methods
        #region Keyboard handlers
        /**
         * Move the selection vertically
         **/
        private void VerticalStep(float value)
        {
            if (timer > 0) return;
            if (value > 0) Highlight -= 3;
            if (value < 0) Highlight += 3;
        }

        /**
         * Move the selection horizontally
         **/
        private void HorizontalStep(float value)
        {
            if (timer > 0) return;
            if (value > 0) Highlight += 1;
            if (value < 0) Highlight -= 1;
        }

        /**
         * Select the highlighted ship
         **/
        void OnSelect()
        {
            if (highlight == -99) return;

            Select(highlight);
        }
        #endregion
        
        /**
         * Select the ship directly by index
         **/
        void Select(int index)
        {
            if (operation != null) return;
                        
            // Play the sound 
            operation = AudioManager.PlayAndWait(selectionSound);
            // When the sound is finished played select the ship
            operation.completed += delegate ()
            {
                FindObjectOfType<GameManager>().LoadGame(models[index]);
            };
        }

        void RenderModel(ShipModel model)
        {
            // determined the number of the model.
            int index = transform.childCount;

            // determine the position.
            Vector3 position = new Vector3(index % 3, 0, Mathf.Floor(((models.Length - 1) - index) / 3));

            // render the model.
            ShipModel instance = Instantiate(model, transform);

            // set position.
            instance.transform.localPosition = position * 5;

            // Set up mouse listeners.
            ShipSelector shipSelector = instance.gameObject.AddComponent<ShipSelector>();
            shipSelector.OnSelect += delegate () {
                Select(index);
            };
            shipSelector.OnEnter += delegate () {
                Highlight = index;
            };
            shipSelector.OnExit += delegate () {
                if (highlight == index)
                {
                    Highlight = -99;
                }
            };

            // store reference.
            selectors[index] = shipSelector;
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            // Load all models
            models = Resources.LoadAll<ShipModel>("Ships");

            // Initialize selectors array.
            selectors = new ShipSelector[models.Length];

            // render all models
            foreach (ShipModel model in models)
            {
                RenderModel(model);
            }

            // Set up key listeners.
            InputManager inputManager = FindObjectOfType<InputManager>();
            inputManager.OnHorizontalAxis += HorizontalStep;
            inputManager.OnVerticalAxis += VerticalStep;
            inputManager.OnSubmit += OnSelect;
        }

        void Update()
        {
            if (timer > 0) timer -= Time.deltaTime;
        }

        void OnDestroy()
        {
            // Clean up the key listeners
            InputManager.Instance.OnHorizontalAxis -= HorizontalStep;
            InputManager.Instance.OnVerticalAxis -= VerticalStep;
            InputManager.Instance.OnSubmit -= OnSelect;
        }
        #endregion

    }
}