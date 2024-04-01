using UnityEngine;
using MotionCapture.Core;
using MotionCapture.Experiment;

namespace MotionCapture.RTMPose
{
    public class FKSolver : Core.FKSolver
    {
        public HumanoidPose Humanoid = new HumanoidPose(size:5);

        public Vector3 GetBonePosition(PoseData pose, Bone bone) => pose.positions[(int)bone];

        public override void Apply(PoseData pose)
        {
            var left_hip = GetBonePosition(pose, Bone.left_hip);
            var right_hip = GetBonePosition(pose, Bone.right_hip);
            var left_shoulder = GetBonePosition(pose, Bone.left_shoulder);
            var right_shoulder = GetBonePosition(pose, Bone.right_shoulder);
            var left_elbow = GetBonePosition(pose, Bone.left_elbow);
            var right_elbow = GetBonePosition(pose, Bone.right_elbow);
            var left_wrist = GetBonePosition(pose, Bone.left_wrist);
            var right_wrist = GetBonePosition(pose, Bone.right_wrist);
            var left_knee = GetBonePosition(pose, Bone.left_knee);
            var right_knee = GetBonePosition(pose, Bone.right_knee);
            var left_ankle = GetBonePosition(pose, Bone.left_ankle);
            var right_ankle = GetBonePosition(pose, Bone.right_ankle);
            var left_toe = GetBonePosition(pose, Bone.left_toe);
            var right_toe = GetBonePosition(pose, Bone.right_toe);
            var left_heel = GetBonePosition(pose, Bone.left_heel);
            var right_heel = GetBonePosition(pose, Bone.right_heel);

            var nose = GetBonePosition(pose, Bone.nose);
            var left_eye = GetBonePosition(pose, Bone.left_eye);
            var right_eye = GetBonePosition(pose, Bone.right_eye);
            var left_ear = GetBonePosition(pose, Bone.left_ear);
            var right_ear = GetBonePosition(pose, Bone.right_ear);
            var mouth = (GetBonePosition(pose, Bone.left_mouth) + GetBonePosition(pose, Bone.right_mouth)) / 2;

            var left_inner_hand = GetBonePosition(pose, Bone.left_thumb_2);
            var left_index_distal = GetBonePosition(pose, Bone.left_index_distal);
            var left_outer_hand = GetBonePosition(pose, Bone.left_little_1);

            var right_inner_hand = GetBonePosition(pose, Bone.right_thumb_2);
            var right_index_distal = GetBonePosition(pose, Bone.right_index_distal);
            var right_outer_hand = GetBonePosition(pose, Bone.right_little_1);

            if (IsMirror)
            {
                left_hip = GetBonePosition(pose, Bone.right_hip);
                left_hip.x = -left_hip.x;
                right_hip = GetBonePosition(pose, Bone.left_hip);
                right_hip.x = -right_hip.x;
                left_shoulder = GetBonePosition(pose, Bone.right_shoulder);
                left_shoulder.x = -left_shoulder.x;
                right_shoulder = GetBonePosition(pose, Bone.left_shoulder);
                right_shoulder.x = -right_shoulder.x;
                left_elbow = GetBonePosition(pose, Bone.right_elbow);
                left_elbow.x = -left_elbow.x;
                right_elbow = GetBonePosition(pose, Bone.left_elbow);
                right_elbow.x = -right_elbow.x;
                left_wrist = GetBonePosition(pose, Bone.right_wrist);
                left_wrist.x = -left_wrist.x;
                right_wrist = GetBonePosition(pose, Bone.left_wrist);
                right_wrist.x = -right_wrist.x;
                left_knee = GetBonePosition(pose, Bone.right_knee);
                left_knee.x = -left_knee.x;
                right_knee = GetBonePosition(pose, Bone.left_knee);
                right_knee.x = -right_knee.x;
                left_ankle = GetBonePosition(pose, Bone.right_ankle);
                left_ankle.x = -left_ankle.x;
                right_ankle = GetBonePosition(pose, Bone.left_ankle);
                right_ankle.x = -right_ankle.x;
                left_toe = GetBonePosition(pose, Bone.right_toe);
                left_toe.x = -left_toe.x;
                right_toe = GetBonePosition(pose, Bone.left_toe);
                right_toe.x = -right_toe.x;
                left_heel = GetBonePosition(pose, Bone.right_heel);
                left_heel.x = -left_heel.x;
                right_heel = GetBonePosition(pose, Bone.left_heel);
                right_heel.x = -right_heel.x;

                nose = GetBonePosition(pose, Bone.nose);
                nose.x = -nose.x;
                left_eye = GetBonePosition(pose, Bone.right_eye);
                left_eye.x = -left_eye.x;
                right_eye = GetBonePosition(pose, Bone.left_eye);
                right_eye.x = -right_eye.x;
                left_ear = GetBonePosition(pose, Bone.right_ear);
                left_ear.x = -left_ear.x;
                right_ear = GetBonePosition(pose, Bone.left_ear);
                right_ear.x = -right_ear.x;
                mouth = (GetBonePosition(pose, Bone.right_mouth) + GetBonePosition(pose, Bone.left_mouth)) / 2;
                mouth.x = -mouth.x;
                
                left_inner_hand = GetBonePosition(pose, Bone.right_thumb_2);
                left_inner_hand.x = -left_inner_hand.x;
                left_index_distal = GetBonePosition(pose, Bone.right_index_distal);
                left_index_distal.x = -left_index_distal.x;
                left_outer_hand = GetBonePosition(pose, Bone.right_little_1);
                left_outer_hand.x = -left_outer_hand.x;

                right_inner_hand = GetBonePosition(pose, Bone.left_thumb_2);
                right_inner_hand.x = -right_inner_hand.x;
                right_index_distal = GetBonePosition(pose, Bone.left_index_distal);
                right_index_distal.x = -right_index_distal.x;
                right_outer_hand = GetBonePosition(pose, Bone.left_little_1);
                right_outer_hand.x = -right_outer_hand.x;
            }

            var hip = (left_hip + right_hip) / 2; 
            var head = (left_ear + right_ear + nose) / 3;
            var neck = (left_shoulder + right_shoulder) / 2;

            // Apply global position
            anim.GetBoneTransform(HumanBodyBones.Hips).localPosition = hip;
            
            // Body rotation
            var hip_forward = Vector3.Cross(left_hip - neck, right_hip - neck).normalized;
            var hip_right = (right_hip - left_hip).normalized;
            var hip_up = Vector3.Cross(hip_forward, hip_right).normalized;
            hip_forward = Vector3.Cross(hip_right, hip_up).normalized;
            //ApplyBoneRotation(HumanBodyBones.Hips, Quaternion.LookRotation(hip_forward, hip_up));
            Humanoid.AddBoneRotation(HumanBodyBones.Hips, Quaternion.LookRotation(hip_forward, hip_up));

            var upperchest_forward = Vector3.Cross(right_shoulder - hip, left_shoulder - hip).normalized;
            var upperchest_right = (right_shoulder - left_shoulder).normalized;
            var upperchest_up = Vector3.Cross(upperchest_forward, upperchest_right);
            //ApplyBoneRotation(HumanBodyBones.UpperChest, Quaternion.LookRotation(upperchest_forward, upperchest_up));
            Humanoid.AddBoneRotation(HumanBodyBones.UpperChest, Quaternion.LookRotation(upperchest_forward, upperchest_up));

            // Head rotation
            var neck_up = (head - neck).normalized;
            //ApplyBoneRotation(HumanBodyBones.Neck, Quaternion.LookRotation(upperchest_forward, neck_up));
            Humanoid.AddBoneRotation(HumanBodyBones.Neck, Quaternion.LookRotation(upperchest_forward, neck_up));

            var head_up = ((left_eye + right_eye) / 2 - mouth).normalized;
            var head_forward = Vector3.Cross(right_eye - mouth, left_eye - mouth).normalized;
            //ApplyBoneRotation(HumanBodyBones.Head, Quaternion.LookRotation(head_forward, head_up));
            Humanoid.AddBoneRotation(HumanBodyBones.Head, Quaternion.LookRotation(head_forward, head_up));

            // Left arm rotation
            var l_upperarm_right = -(left_elbow - left_shoulder).normalized;
            var l_upperarm_up = -Vector3.Cross(left_shoulder - left_elbow, left_wrist - left_elbow);
            var l_upperarm_forward = -Vector3.Cross(l_upperarm_up, l_upperarm_right);
            l_upperarm_up = Vector3.Cross(l_upperarm_forward, l_upperarm_right);
            var l_upperarm_rotation = Quaternion.LookRotation(l_upperarm_forward, l_upperarm_up);
            //ApplyBoneRotation(HumanBodyBones.LeftUpperArm, l_upperarm_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.LeftUpperArm, l_upperarm_rotation);

            var l_lowerarm_right = (left_elbow - left_wrist).normalized;
            //var l_lowerarm_up = Vector3.Cross(left_elbow - left_wrist, left_index_distal - left_wrist);
            var l_lowerarm_forward = (left_inner_hand - left_outer_hand).normalized; 
            //var l_lowerarm_forward = Vector3.Cross(l_lowerarm_up, l_lowerarm_right);
            var l_lowerarm_up = Vector3.Cross(l_lowerarm_forward, l_lowerarm_right);
            var l_lowerarm_rotation = Quaternion.LookRotation(l_lowerarm_forward, l_lowerarm_up);
            //ApplyBoneRotation(HumanBodyBones.LeftLowerArm, l_lowerarm_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.LeftLowerArm, l_lowerarm_rotation);

            var l_hand_right = -(left_index_distal - left_wrist).normalized;
            var l_hand_forward = (left_inner_hand - left_outer_hand).normalized;
            var l_hand_up = Vector3.Cross(l_hand_forward, l_hand_right);
            var l_hand_rotation = Quaternion.LookRotation(l_hand_forward, l_hand_up);
            //ApplyBoneRotation(HumanBodyBones.LeftHand, l_hand_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.LeftHand, l_hand_rotation);

            // right arm rotation
            var r_upperarm_right = (right_elbow - right_shoulder).normalized;
            var r_upperarm_up = Vector3.Cross(right_shoulder - right_elbow, right_wrist - right_elbow);
            var r_upperarm_forward = -Vector3.Cross(r_upperarm_up, r_upperarm_right);
            r_upperarm_up = Vector3.Cross(r_upperarm_forward, r_upperarm_right);
            var r_upperarm_rotation = Quaternion.LookRotation(r_upperarm_forward, r_upperarm_up);
            //ApplyBoneRotation(HumanBodyBones.RightUpperArm, r_upperarm_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.RightUpperArm, r_upperarm_rotation);

            var r_lowerarm_right = -(right_elbow - right_wrist).normalized;
            //var r_lowerarm_up = Vector3.Cross(right_elbow - right_wrist, right_index_distal - right_wrist);
            var r_lowerarm_forward = (right_inner_hand - right_outer_hand).normalized;
            //var r_lowerarm_forward = Vector3.Cross(r_lowerarm_up, r_lowerarm_right);
            var r_lowerarm_up = Vector3.Cross(r_lowerarm_forward, r_lowerarm_right);
            var r_lowerarm_rotation = Quaternion.LookRotation(r_lowerarm_forward, r_lowerarm_up);
            //ApplyBoneRotation(HumanBodyBones.RightLowerArm, r_lowerarm_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.RightLowerArm, r_lowerarm_rotation);

            var r_hand_right = (right_index_distal - right_wrist).normalized;
            var r_hand_forward = (right_inner_hand - right_outer_hand).normalized;
            var r_hand_up = Vector3.Cross(r_hand_forward, r_hand_right);
            var r_hand_rotation = Quaternion.LookRotation(r_hand_forward, r_hand_up);
            //ApplyBoneRotation(HumanBodyBones.RightHand, r_hand_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.RightHand, r_hand_rotation);

            // Left leg rotation
            var l_upperleg_up = -(left_knee - left_hip).normalized;
            var l_upperleg_forward = (left_toe - left_heel).normalized;
            var l_upperleg_right = Vector3.Cross(l_upperleg_up, l_upperleg_forward);
            var l_knee_angle = Vector3.Angle(left_hip - left_knee, left_ankle - left_knee);
            if (l_knee_angle < 160)
            {
                l_upperleg_right = -Vector3.Cross(left_hip - left_knee, left_ankle - left_knee);
            }
            l_upperleg_forward = Vector3.Cross(l_upperleg_right, l_upperleg_up);
            var l_upperleg_rotation = Quaternion.LookRotation(l_upperleg_forward, l_upperleg_up);
            //ApplyBoneRotation(HumanBodyBones.LeftUpperLeg, l_upperleg_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.LeftUpperLeg, l_upperleg_rotation);

            var l_lowerleg_angle = Vector3.Angle(left_hip - left_knee, left_ankle - left_knee);
            anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).localRotation = Quaternion.Euler(180 - l_lowerleg_angle, 0f, 0f);

