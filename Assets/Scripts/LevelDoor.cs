using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard
{
    public class LevelDoor : MonoBehaviour
    {
        [SerializeField] private int _killsRequired;

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.EnemiesKilled >= _killsRequired) gameObject.SetActive(false);
        }
    }
}
