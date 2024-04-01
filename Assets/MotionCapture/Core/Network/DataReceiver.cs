using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MotionCapture.Core
{
    public class DataReceiver : MonoBehaviour
    {
        [Header("UDP Settings")]
        public int Port; // UDP�ʐM�p�̃|�[�g�ԍ�
        private UdpClient udp;
        private bool _started;

        [Header("Data Settings")]
        public int BufferSize = 10; // �f�[�^�̍ő�o�b�t�@�T�C�Y
        private Queue<PoseData> _buffer;  // ���[�V�����f�[�^�̃v�[��
        private bool _received;
        private PoseData _latestData;

        [Header("Events")]
        public UnityEvent<PoseData> OnDataReceived = new UnityEvent<PoseData>(); // �f�[�^��M���̃C�x���g

        [Header("Others")]
        public bool StartOnAwake;

        private Task task;
        private CancellationTokenSource cancellationTokenSource;

        void Start() {
            _buffer = new Queue<PoseData>();
            _started = false;

            if (StartOnAwake)
            {
                UdpStart();
            }
        }

        // UDP�ʐM���J�n����
        public void UdpStart() {
            if (_started) return;

            udp = new UdpClient(Port);
            udp.Client.ReceiveTimeout = 1000;

            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(() => ReceiveDataAsync(cancellationTokenSource.Token));
            _started = true;
            Debug.Log("[DataReceiver] Udp started.");
        }

        // �񓯊��Ńf�[�^����M���郁�\�b�h
        async Task ReceiveDataAsync(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                try {
                    UdpReceiveResult result = await udp.ReceiveAsync();

                    (PoseType type, long timestamp, Vector3[] keypoints3d) = DataSerializer.Decode(result.Buffer);

                    PoseData pose = new PoseData(timestamp, keypoints3d, type);

                    // ���W�n�̕ϊ�
                    for (int i = 0; i < pose.positions.Length; i++)
                    {
                        pose.positions[i].x = -pose.positions[i].x;
                    }

                    _latestData = pose;
                    _received = true;

                    lock (_buffer) {
                        _buffer.Enqueue(pose);
                        if (_buffer.Count > BufferSize)
                            _buffer.Dequeue();
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
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
            udp.Close();
            if (!task.IsCompleted) {
                task.Wait();
            }
            udp.Dispose();
            _started = false;
            Debug.Log("[DataReceiver] Udp ended.");
        }

        // �I�u�W�F�N�g���j������鎞��UDP�ʐM���I������
        void OnDestroy() {
            UdpEnd();
        }

        // �f�[�^����M���ꂽ��C�x���g�𔭉΂���
        void Update() {
            if (_received) {
                _received = false;
                OnDataReceived?.Invoke(_latestData);
            }
        }
    }
}
