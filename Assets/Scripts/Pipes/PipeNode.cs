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

        bool isPropagating;

        void Update()
        {
            if (_next == null) return;

            if (!isPropagating && _next.isOn != isOn)
            {
                isPropagating = true;
                _pendingState = isOn;
                Invoke(nameof(InvokeNext), 1f);
            }
        }

        void InvokeNext()
        {
            _next.SwitchMode(_pendingState);
            isPropagating = false;
        }

        public void SwitchMode(bool isOn)
        {
            _on.SetActive(isOn); //when true turn on
            _off.SetActive(!isOn); //when true turn off
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
                _next = node;
            }

            if (_next._ende == other)
            {
                _next = null;
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