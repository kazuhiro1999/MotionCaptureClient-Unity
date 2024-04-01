using UnityEngine;
using MotionCapture.Core;
using MotionCapture.Mediapipe;

namespace MotionCapture.Calibration
{
    public class AvatarCalibratorMediapipe : AvatarCalibrator
    {
        public Vector3 GetBonePosition(PoseData pose, Bones bone) => pose.positions[(int)bone];

        public override void Calibrate(PoseData pose) {

            var base_y = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position.y;

            var left_hip = GetBonePosition(pose, Bones.left_hip);
            var right_hip = GetBonePosition(pose, Bones.right_hip);
            var left_shoulder = GetBonePosition(pose, Bones.left_shoulder);
            var right_shoulder = GetBonePosition(pose, Bones.right_shoulder);
            var left_elbow = GetBonePosition(pose, Bones.left_elbow);
            var right_elbow = GetBonePosition(pose, Bones.right_elbow);
            var left_wrist = GetBonePosition(pose, Bones.left_wrist);
            var right_wrist = GetBonePosition(pose, Bones.right_wrist);
            var left_knee = GetBonePosition(pose, Bones.left_knee);
            var right_knee = GetBonePosition(pose, Bones.right_knee);
            var left_ankle = GetBonePosition(pose, Bones.left_ankle);
            var right_ankle = GetBonePosition(pose, Bones.right_ankle);
            var left_toe = GetBonePosition(pose, Bones.left_toe);
            var right_toe = GetBonePosition(pose, Bones.right_toe);
            var left_heel = GetBonePosition(pose, Bones.left_heel);
            var right_heel = GetBonePosition(pose, Bones.right_heel);

            // ì∑ëÃ
            var hip = (left_hip + right_hip) / 2;
            var neck = (left_shoulder + right_shoulder) / 2;
            var body_length = (neck - hip).magnitude;

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
            var body_width = (left_shoulder - right_shoulder).magnitude;
            var width_multiplier = body_width / (anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position - anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).localPosition *= width_multiplier;
            anim.GetBoneTransform(HumanBodyBones.RightUpperArm).localPosition *= width_multiplier;

            // ç∂òr
            var l_upperarm_length = (left_elbow - left_shoulder).magnitude;
            var l_upperarm_multiplier = l_upperarm_length / (anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).position - anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).localPosition *= l_upperarm_multiplier;

            var l_lowerarm_length = (left_wrist - left_elbow).magnitude;
            var l_lowerarm_multiplier = l_lowerarm_length / (anim.GetBoneTransform(HumanBodyBones.LeftHand).position - anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftHand).localPosition *= l_lowerarm_multiplier;

            // âEòr
            var r_upperarm_length = (right_elbow - right_shoulder).magnitude;
            var r_upperarm_multiplier = r_upperarm_length / (anim.GetBoneTransform(HumanBodyBones.RightLowerArm).position - anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightLowerArm).localPosition *= r_upperarm_multiplier;

            var r_lowerarm_length = (right_wrist - right_elbow).magnitude;
            var r_lowerarm_multiplier = r_lowerarm_length / (anim.GetBoneTransform(HumanBodyBones.RightHand).position - anim.GetBoneTransform(HumanBodyBones.RightLowerArm).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightHand).localPosition *= r_lowerarm_multiplier;

            // ç∂ãr
            var l_upperleg_length = (left_knee - left_hip).magnitude;
            var l_upperleg_multiplier = l_upperleg_length / (anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position - anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).localPosition *= l_upperleg_multiplier;

            var l_lowerleg_length = (left_ankle - left_knee).magnitude;
            var l_lowerleg_multiplier = l_lowerleg_length / (anim.GetBoneTransform(HumanBodyBones.LeftFoot).position - anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.LeftFoot).localPosition *= l_lowerleg_multiplier;

            // âEãr
            var r_upperleg_length = (right_knee - right_hip).magnitude;
            var r_upperleg_multiplier = r_upperleg_length / (anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).position - anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).localPosition *= r_upperleg_multiplier;

            var r_lowerleg_length = (right_ankle - right_knee).magnitude;
            var r_lowerleg_multiplier = r_lowerleg_length / (anim.GetBoneTransform(HumanBodyBones.RightFoot).position - anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).position).magnitude;
            anim.GetBoneTransform(HumanBodyBones.RightFoot).localPosition *= r_lowerleg_multiplier;

            // ê⁄ín
            var y = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position.y;
            var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            hips.position = new Vector3(hips.position.x, hips.position.y - (y - base_y), hips.position.z);
        }
    }

}
