using System;
using GTA;
using GTA.Native;
using System.Windows.Forms;


namespace ActionMode
{
    public class Entry : Script
    {
		bool TOGGLE = true;		 		//	Should mod be toggled on first start?
		bool bShowAlert=true;			//	Show on screen short message for script status
        bool bArmedOnly = true; 		//  Only put in action mode when armed, no fists
        bool bApplyToGroup = false; 	//	Force your bodyguards to go into action mode
		Keys myKey = Keys.T;			//	Change 'T' to any key you want, see https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx

        Ped playerPed = null;

        public Entry()
        {
            playerPed = Game.Player.Character;
            base.Tick += new EventHandler(this.OnTick);
			base.Interval = 750;
			KeyDown += OnKeyDown;   
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

        void PutPedIntoActionMode(ref Ped ped, bool toggle)
        {
            Function.Call(Hash.SET_PED_USING_ACTION_MODE, ped, toggle, -1, "DEFAULT_ACTION");
        }

        void UpdateActionMode(ref Ped ped)
        {
            if (IsValid(ref ped))
            {
                if (!IsPedInActionMode(ref ped))
                {
                    if(IsPedArmed(ref ped))
					{
						if(bArmedOnly) PutPedIntoActionMode(ref ped, true);
					}
                }
				else
				{
					if(bArmedOnly) PutPedIntoActionMode(ref ped, false);
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
			if(TOGGLE)
			{
				if(Function.Call<bool>(Hash.IS_ENTITY_ON_SCREEN, Game.Player.Character))
				{
					playerPed = Game.Player.Character;
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
		
		private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == myKey)
			{
				if(TOGGLE){
					PutPedIntoActionMode(ref playerPed, false);
					TOGGLE=false;
				}
				else if(!TOGGLE)
				{
					TOGGLE=true;
				}
				if(bShowAlert) UI.ShowSubtitle("ForceActionMode : " + (TOGGLE == true ? "~g~true" : "~r~false"), 1000);
			}
		}
    }
}
