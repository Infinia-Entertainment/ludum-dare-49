using System;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Wizard;
using Wizard.Unstability;

namespace Wizard.Spells
{
    public class Fireball : SpellBase
    {
        [SerializeField] private float _impulseStrength = 100f;
        [SerializeField] private VisualEffect _vfx;

        protected override void Awake()
        {
            base.Awake();
        }
        public override void Launch(Vector2 _target, bool isFromPlayer)
        {
            _isFromPlayer = isFromPlayer;
            var dirToTarget = _target - (Vector2)transform.position;
            RigidBody.AddForce(dirToTarget.normalized * _impulseStrength, ForceMode2D.Impulse);
            _vfx.SetVector3("Velocity", -dirToTarget);

            OnLaunch();
        }

        protected void OnLaunch()
        {
            base.OnLaunch();

        }

        private void Update()
        {
            _vfx.SetVector3("Velocity", -RigidBody.velocity);
            _vfx.SetVector3("BallPosition", transform.position);
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

            _vfx.SendEvent("OnDestroy");
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}