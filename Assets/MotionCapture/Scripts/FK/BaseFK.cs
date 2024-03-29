using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture
{
    // Forward Kinematics 適用のための基底クラス
    public abstract class BaseFK : MonoBehaviour
    {
        protected Animator anim;
        public bool IsMirror;

        void Awake() {
            anim = GetComponent<Animator>();
        }

        // Poseをアバターに適用する
        public abstract void Forward(PoseData pose);
    }

}
