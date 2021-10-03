using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Wizard.Utils;

namespace Wizard.Unstability
{
    public class UnstabilityManager : MonoBehaviour
    {
        public static UnstabilityManager Instance { get; set; }

        public bool BouncyWalls;

        [SerializeField] private UnstabilityMeter _unstabilityMeter;
        [SerializeField] private TMP_Text _eventText;
        [SerializeField] private PlayableDirector _eventTextAnimation;

        private float _currentUnstability;
        private Vector2 _defaultGravity;

        private List<UnstabilityEvent> _availableUnstabilityEvents;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;

            _defaultGravity = Physics2D.gravity;
            _availableUnstabilityEvents = Enum.GetValues(typeof(UnstabilityEvent)).Cast<UnstabilityEvent>().ToList();
        }

        public void AddUnstability(float percentage)
        {
            _currentUnstability += percentage;
            _unstabilityMeter.UpdateUnstability(_currentUnstability);

            if (_currentUnstability >= 1f)
            {
                ChooseRandomUnstabilityEvent();
                    _currentUnstability = 0f;
                
            }    
        }

        private void ChooseRandomUnstabilityEvent()
        {
            if (_availableUnstabilityEvents.Count > 0f)
            {
                switch (_availableUnstabilityEvents.TakeRandom())
                {
                    case UnstabilityEvent.BouncyWall:
                        _eventText.text = "Bouncy Walls";
                        _eventTextAnimation.Play();
                        StartCoroutine(BouncyWallEvent());
                        break;
                    
                    case UnstabilityEvent.LowGravity:
                        _eventText.text = "Low Gravity";
                        _eventTextAnimation.Play();
                        StartCoroutine(LowGravityEvent());
                        break;
                    
                    case  UnstabilityEvent.HighGravity:
                        _eventText.text = "High Gravity";
                        _eventTextAnimation.Play();
                        StartCoroutine(HighGravityEvent());
                        break;
                    
                    case  UnstabilityEvent.SlowTime:
                        _eventText.text = "Slower";
                        _eventTextAnimation.Play();
                        StartCoroutine(SlowTimeEvent());
                        break;
                    
                    case  UnstabilityEvent.FastTime:
                        _eventText.text = "Faster";
                        _eventTextAnimation.Play();
                        StartCoroutine(FastTimeEvent());
                        break;
                }
            }
        }

        private IEnumerator BouncyWallEvent()
        {
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.BouncyWall);
            BouncyWalls = true;
            yield return new WaitForSeconds(30f);
            BouncyWalls = false;
            _availableUnstabilityEvents.Add(UnstabilityEvent.BouncyWall);
        }
        
        private IEnumerator LowGravityEvent()
        {
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.LowGravity);
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.HighGravity);
            Physics2D.gravity = new Vector2(0, -3f);
            yield return new WaitForSeconds(30f);
            Physics2D.gravity = _defaultGravity;
            _availableUnstabilityEvents.Add(UnstabilityEvent.LowGravity);
            _availableUnstabilityEvents.Add(UnstabilityEvent.HighGravity);
        }
        
        private IEnumerator HighGravityEvent()
        {
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.LowGravity);
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.HighGravity);
            Physics2D.gravity = new Vector2(0, -12f);         
            yield return new WaitForSeconds(30f);
            Physics2D.gravity = _defaultGravity;        
            _availableUnstabilityEvents.Add(UnstabilityEvent.LowGravity);
            _availableUnstabilityEvents.Add(UnstabilityEvent.HighGravity);        }
        
        private IEnumerator SlowTimeEvent()
        {
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.SlowTime);
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.FastTime);
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(30f);
            Time.timeScale = 1f;     
            _availableUnstabilityEvents.Add(UnstabilityEvent.SlowTime);
            _availableUnstabilityEvents.Add(UnstabilityEvent.FastTime);        }
        
        private IEnumerator FastTimeEvent()
        {
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.SlowTime);
            _availableUnstabilityEvents.RemoveAll(e => e == UnstabilityEvent.FastTime);
            Time.timeScale = 1.5f;        
            yield return new WaitForSeconds(30f);
            Time.timeScale = 1f;
            _availableUnstabilityEvents.Add(UnstabilityEvent.SlowTime);
            _availableUnstabilityEvents.Add(UnstabilityEvent.FastTime);        }
    }
    
    
}