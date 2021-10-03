using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]private GameObject _enemyPrefab;
        private void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), 0f, 2.5f);
        }

        private void SpawnEnemy()
        {
            if (GameManager.Instance.EnemyCount < 5)
            {
                Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
                GameManager.Instance.EnemyCount++;
            }
        }
    }
}
