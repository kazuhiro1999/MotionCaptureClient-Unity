using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MotionCapture.Data;

namespace MotionCapture
{
    public class SkeletonManager : MonoBehaviour
    {
        public float PointSize = 0.1f;
        private List<GameObject> points = new List<GameObject>();

        public void Apply(PoseData pose) {
            foreach (var bone in pose.Bones) {
                MovePoint(bone.Name, bone.Position);
            }

            // 破棄するポイントのリストを作成
            List<GameObject> toDestroy = new List<GameObject>();

            foreach (var point in points) {
                if (!pose.Bones.Select(x => x.Name).Contains(point.name))
                    toDestroy.Add(point);
            }

            // ポイントを破棄
            foreach (var dest in toDestroy) {
                points.Remove(dest);
                Destroy(dest);
            }
        }

        public void MovePoint(string name, Vector3 position) {
            if (!points.Select(x => x.name).Contains(name))
                AddPoint(name);

            var point = points.Find(x => x.name == name);
            point.transform.localPosition = position;
        }

        public void AddPoint(string name) {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = name;
            point.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
            point.transform.SetParent(transform);
            points.Add(point);
        }
    }
}

