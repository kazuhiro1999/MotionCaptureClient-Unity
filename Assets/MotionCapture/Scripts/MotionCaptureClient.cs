using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using MotionCapture.Data;
using System.Linq;

namespace MotionCapture
{
    public class MotionCaptureClient : MonoBehaviour
    {
        // WebSocket settings
        [Header("WebSocket Server Setting")]
        public string Host = "192.168.x.x";
        public int Port = 50000;
        private WebSocket ws;

        // Data settings
        [Header("Data Settings")]
        public int BufferSize = 10; 
        private MotionData _motion = new MotionData();
        private bool _received = false;

        // Events
        [Header("Events")] 
        public UnityEvent OnCaptureStarted = new UnityEvent();
        public UnityEvent OnCaptureEnded = new UnityEvent();
        public UnityEvent<PoseData> OnDataReceived = new UnityEvent<PoseData>();
        public UnityEvent<CalibrationResult> OnCalibrationExecuted = new UnityEvent<CalibrationResult>();

        // Advanced settings
        [Header("Advanced Setting")]
        public Transform HMD;
        public bool UseHMDTrajectry;
        private bool isCollectingHMDData = false;
        private TrajectryData trajectry;

        public MotionCaptureSetting Setting; // Motion capture settings

        public bool IsCaptureStarted { get; private set; }
        public bool IsConnected => ws.ReadyState == WebSocketState.Open;

        private Dictionary<string, TaskCompletionSource<ResponseData>> _pendingResponses = new Dictionary<string, TaskCompletionSource<ResponseData>>();


        private void Start() {

            // Setup WebSocket
            ws = new WebSocket($"ws://{Host}:{Port}");
            ws.OnOpen += OnConnected;
            ws.OnMessage += OnMessageReceived;
            ws.OnClose += OnClosed;

            _ = Connect(3000);
        }

        private void Update() {

            // Process received pose data
            if (_received) {
                _received = false;
                PoseData currentPose = _motion.GetLatestPose();
                if (currentPose != null)
                    OnDataReceived?.Invoke(currentPose);
            }

            // Collect HMD trajectory data
            if (isCollectingHMDData && HMD) {
                TransformData transformData = new TransformData(HMD.transform);
                trajectry.Add(transformData);
            }
        }

        /// <summary>
        /// Connect to the WebSocket server with timeout.
        /// </summary>
        [ContextMenu("Connect")]
        public async Task<bool> Connect(int timeout = 3000) {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.Log($"Trying to connect {Host}");

                Task task = Task.Run(() => ws.Connect());
                if (await Task.WhenAny(task, Task.Delay(timeout)) != task) {
                    Debug.LogError("WebSocket connection timed out.");
                    return false;
                }
                return IsConnected;
            }
            return true;
        }

        /// <summary>
        /// Initialize settings and synchronize time.
        /// </summary>
        private void OnConnected(object sender, EventArgs e) {
            Debug.Log("Connected to the server.");
            _ = Initialize();
            _ = SyncTime();
            IsCaptureStarted = false;
        }

        private void OnClosed(object sender, CloseEventArgs e) {
            Debug.LogWarning($"WebSocket connection closed: {e.Reason}");
        }

