using System.Collections.Generic;
using System.Linq;

namespace MotionCapture.Data
{
    [System.Serializable]
    public class MotionData
    {
        public List<PoseData> Data;

        // �V�[�P���X�̌����������̂��߂̃J�����g�C���f�b�N�X
        private int _currentIndex = 0;

        // �R���X�g���N�^
        public MotionData() => Data = new List<PoseData>();

        // �����t���R���X�g���N�^
        public MotionData(List<PoseData> poses) => Data = poses;

        // Pose�̒ǉ�
        public void AddPose(PoseData pose) => Data.Add(pose);

        // �ŐV��Pose���擾
        public PoseData GetLatestPose() => Data.Count > 0 ? Data[Data.Count - 1] : null;

        // �w�肳�ꂽ���ԓ���Pose�𕽋ς��Ď擾
        public PoseData GetPose(float duration) {
            if (Data.Count == 0)
                return null;

            var lastPose = Data[Data.Count - 1];
            var endTimeStamp = lastPose.TimeStamp;
            var totalPoses = Data.Where(p => p.TimeStamp >= endTimeStamp - duration).ToList();

            var averagePose = totalPoses.Aggregate((acc, next) => acc + next);
            averagePose /= totalPoses.Count;

            return averagePose;
        }

        // �w�肳�ꂽ���Ԃ�Pose�𒊏o���ĐV����MotionData���쐬
        public MotionData Extract(float start, float end) {
            if (_currentIndex > 1 && Data[_currentIndex - 1].GetTimeSecond() >= start)
                _currentIndex = 0;

            var extractedPoses = new List<PoseData>();

            for (int i = _currentIndex; i < Data.Count; i++) {
                float time = Data[i].GetTimeSecond();
                if (time < start)
                    _currentIndex = i;
                else if (time >= start && time <= end)
                    extractedPoses.Add(Data[i]);
                else
                    break;
            }

            return new MotionData(extractedPoses);
        }

        // �SPose�̕��ς��v�Z
        public PoseData Average() {
            if (Data.Count == 0)
                return null;

            var totalPose = Data.Aggregate((acc, next) => acc + next);
            totalPose /= Data.Count;

            return totalPose;
        }
    }
}
