using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class EquipmentEntityWeaponIK : MonoBehaviour
    {
        public BaseEquipmentEntity equipmentEntity;
        public Transform aimTransform;
        public Transform anotherHandTransform;

        private void Awake()
        {
            equipmentEntity.onSetup.AddListener(OnSetup);
        }

        private void OnDestroy()
        {
            equipmentEntity.onSetup.RemoveListener(OnSetup);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (anotherHandTransform != null)
            {
                Gizmos.color = new Color(1, 1, 0, 0.5f);
                Gizmos.DrawSphere(anotherHandTransform.position, 0.03f);
                Handles.Label(anotherHandTransform.position, name + "(Another Hand)");
            }
        }
#endif

        public void OnSetup()
        {
            if (equipmentEntity.EquipPosition.Equals(GameDataConst.EQUIP_POSITION_RIGHT_HAND))
            {
                EquipmentEntityWeaponIKManager weaponIK = GetComponentInParent<EquipmentEntityWeaponIKManager>();
                weaponIK.SetRightHandEquipmentEntityWeaponIK(this);
            }
            else if (equipmentEntity.EquipPosition.Equals(GameDataConst.EQUIP_POSITION_LEFT_HAND))
            {
                EquipmentEntityWeaponIKManager weaponIK = GetComponentInParent<EquipmentEntityWeaponIKManager>();
                weaponIK.SetLeftHandEquipmentEntityWeaponIK(this);
            }
        }
    }
}
