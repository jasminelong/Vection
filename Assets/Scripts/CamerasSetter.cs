using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasSetter : MonoBehaviour
{
    [Space(10)]
    public Camera User_Camera;
    public Camera A_Camera;
    public Camera B_Camera;

    [Space(10)]
    public float InterCameraDistance;
    public float Hertz;
    public float CirclePerSecond;
    public float Phase;

    [Space(10)]
    public Vector3 Initial_User_Postion;
    public Vector3 Initial_A_LR_Postion;
    public Vector3 Initial_B_LR_Postion;

    // Start is called before the first frame update
    void Start()
    {
        //ÉJÉÅÉâÇÃç≈èâÇÃà íuÇê›íËÇ∑ÇÈ
        Initial_User_Postion = User_Camera.transform.position;
        Initial_A_LR_Postion = new Vector3(User_Camera.transform.position.x - InterCameraDistance / 2, User_Camera.transform.position.y, User_Camera.transform.position.z);
        Initial_B_LR_Postion = new Vector3(User_Camera.transform.position.x + InterCameraDistance / 2, User_Camera.transform.position.y, User_Camera.transform.position.z);
        A_Camera.transform.position = Initial_A_LR_Postion;
        B_Camera.transform.position = Initial_B_LR_Postion;
    }

    // Update is called once per frame
    void Update()
    {
        CirclePerSecond = Hertz * 2 * Mathf.PI;
        Phase = Time.time * CirclePerSecond % (2.0f * Mathf.PI);
        //ÉJÉÅÉâÇÃâ^ìÆÇê›íËÇ∑ÇÈ
        A_Camera.transform.position = new Vector3(Initial_A_LR_Postion.x + InterCameraDistance * Mathf.Sin(Phase) / 2, A_Camera.transform.position.y, A_Camera.transform.position.z);
        B_Camera.transform.position = new Vector3(Initial_B_LR_Postion.x + InterCameraDistance * Mathf.Sin(Phase) / 2, B_Camera.transform.position.y, B_Camera.transform.position.z);
    }
}
