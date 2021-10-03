using System;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Wizard;
using Wizard.Unstability;

namespace Spells
{
    public class Fireball : SpellBase
    {
        [SerializeField] private float _impulseStrength = 100f;
        [SerializeField] private VisualEffect _vfx;

        private int _maxBounce = 3;
        private int _bounceCount = 0;
        private Vector2 _lastFrameVelocity;

        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override void OnLaunch()
        {
            base.OnLaunch();

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            RigidBody.AddForce(direction.normalized * _impulseStrength, ForceMode2D.Impulse);
            _vfx.SetVector3("Velocity", -direction );
        }

        private void Update()
        {
            _vfx.SetVector3("Velocity", -RigidBody.velocity );
            _vfx.SetVector3("BallPosition", transform.position );
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
            _vfx.SendEvent("OnDestroy");
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}