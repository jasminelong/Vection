using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewsBlender : MonoBehaviour
{
    public Camera User_Camera;
    public Camera A_Camera;
    public Camera B_Camera;
    public GameObject A_Image;
    public GameObject B_Image;

    [Space(10)]
    public float CirclePerSecond;
    public float Phase;

    [Space(10)]
    public float AToBDistance;
    public float UserToADistance;
    public float UserToBDistance;

    [Space(10)]
    [Range(0, 1)] public float RatioCameraA;
    [Range(0, 1)] public float RatioCameraB;

    // Start is called before the first frame update
    void Start()
    {
        AToBDistance = this.GetComponent<CamerasSetter>().InterCameraDistance;
    }

    // Update is called once per frame
    void Update()
    {
        CirclePerSecond = this.GetComponent<CamerasSetter>().CirclePerSecond;
        Phase = Time.time * CirclePerSecond % (2.0f * Mathf.PI);
        UserToBDistance = Vector3.Distance(User_Camera.transform.position, B_Camera.transform.position);
        //辉度比例
        RatioCameraA = UserToBDistance / AToBDistance;
        RatioCameraB = 1.0f - RatioCameraA;
        //辉度設定
        RawImage A_rawimage = A_Image.GetComponents<RawImage>()[0];
        RawImage B_rawimage = B_Image.GetComponents<RawImage>()[0];
        A_rawimage.color = new Color(A_rawimage.color.r, A_rawimage.color.g, A_rawimage.color.b, RatioCameraA);
        B_rawimage.color = new Color(B_rawimage.color.r, B_rawimage.color.g, B_rawimage.color.b, RatioCameraB);
    }
}
