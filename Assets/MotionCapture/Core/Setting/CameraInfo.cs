using System;
using UnityEngine;
using Newtonsoft.Json;

namespace MotionCapture
{
    /// <summary>
    /// �g�p����J�����̏��
    /// </summary>
    [Serializable]
    public class CameraInfo
    {
        // �J�����̖��O
        [JsonProperty("name")] public string Name = "Camera";

        // �J�����̎�ށiUSB�J������r�f�I�Ȃǁj
        public enum CameraType
        {
            USBCamera,  // USB�J������\��
            Video       // �r�f�I�t�@�C����\��
        }
        [JsonIgnore] public CameraType Type;
        [JsonProperty("type")] public string type => Type.ToString();

        // USB�J�����𗘗p����ꍇ�̃f�o�C�XID
        [JsonProperty("device_id")] public int DeviceID = 0;

        // �r�f�I�t�@�C���𗘗p����ꍇ�̃t�@�C���p�X
        [JsonProperty("video_path")] public string VideoPath = "path/to/video.mp4";

        // Unity��3D��Ԃł̃J�����ʒu
        [JsonIgnore] public Vector3 Position;

        // �T�[�o�ɑ��M����ʒu�f�[�^
        [JsonProperty("position")]
        private PositionData _position {
            get => CameraObject ? new PositionData(CameraObject.transform.position) : new PositionData(Position);
            set => Position = value.ToVector3();   // PositionData����Vector3�ɕϊ����郁�\�b�h������
        }

        // Unity��3D��Ԃł̃J�����̉�]
        [JsonIgnore] public Quaternion Rotation;

        // �T�[�o�ɑ��M�����]�f�[�^
        [JsonProperty("rotation")]
        private RotationData _rotation {
            get => CameraObject ? new RotationData(CameraObject.transform.rotation) : new RotationData(Rotation);
            set => Rotation = value.ToQuaternion(); // RotationData����Quaternion�ɕϊ����郁�\�b�h������
        }

        // ���ۂ̃J�����I�u�W�F�N�g�ւ̎Q�Ɓi�V���A���C�Y���Ȃ��j
        [NonSerialized]
        [JsonIgnore] public GameObject CameraObject;

        // �J�����̐ݒ�t�@�C���̃p�X
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