        [ContextMenu("Test")]
        public async void Test() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return;
            }
            var data = new ActionData {
                type = "test",
                action = "connection"
            };
            var res = await SendActionAsync(data);
            Debug.Log(res.message);
        }

        [ContextMenu("Sync Time")]
        public async Task<bool> SyncTime() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData {
                type = "time",
                action = "sync",
                data = TimeUtil.GetUnixTime().ToString()
            };
            var response = await SendActionAsync(data);
            if (response.status == "success") {
                Debug.Log(response.message);
                return true;
            }
            else {
                Debug.LogError(response.message);
                return false;
            }
        }

        [ContextMenu("Initialize")]
        public async Task<bool> Initialize() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData {
                type = "capture",
                action = "init",
                data = Setting.ConfigPath
            };
            var response = await SendActionAsync(data);
            if (response.status == "success") {
                Debug.Log(response.message);
                return true;
            }
            else {
                Debug.LogError(response.message);
                return false;
            }
        }


        [ContextMenu("Start Capture")]
        public async Task<bool> StartCapture() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData {
                type = "capture",
                action = "start",
            };
            var response = await SendActionAsync(data);

            if (response.status == "success") {
                Debug.Log(response.message);
                IsCaptureStarted = true;
                OnCaptureStarted?.Invoke();
                return true;
            } else if (response.status == "failed") {
                Debug.LogError(response.message);
                return false;
            }
            else {
                Debug.LogError("Unknown response status: " + response.status);
                return false;
            }
        }

        [ContextMenu("End Capture")]
        public async Task<bool> EndCapture() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData {
                type = "capture",
                action = "end"
            };
            var response = await SendActionAsync(data);

            if (response.status == "success") {
                Debug.Log(response.message);
                IsCaptureStarted = false;
                OnCaptureEnded?.Invoke();
                return true;
            }
            else if (response.status == "failed") {
                Debug.LogError(response.message);
                return false;
            }
            else {
                Debug.LogError("Unknown response status: " + response.status);
                return false;
            }
        }

        [ContextMenu("Init Calibration")]
        public async Task<bool> InitCalibration() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData {
                type = "calibration",
                action = "init"
            };
            var response = await SendActionAsync(data);

            if (response.status == "initialized") {
                Debug.Log(response.message); 
                IsCaptureStarted = true;
                return true;
            } else if (response.status == "failed") {
                Debug.LogError("Calibration failed to initialize: " + response.message);
                return false;
            }
            else {
                return false;
            }
        }

        [ContextMenu("Start Calibration")]
        public async Task<bool> StartCalibration() {
            if (ws.ReadyState != WebSocketState.Open) {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }

            // Start collecting HMD trajectory data.
            if (UseHMDTrajectry) {
                trajectry = new TrajectryData();
                isCollectingHMDData = true;
            }

            var data = new ActionData {
                type = "calibration",
                action = "start",
                data = HMD ? HMD.transform.position.y.ToString() : "1.6"
            };
            var response = await SendActionAsync(data);            

            if (response.status == "finished") {
                Debug.Log(response.message);
                var result = JsonUtility.FromJson<CalibrationResult>(response.data);
                
                // Send Trajectory data if Use HMD Trajectory enabled.
                if (UseHMDTrajectry) {
                    result = await SendTrajectoryData();
                }

                // Update setting
                foreach (var camera in result.cameras) {
                    CameraInfo info = Setting.Cameras.FirstOrDefault(x => x.Name == camera.name);
                    if (info != null) {
                        info.Position = camera.position;
                        info.Rotation = camera.rotation;
                    }
                }
                OnCalibrationExecuted?.Invoke(result);
                return true;
            } else {
                Debug.LogError("Calibration failed to start: " + response.message);
                return false;
            }
        }

        private async Task<CalibrationResult> SendTrajectoryData() {
            isCollectingHMDData = false;
            var data = new ActionData {
                type = "calibration",
                action = "trajectry",
                data = JsonUtility.ToJson(trajectry)
            };
            var response = await SendActionAsync(data);
            if (response.status == "success") {
                return JsonUtility.FromJson<CalibrationResult>(response.data);
            }
            else {
                Debug.LogError(response.message);
                return null;
            }
        }

        /// <summary>
        /// Handle server responses.
        /// </summary>
        private void OnMessageReceived(object sender, MessageEventArgs e) {
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(e.Data);

            if (responseData.type == "pose") {
                HandlePoseData(responseData.data);
            }         
            else if (_pendingResponses.TryGetValue(responseData.id, out var tcs)) {
                tcs.SetResult(responseData);
                _pendingResponses.Remove(responseData.id);
            }
            else {
                Debug.LogWarning("Unknown response: " + responseData.type);
            }
        }

        void HandlePoseData(string data) {
            
            PoseData pose = JsonUtility.FromJson<PoseData>(data).Left2Right();

            lock (_motion) {
                _motion.AddPose(pose);
                if (_motion.Data.Count > BufferSize)
                    _motion.Data.RemoveAt(0);
            }
            _received = true;
        }

        async Task<ResponseData> SendActionAsync(ActionData action) {
            if (ws.ReadyState != WebSocketState.Open) {
                throw new InvalidOperationException("WebSocket is not connected.");
            }
            // Generate unique ID
            string id = Guid.NewGuid().ToString();
            action.id = id;

            var tcs = new TaskCompletionSource<ResponseData>();
            _pendingResponses[id] = tcs;

            ws.Send(JsonUtility.ToJson(action));
            return await tcs.Task;
        }


        private async void OnDestroy() {
            if (IsCaptureStarted) {
                var success = await EndCapture();
            }

            if (ws != null) {
                ws.OnClose -= OnClosed;
                if (ws.ReadyState != WebSocketState.Closed)
                    ws.Close();
                ws = null;
            }
        }

        /// <summary>
        /// サーバに送信するアクション
        /// </summary>
        [System.Serializable]
        public class ActionData
        {
            public string id;
            public string type;
            public string action;
            public string data;
        }

        /// <summary>
        /// サーバから受信するレスポンス
        /// </summary>
        [System.Serializable]
        public class ResponseData
        {
            public string id;
            public string type;
            public string action;
            public string status;
            public string message;
            public string data;
        }

        /// <summary>
        /// HMDの軌跡を送信するためのデータ構造
        /// </summary>

        [Serializable]
        public class TrajectryData
        {
            public List<TransformData> transforms;
            public TrajectryData() {
                transforms = new List<TransformData>();
            }

            public void Add(TransformData transformData) {
                transforms.Add(transformData);
            }
        }

        [Serializable]
        public class TransformData
        {
            public long timestamp;
            public Vector3 position;
            public Quaternion rotation;
            public TransformData() {
                timestamp = TimeUtil.GetUnixTime();
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
            public TransformData(Transform transform) {
                timestamp = TimeUtil.GetUnixTime();
                position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
                rotation = new Quaternion(transform.rotation.x, -transform.rotation.y, -transform.rotation.z, transform.rotation.w);
            }
        }

        /// <summary>
        /// キャリブレーション結果を格納するためのクラス
        /// </summary>
        [Serializable]
        public class CalibrationResult
        {
            public List<CameraData> cameras;
        }

        [Serializable]
        public class CameraData
        {
            public string name;
            public Vector3 position;
            public Quaternion rotation;
        }
    }
}
