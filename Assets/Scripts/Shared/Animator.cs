using System;
using System.Collections;
using UnityEngine;

namespace Asteroids.Shared
{
    public class Animator : MonoBehaviour
    {
        public static Animator Instance { get; private set; }

        public static GameAsyncOperation Rotate(Transform subject, Vector3 axis, float angle, float time)
        {
            GameAsyncOperation action = new GameAsyncOperation();

            Instance.StartCoroutine(Instance.DoRotate(subject, axis, angle, time, action));

            return action;
        }

        public static GameAsyncOperation Move(Transform subject, Vector3 movement, float time)
        {
            GameAsyncOperation action = new GameAsyncOperation();

            Instance.StartCoroutine(Instance.DoMove(subject, movement, time, action));

            return action;
        }

        public static GameAsyncOperation Do(Transform subject, Action delegated, float time)
        {
            GameAsyncOperation action = new GameAsyncOperation();

            Instance.StartCoroutine(Instance.DoAction(subject, delegated, time, action));

            return action;
        }

        private IEnumerator DoAction(Transform subject, Action delegated, float time, GameAsyncOperation action)
        {
            delegated.Invoke();
            yield return new WaitForSeconds(time);

            action.Done();
        }

        private IEnumerator DoRotate(Transform subject, Vector3 axis, float angle, float time, GameAsyncOperation action)
        {
            float timer = 0;
            Quaternion startRotation = subject.localRotation;
            Quaternion targetRotation = Quaternion.Euler(subject.localEulerAngles + Quaternion.AngleAxis(angle, axis).eulerAngles);
            while (timer < time)
            {
                timer += Time.deltaTime;
                subject.localRotation = Quaternion.Slerp(startRotation, targetRotation, timer / time);
                yield return new WaitForEndOfFrame();
            }
            action.Done();
        }

        private IEnumerator DoMove(Transform subject, Vector3 movement, float time, GameAsyncOperation action)
        {
            float timer = 0;
            Vector3 startPosition = subject.localPosition;
            Vector3 targetPosition = startPosition + movement;
            while (timer < time)
            {
                timer += Time.deltaTime;
                subject.localPosition = Vector3.Lerp(startPosition, targetPosition, timer / time);
                yield return new WaitForEndOfFrame();
            }
            action.Done();
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
