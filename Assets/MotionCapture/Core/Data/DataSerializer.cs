using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MotionCapture.Core
{
    public class DataSerializer
    {
        public static byte[] Encode(long timestamp, Vector3[] keypoints3d, PoseType type)
        {
            int n_joints = keypoints3d.Length;
            int dataSize = sizeof(byte) + sizeof(long) + sizeof(ushort) + n_joints * sizeof(float) * 3;
            byte[] data = new byte[dataSize];

            int offset = 0;

            // pose_typeをバイトデータとしてパック
            data[offset] = (byte)type;
            offset += sizeof(byte);

            // timestampをバイトデータとしてパック
            Buffer.BlockCopy(BitConverter.GetBytes(timestamp), 0, data, offset, sizeof(long));
            offset += sizeof(long);

            // n_jointsをバイトデータとしてパック
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)n_joints), 0, data, offset, sizeof(ushort));
            offset += sizeof(ushort);

            // positionsをバイトデータとしてパック
            foreach (Vector3 point in keypoints3d)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(point.x), 0, data, offset, sizeof(float));
                offset += sizeof(float);
                Buffer.BlockCopy(BitConverter.GetBytes(point.y), 0, data, offset, sizeof(float));
                offset += sizeof(float);
                Buffer.BlockCopy(BitConverter.GetBytes(point.z), 0, data, offset, sizeof(float));
                offset += sizeof(float);
            }

            return data;
        }
        public static (PoseType type, long timestamp, Vector3[] keypoints3d) Decode(byte[] data)
        {
            int offset = 0;

            // pose_typeをアンパック
            PoseType type = (PoseType)data[offset];
            offset += sizeof(byte);

            // timestampをアンパック
            long timestamp = BitConverter.ToInt64(data, offset);
            offset += sizeof(long);

            // n_jointsをアンパック
            ushort n_joints = BitConverter.ToUInt16(data, offset);
            offset += sizeof(ushort);

            Vector3[] keypoints3d = new Vector3[n_joints];

            // positionsをアンパック
            for (int i = 0; i < n_joints; i++)
            {
                float x = BitConverter.ToSingle(data, offset);
                offset += sizeof(float);
                float y = BitConverter.ToSingle(data, offset);
                offset += sizeof(float);
                float z = BitConverter.ToSingle(data, offset);
                offset += sizeof(float);
                keypoints3d[i] = new Vector3(x, y, z);
            }

            return (type, timestamp, keypoints3d);
        }

        public static byte[] EncodeMotion(MotionData motionData)
        {
            List<byte> data = new List<byte>();

            // ポーズの数をエンコード
            data.AddRange(BitConverter.GetBytes(motionData.Poses.Count));

            foreach (var pose in motionData.Poses)
            {
                // 各ポーズをバイト配列にエンコードし、追加
                byte[] encodedPose = Encode(pose.timestamp, pose.positions, pose.type);

                data.AddRange(BitConverter.GetBytes(encodedPose.Length));
                data.AddRange(encodedPose);
            }

            return data.ToArray();
        }

        public static MotionData DecodeMotionData(byte[] data)
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
                var (type, timestamp, keypoints3d) = Decode(data.Skip(offset).ToArray());

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
