using System;
using UnityEngine;

namespace Assets.Scenes.DoozyCatcherScene
{
    public class PlayerController : MonoBehaviour
    {
        public event Action EnemyCaught;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
                EnemyCaught?.Invoke();
            }
        }
    }
}