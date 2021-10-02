using System;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Wizard;

namespace Spells
{
    public class Fireball : SpellBase
    {
        [SerializeField] private float _impulseStrength = 100f;
        [SerializeField] private VisualEffect _vfx;

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

        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable == null)
                return;

            damageable.OnDamage(Data.Damage);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}