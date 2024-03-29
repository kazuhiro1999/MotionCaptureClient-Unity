using UnityEngine;
using MotionCapture.Data;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace MotionCapture
{
    public class MediapipeFK : BaseFK
    {
        public override void Forward(PoseData pose) {

            if (IsMirror) {
                pose.Left2Right();
            }
            RotateAll(pose);
            /*RotateBody(pose);
            RotateHead(pose);
            RotateLeftArm(pose);
            RotateRightArm(pose);
            RotateLeftLeg(pose);
            RotateRightLeg(pose);*/
        }

        void RotateBody(PoseData pose) {
            var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            var hip_pos = (pose.GetBone("left_hip").Position + pose.GetBone("right_hip").Position) / 2;
            hips.localPosition = hip_pos;

            var neck_pos = (pose.GetBone("left_shoulder").Position + pose.GetBone("right_shoulder").Position) / 2;

            var forward = transform.rotation * Vector3.Cross(pose.GetBone("left_hip").Position - neck_pos, pose.GetBone("right_hip").Position - neck_pos).normalized;
            var right = transform.rotation * (pose.GetBone("right_hip").Position - pose.GetBone("left_hip").Position).normalized;
            var up = Vector3.Cross(forward, right).normalized;

            hips.forward = forward;
            //hips.right = transform.rotation * right;
            var angle = Vector3.SignedAngle(hips.right, right, up);
            hips.rotation = Quaternion.AngleAxis(angle, up) * hips.rotation;

            var upperchest = anim.GetBoneTransform(HumanBodyBones.UpperChest);
            var body_forward = transform.rotation * Vector3.Cross(pose.GetBone("right_shoulder").Position - hip_pos, pose.GetBone("left_shoulder").Position - hip_pos).normalized;
            upperchest.forward = body_forward;
            var body_right = transform.rotation * (pose.GetBone("right_shoulder").Position - pose.GetBone("left_shoulder").Position).normalized;
            var body_up = Vector3.Cross(body_forward, body_right);
            var angle2 = Vector3.SignedAngle(upperchest.right, body_right, body_up);
            upperchest.rotation = Quaternion.AngleAxis(angle2, body_up) * upperchest.rotation;
        }
        void RotateHead(PoseData pose) {
            var neck = anim.GetBoneTransform(HumanBodyBones.Neck);
            var head_pos = (pose.GetBone("left_ear").Position + pose.GetBone("right_ear").Position + pose.GetBone("nose").Position) / 3;
            var neck_pos = (pose.GetBone("left_shoulder").Position + pose.GetBone("right_shoulder").Position) / 2;
            neck.up = transform.rotation * (head_pos - neck_pos).normalized;

            var head = anim.GetBoneTransform(HumanBodyBones.Head);
            var mouth_mid = (pose.GetBone("left_mouth").Position + pose.GetBone("right_mouth").Position) / 2;
            var eye_mid = (pose.GetBone("left_eye").Position + pose.GetBone("right_eye").Position) / 2;
            var up = transform.rotation * (eye_mid - mouth_mid).normalized;
            var forward = transform.rotation * Vector3.Cross(pose.GetBone("right_eye").Position - mouth_mid, pose.GetBone("left_eye").Position - mouth_mid).normalized;
            head.forward = forward;
            var angle = Vector3.SignedAngle(head.up, up, forward);
            head.rotation = head.rotation * Quaternion.AngleAxis(angle, forward);
        }
        void RotateLeftArm(PoseData pose) {
            var l_shoulder = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);

            var l_upperarm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            l_upperarm.right = transform.rotation * -(pose.GetBone("left_elbow").Position - pose.GetBone("left_shoulder").Position).normalized; ;

            // ã˜r‚Ì‰ñ“]
            var l_arm_up = transform.rotation * -Vector3.Cross(pose.GetBone("left_shoulder").Position - pose.GetBone("left_elbow").Position, pose.GetBone("left_wrist").Position - pose.GetBone("left_elbow").Position);
            var angle = Vector3.SignedAngle(l_upperarm.up, l_arm_up, l_upperarm.right);
            l_upperarm.rotation = Quaternion.AngleAxis(angle, l_upperarm.right) * l_upperarm.rotation;

            var l_lowerarm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            l_lowerarm.right = transform.rotation * (pose.GetBone("left_elbow").Position - pose.GetBone("left_wrist").Position).normalized;

            // ŽèŽñ‚Ì‰ñ“]
            var hand_up = transform.rotation * -Vector3.Cross(pose.GetBone("left_inner_hand").Position - pose.GetBone("left_wrist").Position, pose.GetBone("left_outer_hand").Position - pose.GetBone("left_wrist").Position).normalized;
            var hand_right = transform.rotation * -(pose.GetBone("left_hand_tip").Position - pose.GetBone("left_wrist").Position).normalized;
            var hand_forward = -Vector3.Cross(hand_up, hand_right).normalized;
            angle = Vector3.SignedAngle(l_lowerarm.forward, hand_forward, l_lowerarm.right);
            l_lowerarm.rotation = Quaternion.AngleAxis(angle, l_lowerarm.right) * l_lowerarm.rotation;

            var l_hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            l_hand.right = transform.rotation * -(pose.GetBone("left_hand_tip").Position - pose.GetBone("left_wrist").Position).normalized;
        }
        void RotateRightArm(PoseData pose) {
            var r_shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);

            var r_upperarm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            r_upperarm.right = transform.rotation * (pose.GetBone("right_elbow").Position - pose.GetBone("right_shoulder").Position).normalized; ;

            // ã˜r‚Ì‰ñ“]
            var r_arm_up = transform.rotation * Vector3.Cross(pose.GetBone("right_shoulder").Position - pose.GetBone("right_elbow").Position, pose.GetBone("right_wrist").Position - pose.GetBone("right_elbow").Position);
            var angle = Vector3.SignedAngle(r_upperarm.up, r_arm_up, r_upperarm.right);
            r_upperarm.rotation = Quaternion.AngleAxis(angle, r_upperarm.right) * r_upperarm.rotation;

            var r_lowerarm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            r_lowerarm.right = transform.rotation * -(pose.GetBone("right_elbow").Position - pose.GetBone("right_wrist").Position).normalized;

            // ŽèŽñ‚Ì‰ñ“]
            var hand_up = transform.rotation * Vector3.Cross(pose.GetBone("right_inner_hand").Position - pose.GetBone("right_wrist").Position, pose.GetBone("right_outer_hand").Position - pose.GetBone("right_wrist").Position).normalized;
            var hand_right = transform.rotation * (pose.GetBone("right_hand_tip").Position - pose.GetBone("right_wrist").Position).normalized;
            var hand_forward = -Vector3.Cross(hand_up, hand_right).normalized;
            angle = Vector3.SignedAngle(r_lowerarm.forward, hand_forward, r_lowerarm.right);
            r_lowerarm.rotation = Quaternion.AngleAxis(angle, r_lowerarm.right) * r_lowerarm.rotation;

            var r_hand = anim.GetBoneTransform(HumanBodyBones.RightHand);
            r_hand.right = transform.rotation * (pose.GetBone("right_hand_tip").Position - pose.GetBone("right_wrist").Position).normalized;
        }
        void RotateLeftLeg(PoseData pose) {
            var l_upperleg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            l_upperleg.up = transform.rotation * -(pose.GetBone("left_knee").Position - pose.GetBone("left_hip").Position).normalized;

            // ã‘Ú‚Ì‰ñ“]
            //var leg_right = transform.rotation * -Vector3.Cross(pose.L_UpperLeg - pose.L_LowerLeg, pose.L_Foot - pose.L_LowerLeg).normalized;
            var leg_right = transform.rotation * Vector3.Cross(pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_toe").Position).normalized;
            var angle = Vector3.SignedAngle(l_upperleg.right, leg_right, l_upperleg.up);
            l_upperleg.rotation = Quaternion.AngleAxis(angle, l_upperleg.up) * l_upperleg.rotation;

            var l_lowerleg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            angle = Vector3.Angle(pose.GetBone("left_hip").Position - pose.GetBone("left_knee").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position);
            l_lowerleg.localRotation = Quaternion.Euler(180 - angle, 0f, 0f);

            var l_foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            var foot_forward = transform.rotation * (pose.GetBone("left_toe").Position - pose.GetBone("left_ankle").Position).normalized;
            l_foot.forward = foot_forward;
            var foot_right = transform.rotation * Vector3.Cross(pose.GetBone("left_toe").Position - pose.GetBone("left_ankle").Position, pose.GetBone("left_heel").Position - pose.GetBone("left_ankle").Position).normalized;
            angle = Vector3.SignedAngle(l_foot.right, foot_right, foot_forward);
            l_foot.rotation = Quaternion.AngleAxis(angle, foot_forward) * l_foot.rotation;
        }
        void RotateRightLeg(PoseData pose) {
            var r_upperleg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            r_upperleg.up = transform.rotation * -(pose.GetBone("right_knee").Position - pose.GetBone("right_hip").Position).normalized;

            // ã‘Ú‚Ì‰ñ“]
            var leg_right = transform.rotation * -Vector3.Cross(pose.GetBone("right_hip").Position - pose.GetBone("right_knee").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position).normalized;
            //var leg_right = transform.rotation * Vector3.Cross(pose.R_Foot - pose.R_LowerLeg, pose.R_Foot - pose.R_Toe).normalized;
            var angle = Vector3.SignedAngle(r_upperleg.right, leg_right, r_upperleg.up);
            r_upperleg.rotation = Quaternion.AngleAxis(angle, r_upperleg.up) * r_upperleg.rotation;

            var r_lowerleg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            angle = Vector3.Angle(pose.GetBone("right_hip").Position - pose.GetBone("right_knee").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position);
            r_lowerleg.localRotation = Quaternion.Euler(180 - angle, 0f, 0f);

            var r_foot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
            var foot_forward = transform.rotation * (pose.GetBone("right_toe").Position - pose.GetBone("right_ankle").Position).normalized;
            r_foot.forward = foot_forward;
            var foot_right = transform.rotation * Vector3.Cross(pose.GetBone("right_toe").Position - pose.GetBone("right_ankle").Position, pose.GetBone("right_heel").Position - pose.GetBone("right_ankle").Position).normalized;
            angle = Vector3.SignedAngle(r_foot.right, foot_right, foot_forward);
            r_foot.rotation = Quaternion.AngleAxis(angle, foot_forward) * r_foot.rotation;
        }

        void RotateAll(PoseData pose) {
            var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            var hip_pos = (pose.GetBone("left_hip").Position + pose.GetBone("right_hip").Position) / 2;
            hips.localPosition = hip_pos;

            var neck_pos = (pose.GetBone("left_shoulder").Position + pose.GetBone("right_shoulder").Position) / 2;

            // Body rotation
            var body_forward = Vector3.Cross(pose.GetBone("left_hip").Position - neck_pos, pose.GetBone("right_hip").Position - neck_pos).normalized;
            var body_right = (pose.GetBone("right_hip").Position - pose.GetBone("left_hip").Position).normalized;
            var body_up = Vector3.Cross(body_forward, body_right).normalized;
            hips.rotation = transform.rotation * Quaternion.LookRotation(body_forward, body_up);

            var upperchest = anim.GetBoneTransform(HumanBodyBones.UpperChest);
            var upperchest_forward = Vector3.Cross(pose.GetBone("right_shoulder").Position - hip_pos, pose.GetBone("left_shoulder").Position - hip_pos).normalized;
            var upperchest_right = (pose.GetBone("right_shoulder").Position - pose.GetBone("left_shoulder").Position).normalized;
            var upperchest_up = Vector3.Cross(upperchest_forward, upperchest_right);
            upperchest.rotation = transform.rotation * Quaternion.LookRotation(upperchest_forward, upperchest_up);

            // Head rotation
            var neck = anim.GetBoneTransform(HumanBodyBones.Neck);
            var head_pos = (pose.GetBone("left_ear").Position + pose.GetBone("right_ear").Position + pose.GetBone("nose").Position) / 3;
            var neck_up = (head_pos - neck_pos).normalized;
            neck.rotation = transform.rotation * Quaternion.LookRotation(upperchest_forward, neck_up);

            var head = anim.GetBoneTransform(HumanBodyBones.Head);
            var mouth_mid = (pose.GetBone("left_mouth").Position + pose.GetBone("right_mouth").Position) / 2;
            var eye_mid = (pose.GetBone("left_eye").Position + pose.GetBone("right_eye").Position) / 2;
            var head_up = (eye_mid - mouth_mid).normalized;
            var head_forward = Vector3.Cross(pose.GetBone("right_eye").Position - mouth_mid, pose.GetBone("left_eye").Position - mouth_mid).normalized;
            head.rotation = transform.rotation * Quaternion.LookRotation(head_forward, head_up);

            // Left arm rotation
            var l_upperarm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            var l_upperarm_right = -(pose.GetBone("left_elbow").Position - pose.GetBone("left_shoulder").Position).normalized;
            var l_upperarm_up = -Vector3.Cross(pose.GetBone("left_shoulder").Position - pose.GetBone("left_elbow").Position, pose.GetBone("left_wrist").Position - pose.GetBone("left_elbow").Position);
            var l_upperarm_forward = -Vector3.Cross(l_upperarm_up, l_upperarm_right);
            l_upperarm_up = Vector3.Cross(l_upperarm_forward, l_upperarm_right);
            var l_upperarm_rotation = Quaternion.LookRotation(l_upperarm_forward, l_upperarm_up);
            l_upperarm.rotation = transform.rotation * l_upperarm_rotation;

            var l_lowerarm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            var l_lowerarm_right = (pose.GetBone("left_elbow").Position - pose.GetBone("left_wrist").Position).normalized;
            var l_lowerarm_up = Vector3.Cross(pose.GetBone("left_elbow").Position - pose.GetBone("left_wrist").Position, pose.GetBone("left_hand_tip").Position - pose.GetBone("left_wrist").Position);
            var l_lowerarm_forward = Vector3.Cross(l_lowerarm_up, l_lowerarm_right);
            l_lowerarm_up = Vector3.Cross(l_lowerarm_forward, l_lowerarm_right);
            var l_lowerarm_rotation = Quaternion.LookRotation(l_lowerarm_forward, l_lowerarm_up);
            l_lowerarm.rotation = transform.rotation * l_lowerarm_rotation;

            var l_hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            var l_hand_right = -(pose.GetBone("left_hand_tip").Position - pose.GetBone("left_wrist").Position).normalized;
            var l_hand_forward = (pose.GetBone("left_inner_hand").Position - pose.GetBone("left_outer_hand").Position).normalized;
            var l_hand_up = Vector3.Cross(l_hand_forward, l_hand_right);
            var l_hand_rotation = Quaternion.LookRotation(l_hand_forward, l_hand_up);
            l_hand.rotation = transform.rotation * l_hand_rotation;

            // right arm rotation
            var r_upperarm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            var r_upperarm_right = (pose.GetBone("right_elbow").Position - pose.GetBone("right_shoulder").Position).normalized;
            var r_upperarm_up = Vector3.Cross(pose.GetBone("right_shoulder").Position - pose.GetBone("right_elbow").Position, pose.GetBone("right_wrist").Position - pose.GetBone("right_elbow").Position);
            var r_upperarm_forward = -Vector3.Cross(r_upperarm_up, r_upperarm_right);
            r_upperarm_up = Vector3.Cross(r_upperarm_forward, r_upperarm_right);
            var r_upperarm_rotation = Quaternion.LookRotation(r_upperarm_forward, r_upperarm_up);
            r_upperarm.rotation = transform.rotation * r_upperarm_rotation;

            var r_lowerarm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            var r_lowerarm_right = -(pose.GetBone("right_elbow").Position - pose.GetBone("right_wrist").Position).normalized;
            var r_lowerarm_up = Vector3.Cross(pose.GetBone("right_elbow").Position - pose.GetBone("right_wrist").Position, pose.GetBone("right_hand_tip").Position - pose.GetBone("right_wrist").Position);
            var r_lowerarm_forward = Vector3.Cross(r_lowerarm_up, r_lowerarm_right);
            r_lowerarm_up = Vector3.Cross(r_lowerarm_forward, r_lowerarm_right);
            var r_lowerarm_rotation = Quaternion.LookRotation(r_lowerarm_forward, r_lowerarm_up);
            r_lowerarm.rotation = transform.rotation * r_lowerarm_rotation;

            var r_hand = anim.GetBoneTransform(HumanBodyBones.RightHand);
            var r_hand_right = (pose.GetBone("right_hand_tip").Position - pose.GetBone("right_wrist").Position).normalized;
            var r_hand_forward = (pose.GetBone("right_inner_hand").Position - pose.GetBone("right_outer_hand").Position).normalized;
            var r_hand_up = Vector3.Cross(r_hand_forward, r_hand_right);
            var r_hand_rotation = Quaternion.LookRotation(r_hand_forward, r_hand_up);
            r_hand.rotation = transform.rotation * r_hand_rotation;

            // Left leg rotation
            var l_upperleg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            var l_upperleg_up = -(pose.GetBone("left_knee").Position - pose.GetBone("left_hip").Position).normalized;
            var l_upperleg_right = -Vector3.Cross(pose.GetBone("left_hip").Position - pose.GetBone("left_knee").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position);
            var l_knee_angle = Vector3.Angle(pose.GetBone("left_hip").Position - pose.GetBone("left_knee").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position);
            if (l_knee_angle > 160) {
                l_upperleg_right = -Vector3.Cross(pose.GetBone("left_heel").Position - pose.GetBone("left_hip").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_hip").Position);
            }
            var l_upperleg_forward = -Vector3.Cross(l_upperleg_up, l_upperleg_right);
            var l_upperleg_rotation = Quaternion.LookRotation(l_upperleg_forward, l_upperleg_up);
            l_upperleg.rotation = transform.rotation * l_upperleg_rotation;

            var l_lowerleg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            var l_lowerleg_angle = Vector3.Angle(pose.GetBone("left_hip").Position - pose.GetBone("left_knee").Position, pose.GetBone("left_ankle").Position - pose.GetBone("left_knee").Position);
            l_lowerleg.localRotation = Quaternion.Euler(180 - l_lowerleg_angle, 0f, 0f);

            var l_foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            var l_foot_forward = (pose.GetBone("left_toe").Position - pose.GetBone("left_ankle").Position).normalized;
            var l_foot_right = Vector3.Cross(pose.GetBone("left_toe").Position - pose.GetBone("left_ankle").Position, pose.GetBone("left_heel").Position - pose.GetBone("left_ankle").Position).normalized;
            var l_foot_up = Vector3.Cross(l_foot_forward, l_foot_right);
            var l_foot_rotation = Quaternion.LookRotation(l_foot_forward, l_foot_up);
            l_foot.rotation = transform.rotation * l_foot_rotation;

            // Right leg rotation
            var r_upperleg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            var r_upperleg_up = -(pose.GetBone("right_knee").Position - pose.GetBone("right_hip").Position).normalized;
            var r_upperleg_right = -Vector3.Cross(pose.GetBone("right_hip").Position - pose.GetBone("right_knee").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position);
            var r_knee_angle = Vector3.Angle(pose.GetBone("right_hip").Position - pose.GetBone("right_knee").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position);
            if (r_knee_angle > 160) {
                r_upperleg_right = -Vector3.Cross(pose.GetBone("right_heel").Position - pose.GetBone("right_hip").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_hip").Position);
            }
            var r_upperleg_forward = -Vector3.Cross(r_upperleg_up, r_upperleg_right);
            var r_upperleg_rotation = Quaternion.LookRotation(r_upperleg_forward, r_upperleg_up);
            r_upperleg.rotation = transform.rotation * r_upperleg_rotation;

            var r_lowerleg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            var r_lowerleg_angle = Vector3.Angle(pose.GetBone("right_hip").Position - pose.GetBone("right_knee").Position, pose.GetBone("right_ankle").Position - pose.GetBone("right_knee").Position);
            r_lowerleg.localRotation = Quaternion.Euler(180 - r_lowerleg_angle, 0f, 0f);

            var r_foot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
            var r_foot_forward = (pose.GetBone("right_toe").Position - pose.GetBone("right_ankle").Position).normalized;
            var r_foot_right = Vector3.Cross(pose.GetBone("right_toe").Position - pose.GetBone("right_ankle").Position, pose.GetBone("right_heel").Position - pose.GetBone("right_ankle").Position).normalized;
            var r_foot_up = Vector3.Cross(r_foot_forward, r_foot_right);
            var r_foot_rotation = Quaternion.LookRotation(r_foot_forward, r_foot_up);
            r_foot.rotation = transform.rotation * r_foot_rotation;
        }
    }
}

