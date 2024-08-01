using System;
using UnityEngine;

namespace Assets.Scenes.GoblinClickerScene
{
    public class PlayerController : MonoBehaviour
    {
        public event Action HitEnemyEvent;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var mousePos = Input.mousePosition;
                var ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out var hitInfo) && hitInfo.collider.CompareTag("Enemy"))
                {
                    HitEnemyEvent?.Invoke();
                }
            }
        }
    }
}
