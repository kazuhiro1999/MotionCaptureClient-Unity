using System;
using UnityEngine;

namespace MotionCapture.Core
{
    public struct PoseData
    {
        public long timestamp;
        public Vector3[] positions;
        public PoseType type;

        public PoseData(long timestamp, Vector3[] keypoints3d, PoseType type)
        {
            this.timestamp = timestamp;
            this.positions = keypoints3d;
            this.type = type;           
        }

        public static PoseData operator +(PoseData a, PoseData b)
        {
            if (a.type != b.type)
                throw new InvalidOperationException("Cannot perform operations on different PoseTypes.");

            if (a.positions.Length != b.positions.Length)
                throw new InvalidOperationException("Vector arrays must have the same length.");

            var pose = new PoseData(a.timestamp + b.timestamp, new Vector3[a.positions.Length], a.type);
            for (int i = 0; i < pose.positions.Length; i++)
            {
                pose.positions[i] = a.positions[i] + b.positions[i];
            }
            return pose;
        }

        public static PoseData operator /(PoseData a, float b)
        {
            var pose = new PoseData(a.timestamp, new Vector3[a.positions.Length], a.type);
            for (int i = 0; i < pose.positions.Length; i++)
            {
                pose.positions[i] = a.positions[i] / b;
            }
            return pose;
        }
    }
}

