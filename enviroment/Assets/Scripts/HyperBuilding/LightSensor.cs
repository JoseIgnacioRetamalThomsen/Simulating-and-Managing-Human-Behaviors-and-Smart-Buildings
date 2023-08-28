using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperBuilding
{
    public class LightSensor : MonoBehaviour
    {
        private RoomThing _roomThing;
        private string sensorId = "sensor1";
        private BoxCollider collider;

        void Start()
        {


            Transform grandparentTransform = transform.parent.parent;
            _roomThing = grandparentTransform.GetComponentInParent<RoomThing>();
            collider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _roomThing.SensorEnter(sensorId, "enter");
        }

        private void OnTriggerExit(Collider other)
        {
            _roomThing.SensorEnter(sensorId, "exit");
        }
    }
}