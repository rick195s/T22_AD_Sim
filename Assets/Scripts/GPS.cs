using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class GPS : MonoBehaviour
{
    ROSConnection ros;
    public float rate = 10;
    public string topicName = "/gps";
    public double publishRate = 0.5f;

    private double timeElapsed;

    // Start is called before the first frame update
    void Start()
    {
    	ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<NavSatFixMsg>(topicName);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= publishRate) {

            Debug.Log("Publishing location to " + topicName);

            Transform transform = GetComponent<Transform>();

            NavSatFixMsg pos = new NavSatFixMsg();
            pos.latitude = transform.position.z;
            pos.longitude = transform.position.x;
            pos.altitude = transform.position.y;

            ros.Publish(topicName, pos);

            timeElapsed = 0;
        }
    }
}
