using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

namespace Wizard
{
    public class DummyPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject _fireball;
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var fb = Instantiate(_fireball, transform.position, Quaternion.identity);
                fb.GetComponent<SpellBase>().Launch();
            }
        }
    }
}
