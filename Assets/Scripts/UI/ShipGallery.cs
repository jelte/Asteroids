using Asteroids.Game;
using Asteroids.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{

    public class ShipGallery : MonoBehaviour {

        #region References
        [SerializeField] private AudioClip selectionSound;
        private ShipModel[] models;
        private new Camera camera;
        private GameObject ships;
        
        private float angle;
        private bool rotating = false;
        public float distance = 10f;
        #endregion

        #region Methods
        private void Rotate(float delta)
        {
            if (rotating || delta == 0) return;
            
            rotating = true;
            GameAsyncOperation operation = Shared.Animator.Rotate(camera.transform, Vector3.up, angle * Mathf.Sign(delta), 1);
            operation.completed += delegate () { rotating = false; };
        }

        private void Select()
        {
            InputManager.Instance.OnHorizontalAxis -= Rotate;
            InputManager.Instance.OnSubmit -= Select;
            InputManager.Instance.OnFire -= Select;

            Vector3 targetPosition = camera.transform.localRotation * (Vector3.forward * distance);
            for (int i = ships.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = ships.transform.GetChild(i);
                if (Vector3.Distance(child.localPosition, targetPosition) < .1f)
                {
                    Launch(child, (models.Length-1)-i);
                    break;
                }
            }
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
            
            int modelCount = models.Length;
            angle = 360f / modelCount;

            // render all models
            for (int i = modelCount - 1; i >= 0; i--)
            {
                ShipModel instance = Instantiate(models[i], ships.transform);
                
                instance.transform.localPosition = Quaternion.Euler(0, angle * i, 0) * (Vector3.forward * distance);
                instance.transform.LookAt(ships.transform);
            }

            // Set up key listeners.
            InputManager.Instance.OnHorizontalAxis += Rotate;
            InputManager.Instance.OnSubmit += Select;
            InputManager.Instance.OnFire += Select;
        }
        #endregion
    }
}
