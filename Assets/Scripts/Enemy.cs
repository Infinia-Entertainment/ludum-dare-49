using UnityEngine;
using UnityEngine.Serialization;

namespace Wizard
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        private int _currentHealth;
        [FormerlySerializedAs("_health")][SerializeField]private int _maxHealth = 1;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        public void OnDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                OnDeath();
            }
        }


        public void OnDeath()
        {

            GameManager.Instance.EnemyCount--;
            GameManager.Instance.EnemiesKilled++;
            Destroy(gameObject);
        }
    }
}