using UnityEngine;
using Wizard.Spells;
using Wizard.Unstability;

namespace Wizard
{
    public class DummyPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject _fireball;
        [SerializeField] private GameObject _sparkleSpell;
        [SerializeField] private Transform _spellFirePoint;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var fb = Instantiate(_fireball, _spellFirePoint.position, Quaternion.identity);
                fb.GetComponent<Fireball>().Launch(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);


                Physics2D.IgnoreCollision(fb.GetComponent<Collider2D>(), _collider);
                UnstabilityManager.Instance.AddUnstability(0.1f);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                var fb = Instantiate(_sparkleSpell, _spellFirePoint.position, Quaternion.identity);
                fb.GetComponent<SparklyRay>().Launch(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
                Physics2D.IgnoreCollision(fb.GetComponent<Collider2D>(), _collider);
                UnstabilityManager.Instance.AddUnstability(0.1f);
            }
        }
    }
}
