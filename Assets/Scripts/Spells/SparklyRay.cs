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

        private int _maxBounce = 3;
        private int _bounceCount = 0;
        private Vector2 _lastFrameVelocity;

        private Transform _target;

        protected override void Awake()
        {
            base.Awake();
            _target = FindObjectOfType<PlayerController>().transform;
        }

        protected override void OnLaunch()
        {
            base.OnLaunch();

            Vector2 direction = (_target.position - transform.position);
            RigidBody.AddForce(direction.normalized * _impulseStrength, ForceMode2D.Impulse);
        }

        private void Update()
        {
            _lastFrameVelocity = RigidBody.velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground")) && UnstabilityManager.Instance.BouncyWalls && _bounceCount < _maxBounce)
            {
                _bounceCount++;
                RigidBody.AddForce(Vector2.Reflect(_lastFrameVelocity, other.contacts[0].normal).normalized * _impulseStrength, ForceMode2D.Impulse);
                return;
            }

            var damageable = other.gameObject.GetComponent<IDamageable>();

            damageable?.OnDamage(Data.Damage);
            OnDestroy();
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
