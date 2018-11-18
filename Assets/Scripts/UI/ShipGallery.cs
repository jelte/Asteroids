using Asteroids.Game;
using Asteroids.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.UI
{
    public class ShipGallery : MonoBehaviour {

        #region References
        [SerializeField] private AudioClip selectionSound;
        [SerializeField] private TextMesh namePrefab;
        private ShipModel[] models;
        private Tuple<ShipSelector, ShipSelector>[] selectors;
        private new Camera camera;
        private GameObject ships;
        
        private float angle;
        private bool rotating = false;
        public float distance = 10f;
        private int current;
        private int modelCount; 
        #endregion

        #region Methods
        private void Rotate(float delta)
        {
            if (rotating || delta == 0) return;
            
            rotating = true;

            float deltaSign = Mathf.Sign(delta);

            GameAsyncOperation operation = Shared.Animator.Rotate(camera.transform, Vector3.up, angle * deltaSign, 1);
            operation.completed += delegate () {
                // finished rotating
                rotating = false;

                current += (int) deltaSign;
                current = (modelCount + current) % modelCount;
            };
        }

        private void Select()
        {
            if (rotating) return;

            // Disable controls
            InputManager.Instance.OnHorizontalAxis -= Rotate;
            InputManager.Instance.OnSubmit -= Select;

            Launch(ships.transform.GetChild(current), current);
        }
        
        IEnumerator Turn(Quaternion start, Quaternion target)
        {
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                camera.transform.localRotation = Quaternion.Slerp(start, target, timer);
                yield return new WaitForEndOfFrame();
            }
            rotating = false;
        }

        void Launch(Transform ship, int index)
        {
            GameAsyncOperation rotation = Shared.Animator.Rotate(ship, Vector3.up, 180f, 1f);
            rotation.completed += delegate () {
                AudioManager.Play(selectionSound);
                Engine[] engines = ship.GetComponentsInChildren<Engine>();
                GameAsyncOperation startEngines = Shared.Animator.Do(ship, delegate ()
                {
                    foreach (Engine engine in engines) engine.StartEngine();
                }, 1f);
                startEngines.completed += delegate ()
                {
                    GameAsyncOperation rotation2 = Shared.Animator.Rotate(ship, Vector3.left, 45f, 1f);
                    GameAsyncOperation movement = Shared.Animator.Move(ship, (ship.forward + ship.up) * 25f, 1f);

                    movement.completed += delegate ()
                    {
                        FindObjectOfType<GameManager>().LoadGame(models[index]);
                    };
                };
            };
        }

        void RotateTo(int index)
        {
            if (index == current) return;
            
            int delta = current < index ? 1 : -1;
            if (index == 0 && current == modelCount - 1) delta = 1;
            if (index == 8 && current == 0) delta = -1;

            Rotate(delta);
        }

        private void Select(int index)
        {
            if (rotating) return;
            if (index == current) Select();
            else RotateTo(index);
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            camera = GetComponentInChildren<Camera>();
            ships = new GameObject("Ship");
            ships.transform.parent = transform;
            ships.transform.localPosition = Vector3.zero;
        }

        void Start()
        {
            // Load all models
            models = Resources.LoadAll<ShipModel>("Ships");

            current = 0;
            modelCount = models.Length;
            angle = 360f / modelCount;

            selectors = new Tuple<ShipSelector, ShipSelector>[modelCount];
            // render all models
            for (int i = 0; i < modelCount; i++)
            {
                // Render the model
                ShipModel instance = Instantiate(models[i], ships.transform);
                // Render the name
                TextMesh name = Instantiate(namePrefab, instance.transform);
                name.text = models[i].name;

                ShipSelector instanceSelector = instance.gameObject.AddComponent<ShipSelector>();
                instanceSelector.index = i;
                ShipSelector nameSelector = name.gameObject.AddComponent<ShipSelector>();
                nameSelector.index = i;

                instanceSelector.OnEnter += RotateTo;
                instanceSelector.OnSelect += Select;
                nameSelector.OnEnter += RotateTo;
                nameSelector.OnSelect += Select;

                instance.transform.localPosition = Quaternion.Euler(0, angle * i, 0) * (Vector3.forward * distance);
                instance.transform.LookAt(ships.transform);

                name.transform.parent = transform;
            }

            // Set up key listeners.
            InputManager.Instance.OnHorizontalAxis += Rotate;
            InputManager.Instance.OnSubmit += Select;
        }
        #endregion
    }
}
