using RootMotion.FinalIK;
using UnityEngine;

namespace MultiplayerARPG
{
    public class EquipmentEntityWeaponIKManager : MonoBehaviour
    {
        public BaseCharacterEntity characterEntity;
        public AimIK leftHandWeaponAimIK;
        public AimIK rightHandWeaponAimIK;
        public LimbIK leftHandWeaponLimbIK;
        public LimbIK rightHandWeaponLimbIK;

        private GameObject aimTarget;
        private EquipmentEntityWeaponIK leftHandWeapon;
        private EquipmentEntityWeaponIK rightHandWeapon;

        public void SetLeftHandEquipmentEntityWeaponIK(EquipmentEntityWeaponIK weapon)
        {
            leftHandWeapon = weapon;
        }

        public void SetRightHandEquipmentEntityWeaponIK(EquipmentEntityWeaponIK weapon)
        {
            rightHandWeapon = weapon;
        }

        private void Awake()
        {
            // Disable the IK components to take control over the updating of their solvers
            if (leftHandWeaponAimIK)
                leftHandWeaponAimIK.enabled = false;
            if (rightHandWeaponAimIK)
                rightHandWeaponAimIK.enabled = false;
            if (leftHandWeaponLimbIK)
                leftHandWeaponLimbIK.enabled = false;
            if (rightHandWeaponLimbIK)
                rightHandWeaponLimbIK.enabled = false;
            aimTarget = new GameObject("_aimTarget");
            aimTarget.transform.parent = transform;
            aimTarget.transform.localPosition = Vector3.zero;
            aimTarget.transform.localEulerAngles = Vector3.zero;
            aimTarget.transform.localScale = Vector3.one;
            if (characterEntity == null)
                characterEntity = GetComponentInParent<BaseCharacterEntity>();
        }

        private void OnDestroy()
        {
            if (aimTarget)
                Destroy(aimTarget);
        }

        private void LateUpdate()
        {
            if (!leftHandWeapon && !rightHandWeapon)
                return;

            bool hasAimPosition = characterEntity.AimPosition.type != AimPositionType.None;
            if (hasAimPosition)
            {
                switch (characterEntity.AimPosition.type)
                {
                    case AimPositionType.Position:
                        aimTarget.transform.position = characterEntity.AimPosition.position;
                        break;
                    case AimPositionType.Direction:
                        aimTarget.transform.position = characterEntity.AimPosition.position + ((Vector3)characterEntity.AimPosition.direction * 10f);
                        break;
                }
            }

            if (leftHandWeapon)
            {
                if (leftHandWeaponAimIK && hasAimPosition)
                {
                    leftHandWeaponAimIK.solver.transform = leftHandWeapon.aimTransform;
                    leftHandWeaponAimIK.solver.target = aimTarget.transform;
                    leftHandWeaponAimIK.solver.Update();
                }
                if (leftHandWeaponLimbIK && !rightHandWeapon)
                {
                    leftHandWeaponLimbIK.solver.IKPosition = leftHandWeapon.anotherHandTransform.position;
                    leftHandWeaponLimbIK.solver.IKRotation = leftHandWeapon.anotherHandTransform.rotation;
                    leftHandWeaponLimbIK.solver.Update();
                }
                return;
            }

            if (rightHandWeapon)
            {
                if (rightHandWeaponAimIK && hasAimPosition)
                {
                    rightHandWeaponAimIK.solver.transform = rightHandWeapon.aimTransform;
                    rightHandWeaponAimIK.solver.target = aimTarget.transform;
                    rightHandWeaponAimIK.solver.Update();
                }
                if (rightHandWeaponLimbIK && !leftHandWeapon)
                {
                    rightHandWeaponLimbIK.solver.IKPosition = rightHandWeapon.anotherHandTransform.position;
                    rightHandWeaponLimbIK.solver.IKRotation = rightHandWeapon.anotherHandTransform.rotation;
                    rightHandWeaponLimbIK.solver.Update();
                }
                return;
            }
        }

