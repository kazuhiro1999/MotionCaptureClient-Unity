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

        // Boneを追加する
        public void AddBone(BoneData bone) => Bones.Add(bone);

        // 名前からBoneを取得する
        public BoneData GetBone(string name) => Bones.Find(x => x.Name == name);

        // タイムスタンプを秒単位で取得
        public float GetTimeSecond() => (float)TimeStamp / 1000;

        // Boneの位置のX座標を反転（左右反転）
        public PoseData Left2Right() {
            foreach (var bone in Bones) {
                bone.Position.x = -bone.Position.x;
            }
            return this;
        }

        // インスタンスのコピーを作成
        public PoseData Copy() {
            return new PoseData {
                Type = this.Type,
                TimeStamp = this.TimeStamp,
                Bones = new List<BoneData>(this.Bones)
            };
        }

        // 演算子オーバーロード: 加算
        public static PoseData operator +(PoseData pose1, PoseData pose2) {
            if (pose1.Type != pose2.Type) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp + pose2.TimeStamp,
                Bones = CombineBones(pose1, pose2, (p1, p2) => p1 + p2)
            };
            return pose;
        }

        // 演算子オーバーロード: 減算
        public static PoseData operator -(PoseData pose1, PoseData pose2) {
            if (pose1.Type != pose2.Type) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp - pose2.TimeStamp,
                Bones = CombineBones(pose1, pose2, (p1, p2) => p1 - p2)
            };
            return pose;
        }

        // 演算子オーバーロード: 乗算
        public static PoseData operator *(PoseData pose1, int value) {
            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp * value,
                Bones = pose1.Bones.Select(bone => new BoneData(bone.Name, bone.Position * value)).ToList()
            };
            return pose;
        }

        // 演算子オーバーロード: 除算
        public static PoseData operator /(PoseData pose1, int value) {
            if (value == 0) return null;

            var pose = new PoseData {
                Type = pose1.Type,
                TimeStamp = pose1.TimeStamp / value,
                Bones = pose1.Bones.Select(bone => new BoneData(bone.Name, bone.Position / value)).ToList()
            };
            return pose;
        }

        // 2つのPoseDataのBoneの位置を結合するヘルパーメソッド
        private static List<BoneData> CombineBones(PoseData pose1, PoseData pose2, Func<Vector3, Vector3, Vector3> operation) {
            return pose1.Bones.Select(bone =>
                new BoneData(bone.Name, operation(bone.Position, pose2.GetBone(bone.Name).Position))
            ).ToList();
        }
    }
}
