using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using MotionCapture.Data;


namespace MotionCapture
{
    public class DataReceiver : MonoBehaviour
    {
        [Header("UDP Settings")]
        public int Port; // UDP通信用のポート番号
        private UdpClient udp;
        private bool _started;

        [Header("Data Settings")]
        public int BufferSize = 10; // データの最大バッファサイズ
        private MotionData motion;  // モーションデータのプール
        private bool _received;

        [Header("Events")]
        public UnityEvent<PoseData> OnDataReceived = new UnityEvent<PoseData>(); // データ受信時のイベント

        private Task task;
        private CancellationTokenSource cancellationTokenSource;

        void Start() {
            motion = new MotionData();
            _started = false;
        }

        // UDP通信を開始する
        public void UdpStart() {
            if (_started) return;

            udp = new UdpClient(Port);
            udp.Client.ReceiveTimeout = 1000;

            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(() => ReceiveDataAsync(cancellationTokenSource.Token));
            _started = true;
        }

        // 非同期でデータを受信するメソッド
        async Task ReceiveDataAsync(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                try {
                    UdpReceiveResult result = await udp.ReceiveAsync();
                    string json = Encoding.UTF8.GetString(result.Buffer);
                    PoseData pose = JsonUtility.FromJson<PoseData>(json);
                    pose.Left2Right();

                    lock (motion) {
                        motion.AddPose(pose);
                        if (motion.Data.Count > BufferSize)
                            motion.Data.RemoveAt(0);
                    }

                    _received = true;
                }
                catch (Exception e) {
                    Debug.LogError(e.Message);
                }
            }
        }

        // UDP通信を終了する
        public void UdpEnd() {
            if (!_started) return;

            cancellationTokenSource?.Cancel();
            if (!task.IsCompleted) {
                task.Wait();
            }
            udp.Dispose();
            _started = false;
        }

        // オブジェクトが破棄される時にUDP通信を終了する
        void OnDestroy() {
            UdpEnd();
        }

        // データが受信されたらイベントを発火する
        void Update() {
            if (_received) {
                _received = false;
                PoseData currentPose = GetPose();
                if (currentPose != null)
                    OnDataReceived?.Invoke(currentPose);
            }
        }

        // 最新のPoseデータを取得する
        public PoseData GetPose() {
            if (motion.Data.Count == 0) return null;
            return motion.Data[motion.Data.Count - 1];
        }

        // 指定した期間の平均Poseデータを取得する
        public PoseData GetPose(float duration) {
            if (motion.Data.Count == 0) return null;

            var pose = motion.GetLatestPose();
            var time = pose.GetTimeSecond();
            var poses = motion.Extract(time - duration, time);

            return poses.Average();
        }

        // 受信データからFPSを計算する
        public float GetFPS(int size) {
            if (motion.Data.Count < 2) return 0f;

            var endIndex = motion.Data.Count - 1;
            if (motion.Data.Count < size) {
                var elapsedTime = (float)(motion.Data[endIndex].TimeStamp - motion.Data[0].TimeStamp) / 1000;
                return (motion.Data.Count - 1) / elapsedTime;
            }
            else {
                var elapsedTime = (float)(motion.Data[endIndex].TimeStamp - motion.Data[endIndex - size + 1].TimeStamp) / 1000;
                return (size - 1) / elapsedTime;
            }
        }
    }
}
