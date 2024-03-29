using UnityEngine;
using MotionCapture.Data;


namespace MotionCapture.Calibration
{
    public class AvatarCalibratorMediapipe : AvatarCalibrator
    {

        public override void Calibrate(PoseData pose) {

            var base_y = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position.y;

            // ì∑ëÃ
            var hip_pos = (pose.GetBone("left_hip").Position + pose.GetBone("right_hip").Position) / 2;
            var neck_pos = (pose.GetBone("left_shoulder").Position + pose.GetBone("right_shoulder").Position) / 2;
            var body_length = (neck_pos - hip_pos).magnitude;

            var body_multiplier = body_length / (anim.GetBoneTransform(HumanBodyBones.Neck).position - anim.GetBoneTransform(HumanBodyBones.Hips).position).magnitude;

            anim.GetBoneTransform(HumanBodyBones.Spine).localPosition *= body_multiplier;
            anim.GetBoneTransform(HumanBodyBones.Chest).localPosition *= body_multiplier;
            anim.GetBoneTransform(HumanBodyBones.UpperChest).localPosition *= body_multiplier;

            // éÒ
            //var head_pos = (pose.left_ear + pose.right_ear + pose.nose) / 3;
            //var neck_length = (head_pos - neck_pos).magnitude;

            //var neck_multiplier = neck_length / (anim.GetBoneTransform(HumanBodyBones.Head).position - anim.GetBoneTransform(HumanBodyBones.Neck).position).magnitude;

            //anim.GetBoneTransform(HumanBodyBones.Head).localPosition *= neck_multiplier;

            // å®ïù
            var body_width = (pose.GetBone("left_shoulder").Position - pose.GetBone("right_shoulder").Position).magnitude;
            var width_multiplier = body_width / (anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position - anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).localPosition *= width_multiplier;
            anim.GetBoneTransform(HumanBodyBones.RightUpperArm).localPosition *= width_multiplier;

            // ç∂òr
            var l_upperarm_length = (pose.GetBone("left_elbow").Position - pose.GetBone("left_shoulder").Position).magnitude;
            var l_upperarm_multiplier = l_upperarm_length / (anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).position - anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).localPosition *= l_upperarm_multiplier;

            var l_lowerarm_length = (pose.GetBone("left_elbow").Position - pose.GetBone("left_elbow").Position).magnitude;
            var l_lowerarm_multiplier = l_lowerarm_length / (anim.GetBoneTransform(HumanBodyBones.LeftHand).position - anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftHand).localPosition *= l_lowerarm_multiplier;

            // âEòr
            var r_upperarm_length = (pose.GetBone("right_elbow").Position - pose.GetBone("right_shoulder").Position).magnitude;
            var r_upperarm_multiplier = r_upperarm_length / (anim.GetBoneTransform(HumanBodyBones.RightLowerArm).position - anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightLowerArm).localPosition *= r_upperarm_multiplier;

            var r_lowerarm_length = (pose.GetBone("right_wrist").Position - pose.GetBone("right_elbow").Position).magnitude;
            var r_lowerarm_multiplier = r_lowerarm_length / (anim.GetBoneTransform(HumanBodyBones.RightHand).position - anim.GetBoneTransform(HumanBodyBones.RightLowerArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightHand).localPosition *= r_lowerarm_multiplier;

            // ç∂ãr
            var l_upperleg_length = (pose.GetBone("left_knee").Position - pose.GetBone("left_hip").Position).magnitude;
            var l_upperleg_multiplier = l_upperleg_length / (anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position - anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).localPosition *= l_upperleg_multiplier;

            var l_lowerleg_length = (pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position).magnitude;
            var l_lowerleg_multiplier = l_lowerleg_length / (anim.GetBoneTransform(HumanBodyBones.LeftFoot).position - anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftFoot).localPosition *= l_lowerleg_multiplier;

            // âEãr
            var r_upperleg_length = (pose.GetBone("right_knee").Position - pose.GetBone("right_hip").Position).magnitude;
            var r_upperleg_multiplier = r_upperleg_length / (anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).position - anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).localPosition *= r_upperleg_multiplier;

            var r_lowerleg_length = (pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position).magnitude;
            var r_lowerleg_multiplier = r_lowerleg_length / (anim.GetBoneTransform(HumanBodyBones.RightFoot).position - anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightFoot).localPosition *= r_lowerleg_multiplier;

            // ê⁄ín
            var y = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position.y;
            var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            hips.position = new Vector3(hips.position.x, hips.position.y - (y - base_y), hips.position.z);
        }
    }

}
