using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Ackermann;

public class CarController : MonoBehaviour
{
    public string controlTopicName = "/ackermann_control";
    public double wheelbase = 1550; // in cm
    public double mass = 150; // in kg

    // Start is called before the first frame update
    void Start()
    {
        // ROSConnection.GetOrCreateInstance().Subscribe<AckermannDriveMsg>(controlTopicName, Control);
    }

    void Control(AckermannDriveMsg msg)
    {

    }
}
