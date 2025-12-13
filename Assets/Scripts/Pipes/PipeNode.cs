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


        public void SwitchMode(bool isOn)
        {
            _on.SetActive(isOn); //when true turn on
            _off.SetActive(!isOn); //when true turn off
        }

        

    }
}