        [ContextMenu("Generate IK Components")]
        public void GenerateIKComponents()
        {
            Animator animator = GetComponentInChildren<Animator>();
            if (!leftHandWeaponAimIK)
            {
                GameObject ikObj = new GameObject("_leftHandWeaponAimIK", typeof(AimIK));
                ikObj.transform.parent = transform;
                ikObj.transform.localPosition = Vector3.zero;
                ikObj.transform.localEulerAngles = Vector3.zero;
                ikObj.transform.localScale = Vector3.one;
                leftHandWeaponAimIK = ikObj.GetComponent<AimIK>();
                GenerateAimIK(animator, leftHandWeaponAimIK);
            }
            if (!rightHandWeaponAimIK)
            {
                GameObject ikObj = new GameObject("_rightHandWeaponAimIK", typeof(AimIK));
                ikObj.transform.parent = transform;
                ikObj.transform.localPosition = Vector3.zero;
                ikObj.transform.localEulerAngles = Vector3.zero;
                ikObj.transform.localScale = Vector3.one;
                rightHandWeaponAimIK = ikObj.GetComponent<AimIK>();
                rightHandWeaponAimIK.fixTransforms = true;
                GenerateAimIK(animator, rightHandWeaponAimIK);
            }
            if (!leftHandWeaponLimbIK)
            {
                GameObject ikObj = new GameObject("_leftHandWeaponArmIK", typeof(LimbIK));
                ikObj.transform.parent = transform;
                ikObj.transform.localPosition = Vector3.zero;
                ikObj.transform.localEulerAngles = Vector3.zero;
                ikObj.transform.localScale = Vector3.one;
                leftHandWeaponLimbIK = ikObj.GetComponent<LimbIK>();
                leftHandWeaponLimbIK.solver.goal = AvatarIKGoal.RightHand;
                GenerateLimbIK(animator, leftHandWeaponLimbIK, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);
            }
            if (!rightHandWeaponLimbIK)
            {
                GameObject ikObj = new GameObject("_rightHandWeaponArmIK", typeof(LimbIK));
                ikObj.transform.parent = transform;
                ikObj.transform.localPosition = Vector3.zero;
                ikObj.transform.localEulerAngles = Vector3.zero;
                ikObj.transform.localScale = Vector3.one;
                rightHandWeaponLimbIK = ikObj.GetComponent<LimbIK>();
                rightHandWeaponLimbIK.solver.goal = AvatarIKGoal.LeftHand;
                GenerateLimbIK(animator, rightHandWeaponLimbIK, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
            }
        }

        public void GenerateAimIK(Animator animator, AimIK aimIK)
        {
            aimIK.fixTransforms = true;
            aimIK.solver.axis = Vector3.forward;
            aimIK.solver.poleAxis = Vector3.zero;
            aimIK.solver.tolerance = 0;
            aimIK.solver.maxIterations = 4;
            aimIK.solver.clampWeight = 0.5f;
            aimIK.solver.clampSmoothing = 2;
            aimIK.solver.useRotationLimits = true;
            aimIK.solver.XY = false;
            aimIK.solver.AddBone(animator.GetBoneTransform(HumanBodyBones.Spine));
            aimIK.solver.AddBone(animator.GetBoneTransform(HumanBodyBones.Chest));
            aimIK.solver.AddBone(animator.GetBoneTransform(HumanBodyBones.UpperChest));
            aimIK.solver.AddBone(animator.GetBoneTransform(HumanBodyBones.Neck));
            foreach (var bone in aimIK.solver.bones)
            {
                bone.weight = 0.5f;
            }
        }

        public void GenerateLimbIK(Animator animator, LimbIK limbIK, HumanBodyBones bone1, HumanBodyBones bone2, HumanBodyBones bone3)
        {
            limbIK.fixTransforms = true;
            limbIK.solver.IKPositionWeight = 1f;
            limbIK.solver.IKRotationWeight = 1f;
            limbIK.solver.maintainRotationWeight = 0f;
            limbIK.solver.bendModifier = IKSolverLimb.BendModifier.Animation;
            limbIK.solver.bendModifierWeight = 1f;
            limbIK.solver.bone1 = new IKSolverTrigonometric.TrigonometricBone()
            {
                transform = animator.GetBoneTransform(bone1)
            };
            limbIK.solver.bone2 = new IKSolverTrigonometric.TrigonometricBone()
            {
                transform = animator.GetBoneTransform(bone2)
            };
            limbIK.solver.bone3 = new IKSolverTrigonometric.TrigonometricBone()
            {
                transform = animator.GetBoneTransform(bone3)
            };
        }
    }
}
