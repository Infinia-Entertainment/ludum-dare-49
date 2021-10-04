using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Unstability;
using Wizard.Spells;

namespace Wizard
{
    public class BounceCollider : MonoBehaviour
    {
        [SerializeField] private float _impulseStrength = 100f;
        [SerializeField] private Rigidbody2D RigidBody;
        [SerializeField] private SpellBase spell;

        private int _maxBounce = 3;
        private int _bounceCount = 0;
        private Vector2 _lastFrameVelocity;

        private void Update()
        {
            _lastFrameVelocity = RigidBody.velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") && UnstabilityManager.Instance.BouncyWalls && _bounceCount < _maxBounce)
            {
                _bounceCount++;
                RigidBody.AddForce(Vector2.Reflect(_lastFrameVelocity, other.contacts[0].normal).normalized * _impulseStrength, ForceMode2D.Impulse);
                return;
            }
            spell.Destroy();
        }
       

    }
}
