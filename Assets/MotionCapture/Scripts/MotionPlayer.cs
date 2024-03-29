using UnityEngine;
using UnityEditor;
using MotionCapture.Data;
using UnityEngine.Events;
using System;
using System.IO;

namespace MotionCapture 
{
    public class MotionPlayer : MonoBehaviour
    {
        // モーション関連
        public string FilePath;
        private MotionData motion;

        // コントローラー
        public float time; // 再生時間
        [HideInInspector]
        public bool isPlaying = false;
        public bool isLoop = false;

        [Header("Events")]
        public UnityEvent<PoseData> OnTimeUpdated = new UnityEvent<PoseData>();


        void Start() {

            if (FilePath != "")
                this.motion = LoadMotion(FilePath);
            else
                this.motion = new MotionData();
        }
        void Update() {
            if (isPlaying) {
                time += Time.deltaTime;

                // モーション終了判定
                if (time > motion.GetLatestPose().GetTimeSecond()) {
                    if (isLoop) {
                        time = 0f;
                    } else {
                        isPlaying = false;
                    }
                }

                // 対応するポーズを取得
                MotionData poses = motion.Extract(time - 0.05f, time + 0.05f);
                PoseData pose = poses.Average();

                if (pose == null)
                    return;

                // 仮コード
                pose.Left2Right();

                OnTimeUpdated?.Invoke(pose);
            }
        }

        public void Play() {
            isPlaying = true;
        }

        public void Pause() {
            isPlaying = false;
        }

        public void SetTime(float t) {
            time = t;
        }

        void EndMotion() {
            time = 0f;
            if (!isLoop) {
                isPlaying = false;
            }
        }

        public MotionData LoadMotion(string filepath) {
            if (string.IsNullOrEmpty(filepath)) {
                Debug.LogError("Filepath could not be null");
                return new MotionData();
            }
            string path = filepath;
            if (!File.Exists(path)) {
                Debug.LogError($"[DataManager] File not exists : {path}");
                return null;
            }
            string json = File.ReadAllText(path);            
            if (!string.IsNullOrEmpty(json)) {
                motion = JsonUtility.FromJson<MotionData>(json);
            }           
            Debug.Log($"Motion loaded from {filepath} : length={motion.GetLatestPose().GetTimeSecond()}");
            time = 0f;
            return motion;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MotionPlayer))]
    public class MotionPlayerEditor : Editor
    {
        private MotionPlayer player;

        private void Awake() {
            player = target as MotionPlayer;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            player.FilePath = EditorGUILayout.TextField("File Path", player.FilePath);
            if (GUILayout.Button("Load")) {
                player.LoadMotion(player.FilePath);
            }

            if (!player.isPlaying) {
                if (GUILayout.Button("Play")) {
                    player.Play();
                }
            } else {
                if (GUILayout.Button("Pause")) {
                    player.Pause();
                }
            }       
        }
    }
#endif 
}