            var l_foot_forward = (left_toe - left_heel).normalized;
            //var l_foot_right = Vector3.Cross(left_toe - left_ankle, left_heel - left_ankle).normalized;
            var l_foot_right = Vector3.Cross(left_toe - left_knee, left_ankle - left_knee).normalized;
            var l_foot_up = Vector3.Cross(l_foot_forward, l_foot_right);
            var l_foot_rotation = Quaternion.LookRotation(l_foot_forward, l_foot_up);
            //ApplyBoneRotation(HumanBodyBones.LeftFoot, l_foot_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.LeftFoot, l_foot_rotation);

            // Right leg rotation
            var r_upperleg_up = -(right_knee - right_hip).normalized;
            var r_upperleg_forward = (right_toe - right_ankle).normalized; // L‚Ñ‚Ä‚¢‚éŽž
            var r_upperleg_right = Vector3.Cross(r_upperleg_up, r_upperleg_forward);
            var r_knee_angle = Vector3.Angle(right_hip - right_knee, right_ankle - right_knee);
            if (r_knee_angle < 160) // ‹È‚ª‚Á‚Ä‚¢‚éŽž
            {
                r_upperleg_right = -Vector3.Cross(right_hip - right_knee, right_ankle - right_knee);
            }
            r_upperleg_forward = -Vector3.Cross(r_upperleg_up, r_upperleg_right);
            var r_upperleg_rotation = Quaternion.LookRotation(r_upperleg_forward, r_upperleg_up);
            //ApplyBoneRotation(HumanBodyBones.RightUpperLeg, r_upperleg_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.RightUpperLeg, r_upperleg_rotation);

            var r_lowerleg_angle = Vector3.Angle(right_hip - right_knee, right_ankle - right_knee);
            anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).localRotation = Quaternion.Euler(180 - r_lowerleg_angle, 0f, 0f);

            var r_foot_forward = (right_toe - right_heel).normalized;
            //var r_foot_right = Vector3.Cross(right_toe - right_ankle, right_heel - right_ankle).normalized;
            var r_foot_right = Vector3.Cross(right_toe - right_knee, right_ankle - right_knee).normalized;
            var r_foot_up = Vector3.Cross(r_foot_forward, r_foot_right);
            var r_foot_rotation = Quaternion.LookRotation(r_foot_forward, r_foot_up);
            //ApplyBoneRotation(HumanBodyBones.RightFoot, r_foot_rotation);
            Humanoid.AddBoneRotation(HumanBodyBones.RightFoot, r_foot_rotation);

            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                var rotation = Humanoid.GetBoneRotation(bone);
                if (rotation != Quaternion.identity)
                    ApplyBoneRotation(bone, rotation);
            }
        }
    }
}
