using System;
using UnityEngine;

namespace Wizard.Spells
{
    public class SpellBase : MonoBehaviour
    {
        [SerializeField] protected SpellData Data;
        [SerializeField] protected bool _isFromPlayer;
        protected Rigidbody2D RigidBody;

        protected virtual void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();
        }

        public virtual void Launch(Vector2 _target, bool isFromPlayer)
        {
            OnLaunch();
        }

        protected virtual void OnLaunch()
        {
            //Play sound here
        }

        public void Destroy()
        {
            OnDestroy();
        }
        protected virtual void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}