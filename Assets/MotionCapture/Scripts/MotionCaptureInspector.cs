using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MotionCapture.Data;

public class MotionCaptureInspector : MonoBehaviour
{
    public int BufferSize;
    public Text Text;

    private List<long> timestamps = new List<long>();
    private float _delay;
    private float _fps;

    private void Update() {
        if (timestamps.Count > 0) {
            var latestTimestamp = timestamps[timestamps.Count - 1];
            _delay = (float)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - latestTimestamp) / 1000;
        }
        else {
            _delay = 0f;
        }
        

        if (timestamps.Count > 1) {
            var elapsedTime = (float)(timestamps[timestamps.Count - 1] - timestamps[0]) / 1000;
            _fps = timestamps.Count / elapsedTime;
        }
        else {
            _fps = 0f;
        }
        Text.text = $"FPS: {_fps}\nDelay: {_delay}";
    }

    public void OnDataReceived(PoseData pose) {

        timestamps.Add(pose.TimeStamp);
        if (timestamps.Count > BufferSize)
            timestamps.RemoveAt(0);
    }
}
