using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Wizard
{
    public class ForceShield : MonoBehaviour, IDamageable
    {
        private int _currentHealth;
        private int _maxHealth = 1;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        private bool isShieldDown = false;
        public float shieldRegenCooldown = 3;
        private VisualEffect _vfx;

        public void Awake()
        {
            _currentHealth = _maxHealth;
            _vfx = GetComponentInChildren<VisualEffect>();
        }
        public void OnDamage(int damage)
        {

        }
        public int ProcessShield(int damage)
        {
            if (isShieldDown) return damage;
            else
            {
                _currentHealth -= damage;
                if (_currentHealth <= 0) OnDeath();
                return 0;
            }

        }
        public void OnDeath()
        {
            isShieldDown = true;
            _vfx.enabled = false;
            Invoke("ResetShield", shieldRegenCooldown);
        }
        private void ResetShield()
        {
            _currentHealth = _maxHealth;
            _vfx.enabled = true;
            isShieldDown = false;
        }
    }
}
