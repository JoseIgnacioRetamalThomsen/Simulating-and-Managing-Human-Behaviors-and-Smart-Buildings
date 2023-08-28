using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HyperBuilding
{
    public class MainLed : MonoBehaviour
    {

        private LEDNode _ledNode;

        public Light _pointLight;

        // Start is called before the first frame update
        void Start()
        {
            _ledNode = GetComponentInChildren<LEDNode>();
            _pointLight = GetComponentInChildren<Light>();
            _pointLight.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void TURN_ON()
        {
            _ledNode.turnOn();
            _pointLight.gameObject.SetActive(true);
        }

        public void TURN_OFF()
        {
            _ledNode.turnOff();
            _pointLight.gameObject.SetActive(false);
        }
    }
}