using UnityEngine;

namespace NTC.Global.System
{
    public abstract class MonoShortСuts : MonoBehaviour
    {
        [SerializeField] private bool debug;

        public T Get<T>() => GetComponent<T>();

        public T[] Gets<T>() => GetComponents<T>();

        public T ChildrenGet<T>() => GetComponentInChildren<T>();

        public T[] ChildrenGets<T>() => GetComponentsInChildren<T>();

        public T ParentGet<T>() => GetComponentInParent<T>();

        public T[] ParentGets<T>() => GetComponentsInParent<T>();

        public T Find<T>() where T : Object => FindObjectOfType<T>();

        public T[] Finds<T>() where T : Object => FindObjectsOfType<T>();


        protected void Log(string msg)
        {
            if (debug)
                Debug.Log(msg);
        }

        protected void LogWarning(string msg)
        {
            Debug.LogWarning(msg);
        }

        protected void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        protected void DrawLine(Vector3 from, Vector3 to)
        {
            Debug.DrawLine(from, to, Color.green);
        }
    }
}