using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MotionCapture.Data
{
    [System.Serializable]
    public class PoseData
    {
        public string Type;
        public long TimeStamp;
        public List<BoneData> Bones = new List<BoneData>();

        // Bone��ǉ�����
        public void AddBone(BoneData bone) => Bones.Add(bone);

        // ���O����Bone���擾����
        public BoneData GetBone(string name) => Bones.Find(x => x.Name == name);

        // �^�C���X�^���v��b�P�ʂŎ擾
        public float GetTimeSecond() => (float)TimeStamp / 1000;

        // Bone�̈ʒu��X���W�𔽓]�i���E���]�j
        public PoseData Left2Right() {
            foreach (var bone in Bones) {
                bone.Position.x = -bone.Position.x;
            }
            return this;
        }

        // �C���X�^���X�̃R�s�[���쐬
        public PoseData Copy() {
            return new PoseData {
                Type = this.Type,
                TimeStamp = this.TimeStamp,
                Bones = new List<BoneData>(this.Bones)
            };
        }

        // ���Z�q�I�[�o�[���[�h: ���Z
        public static PoseData operator +(PoseData pose1, PoseData pose2) {
            if (pose1.Type != pose2.Type) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp + pose2.TimeStamp,
                Bones = CombineBones(pose1, pose2, (p1, p2) => p1 + p2)
            };
            return pose;
        }

        // ���Z�q�I�[�o�[���[�h: ���Z
        public static PoseData operator -(PoseData pose1, PoseData pose2) {
            if (pose1.Type != pose2.Type) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp - pose2.TimeStamp,
                Bones = CombineBones(pose1, pose2, (p1, p2) => p1 - p2)
            };
            return pose;
        }

        // ���Z�q�I�[�o�[���[�h: ��Z
        public static PoseData operator *(PoseData pose1, int value) {
            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp * value,
                Bones = pose1.Bones.Select(bone => new BoneData(bone.Name, bone.Position * value)).ToList()
            };
            return pose;
        }

        // ���Z�q�I�[�o�[���[�h: ���Z
        public static PoseData operator /(PoseData pose1, int value) {
            if (value == 0) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp / value,
                Bones = pose1.Bones.Select(bone => new BoneData(bone.Name, bone.Position / value)).ToList()
            };
            return pose;
        }

        // 2��PoseData��Bone�̈ʒu����������w���p�[���\�b�h
        private static List<BoneData> CombineBones(PoseData pose1, PoseData pose2, Func<Vector3, Vector3, Vector3> operation) {
            return pose1.Bones.Select(bone =>
                new BoneData(bone.Name, operation(bone.Position, pose2.GetBone(bone.Name).Position))
            ).ToList();
        }
    }
}
