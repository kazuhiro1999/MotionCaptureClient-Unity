using MotionCapture.Core;
using MotionCapture.RTMPose;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MotionCapture.Experiment
{
    public class VRWalkDefault : MonoBehaviour
    {
        [SerializeField] DataReceiver receiver;

        public bool Enabled;

        public int QueueSize;

        private Queue left_foot_positions;
        private Queue right_foot_positions;
        private Queue directions;
        private Queue movements;
        public bool is_left_foot_grounded;
        public bool is_right_foot_grounded;

        private Vector3 left_foot_position;
        private Vector3 right_foot_position;
        private Vector3 previous_left_foot_position;
        private Vector3 previous_right_foot_position;

        private float previousDistance;

        private void Start()
        {
            left_foot_positions = new Queue(QueueSize);
            right_foot_positions = new Queue(QueueSize);
            directions = new Queue(QueueSize);
            movements = new Queue(1);

            receiver.OnDataReceived.AddListener(OnDataReceived);
        }

        private void OnDataReceived(PoseData pose)
        {
            var left_hip = pose.positions[(int)Bone.left_hip];
            var right_hip = pose.positions[(int)Bone.right_hip];
            var left_shoulder = pose.positions[(int)Bone.left_shoulder];
            var right_shoulder = pose.positions[(int)Bone.right_shoulder];
            var neck = (left_shoulder + right_shoulder) / 2;
            var left_ankle = pose.positions[(int)Bone.left_toe];
            var right_ankle = pose.positions[(int)Bone.right_toe];

            // 体の向き
            var hip_forward = Vector3.Cross(left_hip - neck, right_hip - neck).normalized;
            var direction = hip_forward;
            direction.y = 0;
            direction = direction.normalized;

            left_foot_positions.Enqueue(left_ankle);
            right_foot_positions.Enqueue(right_ankle);
            directions.Enqueue(direction);

            left_foot_position = left_foot_positions.GetValue();
            right_foot_position = right_foot_positions.GetValue();

            // 2点間のベクトルを計算
            Vector3 vectorAB = left_foot_positions.GetValue() - right_foot_positions.GetValue();

            // ベクトルABをベクトルDに投影する
            float distance = Vector3.Dot(vectorAB, directions.GetValue()); // 右足が前 -> 負

            float delta = distance - previousDistance;

            is_left_foot_grounded = delta < 0;
            is_right_foot_grounded = delta > 0;

            if (is_left_foot_grounded && is_right_foot_grounded) // 両足が接地
            {
                movements.Enqueue(Vector3.zero);
            }
            else if (is_left_foot_grounded) // 左足のみ接地
            {
                var diff = previous_left_foot_position - left_foot_position;
                diff.y = 0;
                movements.Enqueue(diff);
                if (Enabled)
                {
                    var move = movements.GetValue();
                    transform.position += move;
                }
            }
            else if (is_right_foot_grounded) // 右足のみ接地
            {
                var diff = previous_right_foot_position - right_foot_position;
                diff.y = 0;
                movements.Enqueue(diff);
                if (Enabled)
                {
                    var move = movements.GetValue();
                    transform.position += move;
                }
            }
            else // 両足とも空中
            {

            }

            previous_left_foot_position = left_foot_position;
            previous_right_foot_position = right_foot_position;
            previousDistance = distance;
        }

        private class Queue : List<Vector3>
        {
            public int Size;

            public Queue(int size)
            {
                Size = size;
            }

            public void Enqueue(Vector3 value)
            {
                base.Add(value);
                if (Count > Size)
                {
                    lock (this)
                    {
                        RemoveAt(0);
                    }
                }
            }

            public Vector3 GetValue()
            {
                if (Count == 0) return Vector3.zero;

                Vector3 sum = Vector3.zero;
                for (int i = 0; i < Count; i++)
                {
                    sum += this[i];
                }
                return sum / Count;
            }
        }
    }

}
