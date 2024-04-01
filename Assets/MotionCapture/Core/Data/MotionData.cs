using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MotionCapture.Core
{
    public class MotionData
    {
        public List<PoseData> Poses { get; private set; } = new List<PoseData>();

        public float Length => Poses.Count > 0 ? (float)Poses.Last().timestamp / 1000 : 0;

        public MotionData()
        {
            Poses = new List<PoseData>();
        }

        public void AddPose(PoseData pose)
        {
            Poses.Add(pose);
        }

        public PoseData GetAveragePose(int start, int end)
        {
            var pose = Poses[start];

            for (int i = start + 1; i <= end; i++)
            {
                pose += Poses[i];
            }

            pose /= (end - start + 1);

            return pose;
        }

        public byte[] GetBytes()
        {
            List<byte> data = new List<byte>();

            // ポーズの数をエンコード
            data.AddRange(BitConverter.GetBytes(Poses.Count));

            foreach (var pose in Poses)
            {
                // 各ポーズをバイト配列にエンコードし、追加
                byte[] encodedPose = DataSerializer.Encode(pose.timestamp, pose.positions, pose.type);

                data.AddRange(BitConverter.GetBytes(encodedPose.Length));
                data.AddRange(encodedPose);
            }

            return data.ToArray();
        }

        public static MotionData Decode(byte[] data)
        {
            int offset = 0;
            MotionData motionData = new MotionData();

            // ポーズの数をデコード
            int posesCount = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            for (int i = 0; i < posesCount; i++)
            {
                int size = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);

                // 各ポーズをデコードし、MotionDataに追加
                var (type, timestamp, keypoints3d) = DataSerializer.Decode(data.Skip(offset).ToArray());

                // Decodeメソッドの更新が必要であれば、オフセットの調整をここで行う
                PoseData pose = new PoseData(timestamp, keypoints3d, type);
                motionData.AddPose(pose);

                // デコードしたポーズのバイトサイズをオフセットに加算（サイズの計算方法を適宜調整）
                offset += size;
            }

            return motionData;
        }
    }
}