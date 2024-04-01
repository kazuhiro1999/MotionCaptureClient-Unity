using System;
using System.IO;
using UnityEngine;
using MotionCapture.Core;

namespace MotionCapture.Recorder
{
    public class MotionRecorder : MonoBehaviour
    {
        [SerializeField] DataReceiver Receiver;

        public string FileName;

        public bool StartOnEditor;
        public bool EndOnEditor;

        private DateTimeOffset startTime;
        private MotionData motion;

        public bool IsRecording { get; private set; }

        void Start() {
            Receiver.OnDataReceived.AddListener(OnDataReceived);
        }

        void Update() {
            if (StartOnEditor)
            {
                StartOnEditor = false;
                StartRecord();
            }
            if (EndOnEditor)
            {
                EndOnEditor = false;
                EndRecord();
            }
        }

        public void OnDataReceived(PoseData pose) {
            if (!IsRecording) return;
            
            pose.timestamp -= startTime.ToUnixTimeMilliseconds(); // UNIXTIME(ƒ~ƒŠ•b)‚Å”äŠr
            motion.AddPose(pose);
        }

        public void StartRecord() {
            motion = new MotionData();
            startTime = DateTimeOffset.UtcNow;
            IsRecording = true;
            Debug.Log("Record started.");
        }

        public void EndRecord() {
            IsRecording = false;
            Debug.Log("Record ended.");
        }

        public void SaveMotionData(string path) {

            if (IsRecording || motion == null) return;

            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            byte[] bytes = motion.GetBytes();
            File.WriteAllBytes(path, bytes);
            Debug.Log($"[MotionRecorder] Motion data saved to {path}");
        }

        [ContextMenu("Save")]
        public void Save()
        {
            var path = Path.Combine(Application.dataPath, "Data", FileName + ".mot");
            SaveMotionData(path);
        }
    }
}
