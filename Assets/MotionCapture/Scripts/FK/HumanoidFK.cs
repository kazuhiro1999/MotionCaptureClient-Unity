using UnityEngine;
using MotionCapture.Data;

namespace MotionCapture
{
    public class HumanoidFK : BaseFK
    {

        private Vector3 forward;
        private Vector3 up;
        private Vector3 right;

        public override void Forward(PoseData pose) {

            RotateHips(pose);

            RotateBody(pose);
            RotateHead(pose);
            RotateLeftArm(pose);
            RotateRightArm(pose);
            RotateLeftLeg(pose);
            RotateRightLeg(pose);

        }

        void RotateHips(PoseData pose) {
            var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            hips.localPosition = (pose.GetBone("Spine").Position + pose.GetBone("L_UpperLeg").Position + pose.GetBone("R_UpperLeg").Position) / (3 * 100);

            forward = transform.rotation * Vector3.Cross(pose.GetBone("L_UpperLeg").Position - pose.GetBone("Spine").Position, pose.GetBone("R_UpperLeg").Position - pose.GetBone("Spine").Position).normalized;
            right = transform.rotation * (pose.GetBone("R_UpperLeg").Position - pose.GetBone("L_UpperLeg").Position).normalized;
            up = Vector3.Cross(forward, right).normalized;

            hips.forward = forward;
            //hips.right = transform.rotation * right;
            var angle = Vector3.SignedAngle(hips.right, right, up);
            hips.rotation = Quaternion.AngleAxis(angle, up) * hips.rotation;

        }

        void RotateBody(PoseData pose) {
            Vector3 relativeVector;

            var spine = anim.GetBoneTransform(HumanBodyBones.Spine);
            relativeVector = transform.rotation * (pose.GetBone("Chest").Position - pose.GetBone("Spine").Position).normalized;
            var spine_x = Vector3.SignedAngle(up, relativeVector, right);
            var spine_z = Vector3.SignedAngle(up, relativeVector, forward);
            spine.localRotation = Quaternion.Euler(spine_x, 0f, spine_z);
            //spine.up = transform.rotation * (pose.Chest - pose.Spine).normalized;

            var chest = anim.GetBoneTransform(HumanBodyBones.Chest);
            relativeVector = transform.rotation * (pose.GetBone("UpperChest").Position - pose.GetBone("Chest").Position).normalized;
            var chest_x = Vector3.SignedAngle(up, relativeVector, right) - spine_x;
            var chest_z = Vector3.SignedAngle(up, relativeVector, forward) - spine_z;
            chest.localRotation = Quaternion.Euler(chest_x, 0f, chest_z);
            //chest.up = transform.rotation * (pose.UpperChest - pose.Chest).normalized;

            var upperchest = anim.GetBoneTransform(HumanBodyBones.UpperChest);
            var body_forward = transform.rotation * Vector3.Cross(pose.GetBone("R_Shoulder").Position - pose.GetBone("UpperChest").Position, pose.GetBone("L_Shoulder").Position - pose.GetBone("UpperChest").Position).normalized;
            upperchest.forward = body_forward;
            var body_right = transform.rotation * (pose.GetBone("R_Shoulder").Position - pose.GetBone("L_Shoulder").Position).normalized;
            var body_up = Vector3.Cross(body_forward, body_right);
            var angle = Vector3.SignedAngle(upperchest.right, body_right, body_up);
            upperchest.rotation = Quaternion.AngleAxis(angle, body_up) * upperchest.rotation;
        }

        void RotateHead(PoseData pose) {
            var neck = anim.GetBoneTransform(HumanBodyBones.Neck);
            var head_pos = (pose.GetBone("L_Ear").Position + pose.GetBone("R_Ear").Position + pose.GetBone("Jaw").Position) / 3;
            neck.up = transform.rotation * (head_pos - pose.GetBone("Neck").Position).normalized;

            var head = anim.GetBoneTransform(HumanBodyBones.Head);
            var up = transform.rotation * (pose.GetBone("HeadTop").Position - head_pos).normalized;
            var forward = transform.rotation * Vector3.Cross(pose.GetBone("L_Ear").Position - pose.GetBone("HeadTop").Position, pose.GetBone("R_Ear").Position - pose.GetBone("HeadTop").Position).normalized;
            head.forward = forward;
            var angle = Vector3.SignedAngle(head.up, up, forward);
            head.rotation = head.rotation * Quaternion.AngleAxis(angle, forward);
        }

