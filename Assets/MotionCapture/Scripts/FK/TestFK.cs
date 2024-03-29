using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture
{
    public class TestFK : BaseFK
{
        public override void Forward(PoseData pose) {
            Debug.Log(pose.TimeStamp);
        }
    }

}
