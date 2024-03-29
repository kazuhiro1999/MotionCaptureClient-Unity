using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace MotionCapture
{
    /// <summary>
    /// ���[�V�����L���v�`���̐ݒ�����Ǘ�
    /// </summary>
    [CreateAssetMenu(menuName = "MotionCapture/Setting", fileName = "MotionCaptureSetting")]
    public class MotionCaptureSetting : ScriptableObject
    {
        [Header("�L���v�`���ݒ�")]
        [JsonProperty("config_path")] public string ConfigPath = "config.json"; // �ݒ�t�@�C���̃p�X
        [JsonProperty("cameras")] public List<CameraInfo> Cameras = new List<CameraInfo>(); // �J�������̃��X�g
    }
}
