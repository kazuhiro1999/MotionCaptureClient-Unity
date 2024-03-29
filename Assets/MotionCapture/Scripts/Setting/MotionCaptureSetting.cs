using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace MotionCapture
{
    /// <summary>
    /// モーションキャプチャの設定情報を管理
    /// </summary>
    [CreateAssetMenu(menuName = "MotionCapture/Setting", fileName = "MotionCaptureSetting")]
    public class MotionCaptureSetting : ScriptableObject
    {
        [Header("キャプチャ設定")]
        [JsonProperty("config_path")] public string ConfigPath = "config.json"; // 設定ファイルのパス
        [JsonProperty("cameras")] public List<CameraInfo> Cameras = new List<CameraInfo>(); // カメラ情報のリスト
    }
}
