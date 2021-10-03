using System;
using UnityEngine;

namespace Wizard.Spells
{
    public class SpellBase : MonoBehaviour
    {
        [SerializeField] protected SpellData Data;
        protected Rigidbody2D RigidBody;

        protected virtual void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();
        }

        public void Launch()
        {
            OnLaunch();
        }

        protected virtual void OnLaunch()
        {
            //Play sound here
        }

        protected virtual void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}