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

[SerializeField]
        private bool clickDisabled = false;

        void Update()
        {
            if (_next == null) return;
            

            if (!_pendingState && _next.isOn != isOn && isOn)
            {
                
                _pendingState = isOn;
                Invoke(nameof(InvokeNext), 0.2f);
            }
        }

        void InvokeNext()
        {
            _next.SwitchMode(_pendingState);
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
                if(node._ende != other)
                    _next = node;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            // Ignore CircleCollider2D
            if (other is CircleCollider2D)
                return;
            
            if (other.TryGetComponent<PipeNode>(out var node))
            {
                _next = null;
            }
        }

        private void OnMouseDown()
        {
            if (clickDisabled) return;
            
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