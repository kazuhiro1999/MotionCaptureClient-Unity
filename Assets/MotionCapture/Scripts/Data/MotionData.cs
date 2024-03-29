using System.Collections.Generic;
using System.Linq;

namespace MotionCapture.Data
{
    [System.Serializable]
    public class MotionData
    {
        public List<PoseData> Data;

        // シーケンスの検索効率化のためのカレントインデックス
        private int _currentIndex = 0;

        // コンストラクタ
        public MotionData() => Data = new List<PoseData>();

        // 引数付きコンストラクタ
        public MotionData(List<PoseData> poses) => Data = poses;

        // Poseの追加
        public void AddPose(PoseData pose) => Data.Add(pose);

        // 最新のPoseを取得
        public PoseData GetLatestPose() => Data.Count > 0 ? Data[Data.Count - 1] : null;

        // 指定された期間内のPoseを平均して取得
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

        // 指定された期間のPoseを抽出して新しいMotionDataを作成
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

        // 全Poseの平均を計算
        public PoseData Average() {
            if (Data.Count == 0)
                return null;

            var totalPose = Data.Aggregate((acc, next) => acc + next);
            totalPose /= Data.Count;

            return totalPose;
        }
    }
}
