using UnityEngine;
using UnityEngine.Video;

public class QuestProVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.url = Application.dataPath+ "/Videos/wobble_1.mp4";
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += VideoPrepared;
    }

    void VideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }
}
