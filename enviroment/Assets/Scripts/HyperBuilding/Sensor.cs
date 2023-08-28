using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperBuilding
{
    public class Sensor : MonoBehaviour
    {
        [SerializeField] private string sensorId;


        private TurnstileThing _turnstileThing;

        private BoxCollider collider;

        // Start is called before the first frame update
        void Start()
        {
            _turnstileThing = GetComponentInParent<TurnstileThing>();
            collider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _turnstileThing.SensorEnter(sensorId, "enter");
        }

        private void OnTriggerExit(Collider other)
        {
            _turnstileThing.SensorEnter(sensorId, "exit");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}