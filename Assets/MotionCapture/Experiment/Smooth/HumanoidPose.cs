using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionCapture.Experiment
{
    public class HumanoidPose 
    {
        public Dictionary<HumanBodyBones, Queue<Quaternion>> queues;

        private int size;

        public HumanoidPose(int size = 10)
        {
            this.size = size;
            queues = new Dictionary<HumanBodyBones, Queue<Quaternion>>();
            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                queues.Add(bone, new Queue<Quaternion>());
            }
        }

        public void AddBoneRotation(HumanBodyBones bone, Quaternion value)
        {
            if (queues.TryGetValue(bone, out var queue)){
                queue.Enqueue(value);
                if (queue.Count > size)
                {
                    queue.Dequeue();
                }
            }
        }

        public Quaternion GetBoneRotation(HumanBodyBones bone)
        {
            if (queues.TryGetValue(bone, out var queue))
            {
                Quaternion[] values = queue.ToArray();
                if (values.Length == 0)
                {
                    return Quaternion.identity;
                }
                else
                {
                    Quaternion average = values[0];
                    float weight;
                    for (int i = 1; i < values.Length; i++)
                    {
                        weight = 1.0f / (i + 1);
                        average = Quaternion.Slerp(average, values[i], weight);
                    }
                    return average;
                }
            }
            else
            {
                return Quaternion.identity;
            }
        }
    }

}
