using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using UnityEngine.Analytics;
using System;

public class DepthCamera : MonoBehaviour
{
    [Header("Shader Setup")]
    public Shader depthCameraShader;

    private new Camera camera;

    ROSConnection ros;
    private string cameraInfoTopic = "camera_info";

    void Start()
    {
        // default fallbacks, if shaders are unspecified
        if (!depthCameraShader)
            depthCameraShader = Shader.Find("DepthCameraShader");

        camera = GetComponent<Camera>();
        camera.targetDisplay = 1;

        SetupCameraWithReplacementShader(depthCameraShader, Color.black);
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<CameraInfoMsg>(cameraInfoTopic); ;
    }

    private void Update()
    {
        publishCameraIntrisics();
    }
    private void publishCameraIntrisics()
    {
        

        // camera Intrinsics 
        CameraInfoMsg cameraIntrinsics = new();

        cameraIntrinsics.header = new HeaderMsg { stamp = new TimeMsg(), frame_id = "camera_frame" };
        cameraIntrinsics.width = (uint)camera.pixelWidth;
        cameraIntrinsics.height = (uint)camera.pixelHeight;


        float fx = camera.lensShift.x;
        float fy = camera.lensShift.y;
        float cx = camera.pixelWidth / 2;
        float cy = camera.pixelHeight / 2;
        cameraIntrinsics.k = new double[9] {
            fx, 0,  cx,
            0,  fy, cy,
            0,  0,  1
        };

        cameraIntrinsics.distortion_model = "plumb_bob";
        cameraIntrinsics.d = new double[5];
        cameraIntrinsics.r = new double[9];
        cameraIntrinsics.p = new double[12];
        cameraIntrinsics.binning_x = 14;
        cameraIntrinsics.binning_y = 123;

        RegionOfInterestMsg roi = new RegionOfInterestMsg(0, 1, 2, 3, true);
        cameraIntrinsics.roi = roi;

        ros.Publish(cameraInfoTopic, cameraIntrinsics);

    }


    private void SetupCameraWithReplacementShader(Shader shader, Color clearColor)
    {
        camera.SetReplacementShader(shader, "");
        camera.backgroundColor = clearColor;
        camera.clearFlags = CameraClearFlags.SolidColor;
    }
}