        void RotateLeftArm(PoseData pose) {

            var l_shoulder = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
            l_shoulder.right = transform.rotation * -(pose.GetBone("L_UpperArm").Position - pose.GetBone("L_Shoulder").Position).normalized;

            var l_upperarm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            l_upperarm.right = transform.rotation * -(pose.GetBone("L_LowerArm").Position - pose.GetBone("L_UpperArm").Position).normalized; ;

            // è„òrÇÃâÒì]
            var l_arm_up = transform.rotation * -Vector3.Cross(pose.GetBone("L_UpperArm").Position - pose.GetBone("L_LowerArm").Position, pose.GetBone("L_Hand").Position - pose.GetBone("L_LowerArm").Position);
            var angle = Vector3.SignedAngle(l_upperarm.up, l_arm_up, l_upperarm.right);
            l_upperarm.rotation = Quaternion.AngleAxis(angle, l_upperarm.right) * l_upperarm.rotation;

            var l_lowerarm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            l_lowerarm.right = transform.rotation * (pose.GetBone("L_LowerArm").Position - pose.GetBone("L_Hand").Position).normalized;

            // éËéÒÇÃâÒì]
            var hand_up = transform.rotation * -Vector3.Cross(pose.GetBone("L_Thumb").Position - pose.GetBone("L_Hand").Position, pose.GetBone("L_Little").Position - pose.GetBone("L_Hand").Position).normalized;
            var hand_right = transform.rotation * -(pose.GetBone("L_FingerTip").Position - pose.GetBone("L_Hand").Position).normalized;
            var hand_forward = -Vector3.Cross(hand_up, hand_right).normalized;
            angle = Vector3.SignedAngle(l_lowerarm.forward, hand_forward, l_lowerarm.right);
            l_lowerarm.rotation = Quaternion.AngleAxis(angle, l_lowerarm.right) * l_lowerarm.rotation;

            var l_hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            l_hand.right = transform.rotation * -(pose.GetBone("L_FingerTip").Position - pose.GetBone("L_Hand").Position).normalized;
        }
        void RotateRightArm(PoseData pose) {

            var r_shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
            r_shoulder.right = transform.rotation * (pose.GetBone("R_UpperArm").Position - pose.GetBone("R_Shoulder").Position).normalized;

            var r_upperarm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            r_upperarm.right = transform.rotation * (pose.GetBone("R_LowerArm").Position - pose.GetBone("R_UpperArm").Position).normalized; ;

            // è„òrÇÃâÒì]
            var r_arm_up = transform.rotation * Vector3.Cross(pose.GetBone("R_UpperArm").Position - pose.GetBone("R_LowerArm").Position, pose.GetBone("R_Hand").Position - pose.GetBone("R_LowerArm").Position);
            var angle = Vector3.SignedAngle(r_upperarm.up, r_arm_up, r_upperarm.right);
            r_upperarm.rotation = Quaternion.AngleAxis(angle, r_upperarm.right) * r_upperarm.rotation;

            var r_lowerarm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            r_lowerarm.right = transform.rotation * -(pose.GetBone("R_LowerArm").Position - pose.GetBone("R_Hand").Position).normalized;

            // éËéÒÇÃâÒì]
            var hand_up = transform.rotation * Vector3.Cross(pose.GetBone("R_Thumb").Position - pose.GetBone("R_Hand").Position, pose.GetBone("R_Little").Position - pose.GetBone("R_Hand").Position).normalized;
            var hand_right = transform.rotation * (pose.GetBone("R_FingerTip").Position - pose.GetBone("R_Hand").Position).normalized;
            var hand_forward = -Vector3.Cross(hand_up, hand_right).normalized;
            angle = Vector3.SignedAngle(r_lowerarm.forward, hand_forward, r_lowerarm.right);
            r_lowerarm.rotation = Quaternion.AngleAxis(angle, r_lowerarm.right) * r_lowerarm.rotation;

            var r_hand = anim.GetBoneTransform(HumanBodyBones.RightHand);
            r_hand.right = transform.rotation * (pose.GetBone("R_FingerTip").Position - pose.GetBone("R_Hand").Position).normalized;
        }

