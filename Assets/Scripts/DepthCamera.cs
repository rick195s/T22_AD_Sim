using UnityEngine;
using UnityEngine.Rendering;

public class DepthCamera : MonoBehaviour
{
    [Header("Shader Setup")]
    public Shader uberReplacementShader;

    private Camera camera;

    void Start()
    {
        // default fallbacks, if shaders are unspecified
        if (!uberReplacementShader)
            uberReplacementShader = Shader.Find("Hidden/UberReplacement");

        camera = GetComponent<Camera>();
        camera.targetDisplay = 1;
        
        SetupCameraWithReplacementShader(uberReplacementShader, Color.white);
    }



    private void SetupCameraWithReplacementShader(Shader shader, Color clearColor)
    {
        camera.SetReplacementShader(shader, "");
        camera.backgroundColor = clearColor;
        camera.clearFlags = CameraClearFlags.SolidColor;
    }
}
