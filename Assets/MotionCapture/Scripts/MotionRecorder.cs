using System;
using System.IO;
using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture.Recorder
{
    public class MotionRecorder : MonoBehaviour
    {
        private DateTimeOffset startTime;
        private MotionData motion;

        public bool IsRecording { get; private set; }

        void Start() {

        }

        void Update() {

        }

        public void OnDataReceived(PoseData pose) {
            if (!IsRecording || pose == null) return;
            
            pose = pose.Copy();
            pose.TimeStamp -= startTime.ToUnixTimeMilliseconds(); // UNIXTIME(�~���b)�Ŕ�r
            motion.AddPose(pose);
        }


        public void StartRecord() {
            motion = new MotionData();
            startTime = DateTimeOffset.UtcNow;
            IsRecording = true;
        }

        public void EndRecord() {
            IsRecording = false;
        }

        public void SaveMotionData(string path) {

            if (IsRecording || motion == null) return;

            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            string json = JsonUtility.ToJson(motion);
            File.WriteAllText(path, json);
            Debug.Log($"[MotionRecorder] Motion data saved to {path}");
        }
    }
}
