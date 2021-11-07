using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class EquipmentEntityWeaponIK : MonoBehaviour
    {
        public BaseEquipmentEntity equipmentEntity;
        public Transform otherHandTransform;

        private void Awake()
        {
            equipmentEntity.onSetup.AddListener(OnSetup);
        }

        private void OnDestroy()
        {
            equipmentEntity.onSetup.RemoveListener(OnSetup);
        }

        public void OnSetup()
        {
            if (equipmentEntity.EquipPosition.Equals(GameDataConst.EQUIP_POSITION_RIGHT_HAND))
            {

            }
            else if (equipmentEntity.EquipPosition.Equals(GameDataConst.EQUIP_POSITION_LEFT_HAND))
            {

            }
        }
    }
}
