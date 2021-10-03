using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; set; }

        public int EnemyCount;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;
        }
        
    }
}
