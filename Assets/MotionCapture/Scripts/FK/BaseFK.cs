using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture
{
    // Forward Kinematics �K�p�̂��߂̊��N���X
    public abstract class BaseFK : MonoBehaviour
    {
        protected Animator anim;
        public bool IsMirror;

        void Awake() {
            anim = GetComponent<Animator>();
        }

        // Pose���A�o�^�[�ɓK�p����
        public abstract void Forward(PoseData pose);
    }

}
