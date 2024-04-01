using System;
using System.Collections.Generic;
using UnityEngine;

namespace MotionCapture.Core
{
    public class PoseController : MonoBehaviour
    {
        public DataReceiver receiver;
        protected Animator avatar;
        protected Dictionary<HumanBodyBones, Quaternion> initialRotations;

        public PoseType PoseType;
        public bool IsMirror;

        private FKSolver solver;

        void Awake()
        {
            avatar = GetComponent<Animator>();

            initialRotations = new Dictionary<HumanBodyBones, Quaternion>();
            if (avatar != null && avatar.isHuman)
            {
                foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    if (bone == HumanBodyBones.LastBone) break;

                    var t = avatar.GetBoneTransform(bone);
                    if (t != null)
                    {
                        initialRotations.Add(bone, t.rotation);
                    }
                }
            }

            switch (PoseType)
            {
                case PoseType.MediapipePose: { 
                        solver = gameObject.AddComponent<Mediapipe.FKSolver>();
                        receiver.OnDataReceived.AddListener(ApplyPose);
                        break; 
                    }
                case PoseType.RTMPose: { 
                        solver = gameObject.AddComponent<RTMPose.FKSolver>();
                        receiver.OnDataReceived.AddListener(ApplyPose);
                        break; 
                    }
                default: { break; }
            }
        }

        public void ApplyPose(PoseData pose)
        {
            if (solver)
            {
                solver.IsMirror = IsMirror;
                solver.Apply(pose);
            }            
        }
    }
}
