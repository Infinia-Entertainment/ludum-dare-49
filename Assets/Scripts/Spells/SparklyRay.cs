using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Wizard.Unstability;

namespace Wizard.Spells
{
    public class SparklyRay : SpellBase
    {
        [SerializeField] private float _impulseStrength = 100f;
        [SerializeField] private VisualEffect _vfx;
        [SerializeField] private GameObject _bounceObj;

        private Transform _target;

        protected override void Awake()
        {
            base.Awake();
            _target = FindObjectOfType<PlayerController>().transform;
        }

        public override void Launch(Vector2 _target, bool isFromPlayer)
        {
            _isFromPlayer = isFromPlayer;
            var dirToTarget = _target - (Vector2)transform.position;

            // Debug.Log($"{dirToTarget.normalized} * { _impulseStrength} = {dirToTarget.normalized * _impulseStrength}");

            RigidBody.AddForce(dirToTarget.normalized * _impulseStrength, ForceMode2D.Impulse);

            OnLaunch();
        }

        protected override void OnLaunch()
        {
            base.OnLaunch();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IDamageable damageable;

            if (_isFromPlayer && other.gameObject.CompareTag("Enemy"))
            {
                damageable = other.gameObject.GetComponent<IDamageable>();
                damageable?.OnDamage(Data.Damage);
                OnDestroy();
            }
            else if (!_isFromPlayer && other.gameObject.CompareTag("Player"))
            {
                damageable = other.gameObject.GetComponent<IDamageable>();
                damageable?.OnDamage(Data.Damage);
                OnDestroy();
            }
        }

        protected override void OnDestroy()
        {
            GetComponent<Collider2D>().enabled = false;
            RigidBody.velocity = Vector2.zero;

            _vfx.SendEvent("OnDestroy");
            Destroy(gameObject, 1f);
        }
    }
}
