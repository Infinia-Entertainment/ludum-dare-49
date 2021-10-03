using System;
using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;
using Wizard.Unstability;

namespace Wizard
{
    public class DummyPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject _fireball;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var fb = Instantiate(_fireball, transform.position, Quaternion.identity);
                fb.GetComponent<SpellBase>().Launch();
                Physics2D.IgnoreCollision(fb.GetComponent<Collider2D>(), _collider);
                UnstabilityManager.Instance.AddUnstability(0.1f);
            }
        }
    }
}
