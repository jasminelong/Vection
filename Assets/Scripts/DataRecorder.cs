using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataRecorder : MonoBehaviour
{
    public string participantName;
    private string experimentalCondition;
    public int trialNumber;

    private List<string> data = new List<string>();
    private float startTime;
    private bool vectionResponse = false;
    private string folderName = "ExperimentData"; // 子文件夹名称
    void Start()
    {
        // 添加数据表头
        data.Add("FrameNum, Time [ms], Vection Response (0:no, 1: yes )");
        startTime = Time.time;
        experimentalCondition = this.GetComponent<CylinderCamera>().movementPattern.ToString() +"_" 
                                                 + "cameraSpeed" + this.GetComponent<CylinderCamera>().cameraSpeed.ToString() + "_"
                                                 + "fps" + this.GetComponent<CylinderCamera>().fps.ToString()
                                                 ;
    }

    void Update()
    {
        // 获取当前帧数和时间
        int frameNum = this.GetComponent<CylinderCamera>().frameNum;
        float timeMs = (Time.time - startTime) * 1000;

        // 检测按键状态
        if (Input.GetKey(KeyCode.Space))
        {
            vectionResponse = true;
        }
        else
        {
            vectionResponse = false;
        }

        // 记录数据
        data.Add($"{frameNum}, {timeMs:F4}, {(vectionResponse ? 1 : 0)}");
    }


}
