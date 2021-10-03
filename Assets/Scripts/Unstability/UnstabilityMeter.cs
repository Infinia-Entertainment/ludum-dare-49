using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wizard.Unstability
{
    public class UnstabilityMeter : MonoBehaviour
    {
        [SerializeField] private Image _fill;

        public void UpdateUnstability(float percentage)
        {
            _fill.fillAmount = percentage;
        }
    }
}
