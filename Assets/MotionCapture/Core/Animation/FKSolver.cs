using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionCapture.Core
{
    public abstract class FKSolver : MonoBehaviour
    {
        protected Animator anim;
        protected Dictionary<HumanBodyBones, Quaternion> _initialRotations;

        public bool IsMirror;

        void Awake()
        {
            anim = GetComponent<Animator>();
            _initialRotations = new Dictionary<HumanBodyBones, Quaternion>();
            if (anim != null && anim.isHuman)
            {
                foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    if (bone == HumanBodyBones.LastBone) break;

                    var t = anim.GetBoneTransform(bone);
                    if (t != null)
                    {
                        _initialRotations.Add(bone, t.rotation);
                    }
                }
            }
        }

        public abstract void Apply(PoseData pose);

        protected void ApplyBoneRotation(HumanBodyBones bone, Quaternion rotation)
        {
            if (!_initialRotations.TryGetValue(bone, out var rot))
            {
                rot = Quaternion.identity;
            }
            anim.GetBoneTransform(bone).rotation = rot * rotation;
        }
    }
}

