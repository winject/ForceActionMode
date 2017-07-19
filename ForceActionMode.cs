using System;
using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using System.Windows.Forms;


namespace ActionMode
{
    public class Entry : Script
    {
        bool bArmedOnly = false; //Only put in action mode when armed, no fists
        bool bApplyToGroup = false; //Force your bodyguards to go into action mode

        Ped playerPed = null;

        public Entry()
        {
            playerPed = Game.Player.Character;
            base.Tick += new EventHandler(this.OnTick);
            base.Interval = 1000; 
        }

        protected override void Dispose(bool A_0)
        {
            
            base.Dispose(A_0);
        }

        bool IsValid(ref Ped ped)
        {
            return ped != null && ped.Exists() && ped.IsAlive;
        }

        bool IsPedArmed(ref Ped ped)
        {
            return ped.Weapons.Current.Hash != WeaponHash.Unarmed;
        }

        bool IsPedInActionMode(ref Ped ped)
        {
            return Function.Call<bool>(Hash.IS_PED_USING_ACTION_MODE, ped);
        }

        void PutPedIntoActionMode(ref Ped ped)
        {
            Function.Call(Hash.SET_PED_USING_ACTION_MODE, ped, true, -1, "DEFAULT_ACTION");
        }

        void UpdateActionMode(ref Ped ped)
        {
            if (IsValid(ref ped))
            {
                if (!IsPedInActionMode(ref ped))
                {
                    if (bArmedOnly)
                    {
                        if (IsPedArmed(ref ped)) PutPedIntoActionMode(ref ped);
                    }
                    else if (!bArmedOnly)
                    {
                        PutPedIntoActionMode(ref ped);
                    }
                }
            }
        }

        void UpdatePlayerGroup()
        {
            int groupCount = Game.Player.Character.CurrentPedGroup.MemberCount;
            for(int i = 0; i < groupCount; i++)
            {
                Ped curPed = Game.Player.Character.CurrentPedGroup.GetMember(i);
                UpdateActionMode(ref curPed);
            }
        }

        void UpdatePlayer()
        {
            UpdateActionMode(ref playerPed);
        } 
        
        public void OnTick(object sender, EventArgs e)
        {
           if(Function.Call<bool>(Hash.IS_ENTITY_ON_SCREEN, Game.Player.Character))
            {
                if (bApplyToGroup)
                {
                    UpdatePlayerGroup();
                    UpdatePlayer();
                }
                else
                {
                    UpdatePlayer();
                }     
            }        
        }
    }
}