        void RotateLeftLeg(PoseData pose) {
            var l_upperleg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            l_upperleg.up = transform.rotation * -(pose.GetBone("L_LowerLeg").Position - pose.GetBone("L_UpperLeg").Position).normalized;

            // è„ë⁄ÇÃâÒì]
            //var leg_right = transform.rotation * -Vector3.Cross(pose.L_UpperLeg - pose.L_LowerLeg, pose.L_Foot - pose.L_LowerLeg).normalized;
            var leg_right = transform.rotation * Vector3.Cross(pose.GetBone("L_Foot").Position - pose.GetBone("L_LowerLeg").Position, pose.GetBone("L_Foot").Position - pose.GetBone("L_Toe").Position).normalized;
            var angle = Vector3.SignedAngle(l_upperleg.right, leg_right, l_upperleg.up);
            l_upperleg.rotation = Quaternion.AngleAxis(angle, l_upperleg.up) * l_upperleg.rotation;

            var l_lowerleg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            angle = Vector3.Angle(pose.GetBone("L_UpperLeg").Position - pose.GetBone("L_LowerLeg").Position, pose.GetBone("L_Foot").Position - pose.GetBone("L_LowerLeg").Position);
            l_lowerleg.localRotation = Quaternion.Euler(180 - angle, 0f, 0f);

            var l_foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            var foot_forward = transform.rotation * (pose.GetBone("L_Toe").Position - pose.GetBone("L_Foot").Position).normalized;
            l_foot.forward = foot_forward;
            var foot_right = transform.rotation * Vector3.Cross(pose.GetBone("L_Toe").Position - pose.GetBone("L_Foot").Position, pose.GetBone("L_Heel").Position - pose.GetBone("L_Foot").Position).normalized;
            angle = Vector3.SignedAngle(l_foot.right, foot_right, foot_forward);
            l_foot.rotation = Quaternion.AngleAxis(angle, foot_forward) * l_foot.rotation;
        }
        void RotateRightLeg(PoseData pose) {
            var r_upperleg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            r_upperleg.up = transform.rotation * -(pose.GetBone("R_LowerLeg").Position - pose.GetBone("R_UpperLeg").Position).normalized;

            // è„ë⁄ÇÃâÒì]
            var leg_right = transform.rotation * -Vector3.Cross(pose.GetBone("R_UpperLeg").Position - pose.GetBone("R_LowerLeg").Position, pose.GetBone("R_Foot").Position - pose.GetBone("R_LowerLeg").Position).normalized;
            //var leg_right = transform.rotation * Vector3.Cross(pose.R_Foot - pose.R_LowerLeg, pose.R_Foot - pose.R_Toe).normalized;
            var angle = Vector3.SignedAngle(r_upperleg.right, leg_right, r_upperleg.up);
            r_upperleg.rotation = Quaternion.AngleAxis(angle, r_upperleg.up) * r_upperleg.rotation;

            var r_lowerleg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            angle = Vector3.Angle(pose.GetBone("R_UpperLeg").Position - pose.GetBone("R_LowerLeg").Position, pose.GetBone("R_Foot").Position - pose.GetBone("R_LowerLeg").Position);
            r_lowerleg.localRotation = Quaternion.Euler(180 - angle, 0f, 0f);

            var r_foot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
            var foot_forward = transform.rotation * (pose.GetBone("R_Toe").Position - pose.GetBone("R_Foot").Position).normalized;
            r_foot.forward = foot_forward;
            var foot_right = transform.rotation * Vector3.Cross(pose.GetBone("R_Toe").Position - pose.GetBone("R_Foot").Position, pose.GetBone("R_Heel").Position - pose.GetBone("R_Foot").Position).normalized;
            angle = Vector3.SignedAngle(r_foot.right, foot_right, foot_forward);
            r_foot.rotation = Quaternion.AngleAxis(angle, foot_forward) * r_foot.rotation;
        }
    }

}