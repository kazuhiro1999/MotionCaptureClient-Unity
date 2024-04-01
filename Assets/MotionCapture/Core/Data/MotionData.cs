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

            // �|�[�Y�̐����G���R�[�h
            data.AddRange(BitConverter.GetBytes(Poses.Count));

            foreach (var pose in Poses)
            {
                // �e�|�[�Y���o�C�g�z��ɃG���R�[�h���A�ǉ�
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

            // �|�[�Y�̐����f�R�[�h
            int posesCount = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            for (int i = 0; i < posesCount; i++)
            {
                int size = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);

                // �e�|�[�Y���f�R�[�h���AMotionData�ɒǉ�
                var (type, timestamp, keypoints3d) = DataSerializer.Decode(data.Skip(offset).ToArray());

                // Decode���\�b�h�̍X�V���K�v�ł���΁A�I�t�Z�b�g�̒����������ōs��
                PoseData pose = new PoseData(timestamp, keypoints3d, type);
                motionData.AddPose(pose);

                // �f�R�[�h�����|�[�Y�̃o�C�g�T�C�Y���I�t�Z�b�g�ɉ��Z�i�T�C�Y�̌v�Z���@��K�X�����j
                offset += size;
            }

            return motionData;
        }
    }
}