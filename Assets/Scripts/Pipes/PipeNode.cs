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

        void Update()
        {
            if (_next == null) return;
            

            if (!_pendingState && _next.isOn != isOn && isOn)
            {
                
                _pendingState = isOn;
                Invoke(nameof(InvokeNext), 1f);
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
            Debug.Log($"Node: {nodePosition}, reached Goal!");
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PipeNode>(out var node))
            {
                //Only set node if it touches the others start
                if(node._ende != other)
                    _next = node;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<PipeNode>(out var node))
            {
                _next = null;
            }
        }

        

    }
}