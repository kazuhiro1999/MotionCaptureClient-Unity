using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MotionCapture.Core;
using System;

namespace MotionCapture
{
    public class SkeletonManager : MonoBehaviour
    {
        public float PointSize = 0.1f;
        private Dictionary<string, GameObject> points = new Dictionary<string, GameObject>();

        public void Apply(PoseData pose) {
            for (int i = 0; i < pose.positions.Length; i++)
            {
                MovePoint(i.ToString(), pose.positions[i]);
            }
        }

        public void MovePoint(string bone, Vector3 position) {
            if (!points.TryGetValue(bone, out var point))
            {
                point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.name = bone.ToString();
                point.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
                point.transform.SetParent(transform);
                points.Add(bone, point);
            }

            point.transform.localPosition = position;
        }
    }
}

