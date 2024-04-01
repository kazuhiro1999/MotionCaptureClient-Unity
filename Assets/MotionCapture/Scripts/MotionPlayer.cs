using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using MotionCapture.Core;
using System;
using System.Threading.Tasks;

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

        private int _currentIndex;


        async void Start() {
            _currentIndex = 0;

            if (FilePath != "")
                this.motion = await LoadMotion(FilePath);
            else
                this.motion = new MotionData();
        }

        void Update() {
            if (isPlaying) {
                time += Time.deltaTime;

                // モーション終了判定
                if (time > motion.Length) {
                    if (isLoop) {
                        time = 0f;
                    } else {
                        isPlaying = false;
                    }
                }

                float startTime = time - 0.1f, endTime = time + 0.1f;
                int start = -1, end = -1;

                if (_currentIndex > 0 && motion.Poses[_currentIndex - 1].timestamp / 1000f > startTime)
                    _currentIndex = 0;

                int index = Math.Max(0, _currentIndex - 1); // 少し前から検索を開始

                // 範囲内の最初のポーズを探索
                while (index < motion.Poses.Count)
                {
                    if (motion.Poses[index].timestamp / 1000f >= startTime)
                    {
                        start = index;
                        _currentIndex = index;
                        break;
                    }
                    index++;
                }
                index++;
                end = index;
                // 範囲内の最後のポーズを探索
                while (index < motion.Poses.Count)
                {
                    if (motion.Poses[index].timestamp / 1000f <= endTime)
                    {
                        end = index;
                        index++;
                    }
                    else
                    {
                        break; // 終了時刻を超えたら終了
                    }
                }

                // 対応するポーズを取得
                PoseData pose = motion.GetAveragePose(start, end);

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

        public async Task<MotionData> LoadMotion(string filepath) {
            if (string.IsNullOrEmpty(filepath)) {
                Debug.LogError("Filepath could not be null");
                return null;
            }
            if (!File.Exists(filepath)) {
                Debug.LogError($"File not exists : {filepath}");
                return null;
            }
            byte[] bytes = await File.ReadAllBytesAsync(filepath);
            MotionData motion = MotionData.Decode(bytes);    
            Debug.Log($"Motion loaded from {filepath} : length={motion.Length}");
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
                _ = player.LoadMotion(player.FilePath);
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

