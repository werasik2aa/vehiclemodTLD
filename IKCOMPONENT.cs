using System;
using UnityEngine;
using static vehiclemod.data;
using MelonLoader;
using SkyCoop;

namespace vehiclemod
{
    public class IKVH : MonoBehaviour
    {
        public IKVH(IntPtr ptr) : base(ptr) { }
        private Transform RL = null, LL = null, LA = null, RA = null;
        private Animator anim = null;
        public void Init(int CarId, int sit)
        {
            InfoMain c = GetObj(CarId).GetComponent<VehComponent>().vehicleData;
            RL = c.m_Sits[sit].transform.Find("RL");
            LL = c.m_Sits[sit].transform.Find("LL");
            LA = c.m_Sits[sit].transform.Find("LA");
            RA = c.m_Sits[sit].transform.Find("RA");
            anim = GetComponent<SkyCoop.MyMod.MultiplayerPlayerAnimator>().m_Animer;
        }
        public void OnAnimatorIK(int _layerIndex)
        {
            if (anim)
            {
                MelonLogger.Msg("fgsdg");
                if (RL)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, RL.position);
                }
                else anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                if (LL)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, LL.position);
                }
                else anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                if (LA)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, RA.position);
                } else anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                if (LA)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, RA.position);
                }
                else anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
