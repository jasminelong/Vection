using UnityEngine;

public class OVRPlayerControllerDebug : MonoBehaviour
{
    public GameObject OVRPlayerController; // 拖拽OVRPlayerController对象到此变量

    void Start()
    {
        // 确保OVRPlayerController存在
        if (OVRPlayerController == null)
        {
            Debug.LogError("OVRPlayerController is not assigned.");
            return;
        }

        // 设置初始位置为(0,0,0)
        OVRPlayerController.transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        // 打印OVRPlayerController的位置
        Vector3 position = OVRPlayerController.transform.position;
        Debug.Log($"OVRPlayerController Position - X: {position.x}, Y: {position.y}, Z: {position.z}");
    }
}
