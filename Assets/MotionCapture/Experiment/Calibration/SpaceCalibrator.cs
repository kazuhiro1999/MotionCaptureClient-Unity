using System;
using System.Collections.Generic;
using UnityEngine;
using MotionCapture.Core;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Events;

using static MotionCapture.Core.MotionCaptureClient;


namespace MotionCapture.Experiment
{
    public class SpaceCalibrator : MonoBehaviour
    {
        [SerializeField] MotionCaptureClient Client;

        public Transform HMD;
        public bool UseHMDTrajectry;
        private bool isCollectingHMDData = false;
        private TrajectryData trajectry;

        public MotionCaptureSetting Setting; // Motion capture settings

        public UnityEvent<CalibrationResult> OnCalibrationExecuted = new UnityEvent<CalibrationResult>();

        private void Update()
        {
            // Collect HMD trajectory data
            if (isCollectingHMDData && HMD)
            {
                TransformData transformData = new TransformData(HMD.transform);
                trajectry.Add(transformData);
            }
        }


        [ContextMenu("Init Calibration")]
        public async Task<bool> InitCalibration()
        {
            if (!Client.IsConnected)
            {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }
            var data = new ActionData
            {
                type = "calibration",
                action = "init"
            };
            var response = await Client.SendActionAsync(data);

            if (response.status == "initialized")
            {
                Debug.Log(response.message);
                return true;
            }
            else if (response.status == "failed")
            {
                Debug.LogError("Calibration failed to initialize: " + response.message);
                return false;
            }
            else
            {
                return false;
            }
        }

        [ContextMenu("Start Calibration")]
        public async Task<bool> StartCalibration()
        {
            if (!Client.IsConnected)
            {
                Debug.LogError("WebSocket is not connected.");
                return false;
            }

            // Start collecting HMD trajectory data.
            if (UseHMDTrajectry)
            {
                trajectry = new TrajectryData();
                isCollectingHMDData = true;
            }

            var data = new ActionData
            {
                type = "calibration",
                action = "start",
                data = HMD ? HMD.transform.position.y.ToString() : "1.6"
            };
            var response = await Client.SendActionAsync(data);

            if (response.status == "finished")
            {
                Debug.Log(response.message);
                var result = JsonUtility.FromJson<CalibrationResult>(response.data);

                // Send Trajectory data if Use HMD Trajectory enabled.
                if (UseHMDTrajectry)
                {
                    result = await SendTrajectoryData();
                }

                // Update setting
                foreach (var camera in result.cameras)
                {
                    CameraInfo info = Setting.Cameras.FirstOrDefault(x => x.Name == camera.name);
                    if (info != null)
                    {
                        info.Position = camera.position;
                        info.Rotation = camera.rotation;
                    }
                }
                OnCalibrationExecuted?.Invoke(result);
                return true;
            }
            else
            {
                Debug.LogError("Calibration failed to start: " + response.message);
                return false;
            }
        }

        private async Task<CalibrationResult> SendTrajectoryData()
        {
            isCollectingHMDData = false;
            var data = new ActionData
            {
                type = "calibration",
                action = "trajectry",
                data = JsonUtility.ToJson(trajectry)
            };
            var response = await Client.SendActionAsync(data);
            if (response.status == "success")
            {
                return JsonUtility.FromJson<CalibrationResult>(response.data);
            }
            else
            {
                Debug.LogError(response.message);
                return null;
            }
        }


        [Serializable]
        public class TrajectryData
        {
            public List<TransformData> transforms;
            public TrajectryData()
            {
                transforms = new List<TransformData>();
            }

            public void Add(TransformData transformData)
            {
                transforms.Add(transformData);
            }
        }

        [Serializable]
        public class TransformData
        {
            public long timestamp;
            public Vector3 position;
            public Quaternion rotation;
            public TransformData()
            {
                timestamp = TimeUtil.GetUnixTime();
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
            public TransformData(Transform transform)
            {
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
