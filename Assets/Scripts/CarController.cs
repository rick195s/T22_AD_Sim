using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Ackermann;

public class CarController : MonoBehaviour
{
    ROSConnection ros;
    public string controlTopicName = "/ackermann_control";
    public double wheelbase = 1550; // in cm
    public double cogToRear = 0.775; // center of gravity to rear wheel axis
    public double mass = 270; // in kg

    private double speed = 0;
    public double maxSpeed = 10; // m/s
    private double steeringAngle = 0;
    public double maxSteeringAngle = 40; // +/-

    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<AckermannDriveMsg>(controlTopicName, Control);
    }

    void Control(AckermannDriveMsg msg)
    {
        this.speed = msg.speed;
        this.steeringAngle = msg.steering_angle;
    }

    void Update() 
    {
        
    }

    public double getSpeed()
    {
        return this.speed;
    }

    public void setSpeed(double speed)
    {
        if(speed <= this.maxSpeed)
            this.speed = speed;
    }

    public double getSteeringAngle()
    {
        return this.steeringAngle;
    }

    public void setSteeringAngle(double steeringAngle)
    {
        if(steeringAngle <= this.maxSteeringAngle && steeringAngle >= -this.maxSteeringAngle)
            this.steeringAngle = steeringAngle;
    }
}
