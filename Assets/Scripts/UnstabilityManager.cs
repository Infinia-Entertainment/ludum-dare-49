using System;
using UnityEngine;

namespace Wizard
{
    public class UnstabilityManager : MonoBehaviour
    {
        public static UnstabilityManager Instance { get; set; }

        public bool BouncyWalls;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;
        }
        
    }
}