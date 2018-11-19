using Asteroids.Game;
using Asteroids.Shared;
using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.CommandBus;
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
        private new Camera camera;
        private Transform ships;
        private Transform names;

        private float angle;
        private bool selecting = false;
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

            Rotate command = new Rotate(camera.transform, Vector3.up, angle * deltaSign);
            command.completed += delegate () {
                // finished rotating
                rotating = false;

                current += (int) deltaSign;
                current = (modelCount + current) % modelCount;
            };
            Bus.Execute(command);
        }

        private void Select()
        {
            if (rotating || selecting) return;

            selecting = true;

            // Disable controls
            InputManager.Instance.OnHorizontalAxis -= Rotate;
            InputManager.Instance.OnSubmit -= Select;

            Launch(ships.GetChild(current), current);
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
            Rotate shipRotation = new Rotate(ship, Vector3.up, 180f);
            shipRotation.completed += delegate () {
                Bus.Execute(new Play(selectionSound));
                Engine[] engines = ship.GetComponentsInChildren<Engine>();
                Delegation startEngine = new Delegation(delegate ()
                {
                    foreach (Engine engine in engines) engine.StartEngine();
                }, 0.75f);
                startEngine.completed += delegate ()
                {
                    Rotate shipVerticalRotation = new Rotate(ship, Vector3.left, 45f);
                    Move shipMovement = new Move(ship, (ship.forward + ship.up) * 25f);
                    shipMovement.completed += delegate ()
                    {
                        FindObjectOfType<GameManager>().LoadGame(models[index]);
                    };
                    
                    Bus.Execute(shipVerticalRotation, shipMovement);
                };
                Bus.Execute(startEngine);
            };
            Bus.Execute(shipRotation);
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
            ships = new GameObject("-- Ships").transform;
            ships.parent = transform;
            ships.localPosition = Vector3.zero;
            names = new GameObject("-- Names").transform;
            names.parent = transform;
            names.localPosition = Vector3.zero;
        }

        void Start()
        {
            // Load all models
            models = Resources.LoadAll<ShipModel>("Ships");

            current = 0;
            modelCount = models.Length;
            angle = 360f / modelCount;
            
            // render all models
            for (int i = 0; i < modelCount; i++)
            {
                // Render the model
                ShipModel instance = Instantiate(models[i], ships);
                // Render the name
                TextMesh name = Instantiate(namePrefab, instance.transform);
                name.text = models[i].name;

                ShipSelector instanceSelector = instance.gameObject.AddComponent<ShipSelector>();
                ShipSelector nameSelector = name.gameObject.AddComponent<ShipSelector>();
                
                // bind event listeners
                instanceSelector.OnSelect += Select;
                nameSelector.OnSelect += Select;
                
#if UNITY_ANDROID	
                instanceSelector.OnSwipe += Rotate;
                nameSelector.OnSwipe += Rotate;
#else
                instanceSelector.OnEnter += RotateTo;
                nameSelector.OnEnter += RotateTo;
#endif

                // move away from center
                instance.transform.localPosition = Quaternion.Euler(0, angle * i, 0) * (Vector3.forward * distance);

                // rotate towards center
                instance.transform.LookAt(ships);

                // Disconnect name from model (so it does not get animated'
                name.transform.parent = names;
            }

            // Set up key listeners.
            InputManager.Instance.OnHorizontalAxis += Rotate;
            InputManager.Instance.OnSubmit += Select;
        }
#endregion
    }
}
