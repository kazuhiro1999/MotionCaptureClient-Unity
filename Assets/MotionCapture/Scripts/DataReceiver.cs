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
        public int Port; // UDP�ʐM�p�̃|�[�g�ԍ�
        private UdpClient udp;
        private bool _started;

        [Header("Data Settings")]
        public int BufferSize = 10; // �f�[�^�̍ő�o�b�t�@�T�C�Y
        private MotionData motion;  // ���[�V�����f�[�^�̃v�[��
        private bool _received;

        [Header("Events")]
        public UnityEvent<PoseData> OnDataReceived = new UnityEvent<PoseData>(); // �f�[�^��M���̃C�x���g

        private Task task;
        private CancellationTokenSource cancellationTokenSource;

        void Start() {
            motion = new MotionData();
            _started = false;
        }

        // UDP�ʐM���J�n����
        public void UdpStart() {
            if (_started) return;

            udp = new UdpClient(Port);
            udp.Client.ReceiveTimeout = 1000;

            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(() => ReceiveDataAsync(cancellationTokenSource.Token));
            _started = true;
        }

        // �񓯊��Ńf�[�^����M���郁�\�b�h
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

        // UDP�ʐM���I������
        public void UdpEnd() {
            if (!_started) return;

            cancellationTokenSource?.Cancel();
            if (!task.IsCompleted) {
                task.Wait();
            }
            udp.Dispose();
            _started = false;
        }

        // �I�u�W�F�N�g���j������鎞��UDP�ʐM���I������
        void OnDestroy() {
            UdpEnd();
        }

        // �f�[�^����M���ꂽ��C�x���g�𔭉΂���
        void Update() {
            if (_received) {
                _received = false;
                PoseData currentPose = GetPose();
                if (currentPose != null)
                    OnDataReceived?.Invoke(currentPose);
            }
        }

        // �ŐV��Pose�f�[�^���擾����
        public PoseData GetPose() {
            if (motion.Data.Count == 0) return null;
            return motion.Data[motion.Data.Count - 1];
        }

        // �w�肵�����Ԃ̕���Pose�f�[�^���擾����
        public PoseData GetPose(float duration) {
            if (motion.Data.Count == 0) return null;

            var pose = motion.GetLatestPose();
            var time = pose.GetTimeSecond();
            var poses = motion.Extract(time - duration, time);

            return poses.Average();
        }

        // ��M�f�[�^����FPS���v�Z����
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
