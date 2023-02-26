using UnityEngine;
using UnityEngine.Rendering;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.SensorMsgs;

public class DepthCamera : MonoBehaviour
{
    [Header("Shader Setup")]
    public Shader uberReplacementShader;

    public Shader depthShader;
    private new Camera camera;

    ROSConnection ros;
    private string cameraInfoTopic = "sensor_msgs";

    void Start()
    {
        // default fallbacks, if shaders are unspecified
        if (!uberReplacementShader)
            uberReplacementShader = Shader.Find("Hidden/UberReplacement");

        camera = GetComponent<Camera>();
        camera.targetDisplay = 1;

        SetupCameraWithReplacementShader(uberReplacementShader, Color.white);
        publishCameraIntrisics();
    }


    private void publishCameraIntrisics()
    {
        // camera Intrinsics 
        Matrix4x4 projection = camera.projectionMatrix;
        CameraInfoMsg cameraIntrinsics = new CameraInfoMsg(
            Mathf.Atan(1 / projection[1, 1]) * 2 * Mathf.Rad2Deg,
            projection[1, 1] / projection[0, 0],
            camera.nearClipPlane,
            camera.farClipPlane
        );


        // publishing camera Intrinsics to sensor_msgs/CameraInfo ROS channel
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<CameraInfoMsg>(cameraInfoTopic);
        ros.Publish(cameraInfoTopic, cameraIntrinsics);
    }


    private void SetupCameraWithReplacementShader(Shader shader, Color clearColor)
    {
        camera.SetReplacementShader(shader, "");
        camera.backgroundColor = clearColor;
        camera.clearFlags = CameraClearFlags.SolidColor;
    }
}
