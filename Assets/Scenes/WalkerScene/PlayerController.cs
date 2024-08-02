using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.WalkerScene
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