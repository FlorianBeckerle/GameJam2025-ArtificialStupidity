using System.Collections;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Pipes
{
    public class PipeNode: MonoBehaviour
    {
        
        [SerializeField]
        public Vector2 nodePosition;

        [SerializeField]
        private GameObject _on;
        [SerializeField]
        private GameObject _off;
        
        [SerializeField]
        private Collider2D _ende;

        [SerializeField] private bool _isEnd;

        [SerializeField] private bool isOn = false;

        [SerializeField] private PipeNode _next;
        private bool _pendingState;
        private PipeNode lastNext;

[SerializeField]
        private bool clickDisabled = false;
        
        void Update()
        {
            if (_next == null) return;
            
            //if state still needs to be changed
            if (_pendingState || isOn != _next.isOn)
            {
                
                Invoke(nameof(InvokeNext), 0.2f);
            }
            
            //Disable Script if aready in right place and next has been activated
            if(isOn && _next.isOn) this.enabled = false;
        }

        void InvokeNext()
        {
            if (_next == null) return;
            if (isOn)
            {
                _next.SwitchMode(isOn);    
            }
            else
            {
                if (lastNext == null) return;
                lastNext.SwitchMode(isOn);
                lastNext = null;
            }
            _pendingState = false;
        }

        public void SwitchMode(bool isOn)
        {
            if (!_isEnd)
            {
                _on.SetActive(isOn); //when true turn on
                _off.SetActive(!isOn); //when true turn off    
            }

            this.isOn = isOn;
            if (_isEnd)
            {
                PerformAction();
            }
        }

        private void PerformAction()
        {
            if (!isOn) return;

            var manager = GetComponentInParent<PipeManager>();
            if (manager != null)
            {
                manager.Solve();
            }


        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore CircleCollider2D
            if (other is CircleCollider2D)
                return;
            
            if (other.TryGetComponent<PipeNode>(out var node))
            {
                //Only set node if it touches the others start
                if (node._ende != other)
                {

                    _next = node; //set current node
                    _pendingState = true; //set buffer to true
                    lastNext = node; //set buffer for next
                }

            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            // Ignore CircleCollider2D
            if (other is CircleCollider2D)
                return;
            
            //Get other collider
            if (other.TryGetComponent<PipeNode>(out var node))
            {
                //Set deactivate next
                _pendingState = true;
                //clear next
                _next = null;
            }
        }

        private void OnMouseDown()
        {
            //If clickDIsabled or script is thisabled skip
            if (clickDisabled || !this.enabled) return;
            
            RotatePipe();
            StartCoroutine(DisableClick());
        }

        private void RotatePipe()
        {
            float z = transform.eulerAngles.z;
            z = (z - 90f) % 360f;
            transform.rotation = Quaternion.Euler(0f, 0f, z);
        }

        private IEnumerator DisableClick()
        {
            this.clickDisabled = true;
            yield return new WaitForSecondsRealtime(0.5f);
            this.clickDisabled = false;
        }

    }
}