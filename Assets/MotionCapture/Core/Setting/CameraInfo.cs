using System;
using UnityEngine;
using Newtonsoft.Json;

namespace MotionCapture
{
    /// <summary>
    /// 使用するカメラの情報
    /// </summary>
    [Serializable]
    public class CameraInfo
    {
        // カメラの名前
        [JsonProperty("name")] public string Name = "Camera";

        // カメラの種類（USBカメラやビデオなど）
        public enum CameraType
        {
            USBCamera,  // USBカメラを表す
            Video       // ビデオファイルを表す
        }
        [JsonIgnore] public CameraType Type;
        [JsonProperty("type")] public string type => Type.ToString();

        // USBカメラを利用する場合のデバイスID
        [JsonProperty("device_id")] public int DeviceID = 0;

        // ビデオファイルを利用する場合のファイルパス
        [JsonProperty("video_path")] public string VideoPath = "path/to/video.mp4";

        // Unityの3D空間でのカメラ位置
        [JsonIgnore] public Vector3 Position;

        // サーバに送信する位置データ
        [JsonProperty("position")]
        private PositionData _position {
            get => CameraObject ? new PositionData(CameraObject.transform.position) : new PositionData(Position);
            set => Position = value.ToVector3();   // PositionDataからVector3に変換するメソッドを仮定
        }

        // Unityの3D空間でのカメラの回転
        [JsonIgnore] public Quaternion Rotation;

        // サーバに送信する回転データ
        [JsonProperty("rotation")]
        private RotationData _rotation {
            get => CameraObject ? new RotationData(CameraObject.transform.rotation) : new RotationData(Rotation);
            set => Rotation = value.ToQuaternion(); // RotationDataからQuaternionに変換するメソッドを仮定
        }

        // 実際のカメラオブジェクトへの参照（シリアライズしない）
        [NonSerialized]
        [JsonIgnore] public GameObject CameraObject;

        // カメラの設定ファイルのパス
        [JsonProperty("setting_path")] public string SettingPath = "path/to/camera_setting.json";
        
    }

    [Serializable]
    public class PositionData
    {
        public float x, y, z;

        public PositionData(float x, float y, float z) {
            this.x = -x;
            this.y = y;
            this.z = z;
        }

        public PositionData(Vector3 vector) {
            x = -vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3() {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class RotationData
    {
        public float x, y, z, w;

        public RotationData() { }

        public RotationData(Quaternion quaternion) {
            x = quaternion.x;
            y = -quaternion.y;
            z = -quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToQuaternion() {
            return new Quaternion(x, y, z, w);
        }
    }

}


