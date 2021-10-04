using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Spells;
using Random = UnityEngine.Random;

namespace Wizard
{
    public class BasicFireSpellAi : MonoBehaviour
    {
        [SerializeField] private GameObject _spell;

        private float _minTimeBetweenSpell = 2.5f;
        private float _maxTimeBetweenSpell = 5f;

        private float _nextSpell;

        private void Awake()
        {
            _nextSpell = Random.Range(_minTimeBetweenSpell, _maxTimeBetweenSpell);
        }

        private void Update()
        {
            if (Time.time > _nextSpell)
            {
                FireSpell();
                _nextSpell = Time.time + Random.Range(_minTimeBetweenSpell, _maxTimeBetweenSpell);
            }
        }

        private void FireSpell()
        {
            var spell = Instantiate(_spell, transform.position, Quaternion.identity);
            //spell.GetComponent<SpellBase>().Launch();
        }
    }
}