using UnityEngine;

public class PlayerControllerSetup : MonoBehaviour
{
    private OVRPlayerController playerController;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;

    void Start()
    {
        // 获取OVRPlayerController组件
        playerController = GetComponent<OVRPlayerController>();
        if (playerController == null)
        {
            Debug.LogError("OVRPlayerController component not found on this GameObject.");
            return;
        }

        // 获取或添加CapsuleCollider组件
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        // 设置CapsuleCollider的中心和高度
        capsuleCollider.center = new Vector3(0, 1.0f, 0); // 中心设置为1.0f高度的一半
        capsuleCollider.height = 2.0f;

        // 获取或添加Rigidbody组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        // 设置Rigidbody属性
        rb.useGravity = true;
        rb.isKinematic = false;

        // 确保创建地面对象
        CreateGroundIfNotExist();
    }

    void LateUpdate()
    {
        // 设置玩家初始位置的Y值为15
        Vector3 initialPosition = playerController.transform.position;
        if (Mathf.Abs(initialPosition.y - 15.0f) > 0.01f) // 确保只有在需要时才设置
        {
            initialPosition.y = 15.0f;
            playerController.transform.position = initialPosition;
            Debug.Log("Position corrected to: " + playerController.transform.position);
        }
    }

    void CreateGroundIfNotExist()
    {
        // 检查场景中是否存在名为"Ground"的对象
        GameObject groundObject = GameObject.Find("Ground");
        if (groundObject == null)
        {
            // 创建一个新的地面对象
            groundObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundObject.name = "Ground";
            groundObject.transform.position = Vector3.zero;
            groundObject.transform.localScale = new Vector3(10, 1, 10); // 调整地面的大小

            Debug.Log("Ground object created at position: " + groundObject.transform.position);
        }
    }

    void Update()
    {
        // 每帧打印Y轴位置以调试
        //Debug.Log("Current Y Position: " + playerController.transform.position.y);
    }
}
