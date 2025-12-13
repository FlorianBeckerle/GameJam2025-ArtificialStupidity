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
        
        
        


        public void SwitchMode(bool isOn)
        {
            _on.SetActive(isOn); //when true turn on
            _off.SetActive(!isOn); //when true turn off

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
            other.GetComponent<PipeNode>().SwitchMode(true);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            other.GetComponent<PipeNode>().SwitchMode(false);
        }
        

    }
}