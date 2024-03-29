using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture.Calibration
{
    public abstract class AvatarCalibrator : MonoBehaviour
    {
        protected Animator anim;

        void Awake() {
            anim = GetComponent<Animator>();
        }

        public abstract void Calibrate(PoseData pose);
    }
}
