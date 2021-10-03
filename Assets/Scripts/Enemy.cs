using UnityEngine;

namespace Wizard
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        private int _health = 1;

        public void OnDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                GameManager.Instance.EnemyCount--;
                Destroy(gameObject);
            }
        }
    }
}