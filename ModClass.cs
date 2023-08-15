namespace VesselMayCry
{
    public class VesselMayCry : Mod
    {

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        new public string GetName() => "Vessel May Cry";
        public override string GetVersion() => "v0.0.0.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            On.HeroController.Start += BecomeTheStormThatIsApproaching;
            SpritePositions.Initialise();
        }

        public static string weapon;
        private void BecomeTheStormThatIsApproaching(On.HeroController.orig_Start orig, HeroController self)
        {
            orig.Invoke(self);

            //anims
            Anims.AnimationInit();

            //weapon hooks and base value changes
            CreateSwitch();
            HeroController.instance.gameObject.AddComponent<Yamato.Base>();
            HeroController.instance.BIG_FALL_TIME = 999;
            HeroController.instance.NAIL_CHARGE_TIME_CHARM = 0.2f;
            HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = 0.2f;
            On.HutongGames.PlayMaker.Actions.SetFloatValue.OnEnter += FocusSpeed;

            //concentration
            HeroController.instance.gameObject.AddComponent<Concentration>();

            //style
            HeroController.instance.gameObject.AddComponent<Style>();

            //vessel trigger
            HeroController.instance.gameObject.AddComponent<VesselTrigger>();

            //switch
            WeaponSwitch();

            //damage mods
            On.HealthManager.Hit += ModifyNailDamage;
        }

        private void FocusSpeed(On.HutongGames.PlayMaker.Actions.SetFloatValue.orig_OnEnter orig, SetFloatValue self)
        {
            //shoutout to exempt medic
            if (self.Fsm.GameObject.name == "Knight" && self.Fsm.Name == "Spell Control" && self.State.Name == "Set Focus Speed")
            {
                if (self.floatValue.Name == "Time Per MP Drain UnCH")
                {
                    self.floatValue.Value = 0.009f;
                }
                else if (self.floatValue.Name == "Time Per MP Drain CH")
                {
                    self.floatValue.Value = 0.005f;
                }
            }

            orig(self);
        }

        private void ModifyNailDamage(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            //concentration buff
            if (hitInstance.AttackType is AttackTypes.Nail)
            {               
                hitInstance.Multiplier += (Concentration.concentrationvalue / Concentration.concentrationmax) * 0.4f;

                //style buff
                hitInstance.Multiplier *= (0.2f + (Style.rank*0.2f));

                //vt buff
                if (VesselTrigger.inVesselTrigger)
                {
                    hitInstance.Multiplier *= 1.3f;
                }               
            }

            Modding.Logger.Log(hitInstance.AttackType);
            Modding.Logger.Log(hitInstance.CircleDirection);
            Modding.Logger.Log(hitInstance.SpecialType);
            Modding.Logger.Log(hitInstance.Source);
            Modding.Logger.Log(hitInstance.MoveAngle);
            Modding.Logger.Log(hitInstance.MoveDirection);
            Modding.Logger.Log(hitInstance.MagnitudeMultiplier);

            orig(self, hitInstance);
        }

        private void CreateSwitch()
        {
            //Neutral Check is a check for if no directional keys are being held when a spell is triggered.
            //This triggers the weapon switch mechanic.
            FsmState neutralstate = HeroController.instance.spellControl.CreateState("Neutral Check");

            neutralstate.InsertMethod(0,() =>
            {
                if (HeroController.instance.move_input == 0 && HeroController.instance.vertical_input == 0)
                {
                    HeroController.instance.spellControl.SendEvent("SWITCH");
                } else
                {
                    HeroController.instance.spellControl.SendEvent("REGULAR");
                }
            });

            //Create the Switch state.
            FsmState switchstate = HeroController.instance.spellControl.CreateState("Switch State");
            switchstate.AddMethod(() =>
            {
                WeaponSwitch();
                HeroController.instance.spellControl.SendEvent("SWITCHED");
            });

            //Transitions
            HeroController.instance.spellControl.ChangeTransition("Inactive", "QUICK CAST", "Neutral Check");
            HeroController.instance.spellControl.ChangeTransition("Button Down", "BUTTON UP", "Neutral Check");
            neutralstate.AddTransition("SWITCH", "Switch State");
            neutralstate.AddTransition("REGULAR", "Can Cast? QC");
            switchstate.AddTransition("SWITCHED", "Inactive");

        }

        private void DeactivateWeapons()
        {
            HeroController.instance.GetComponent<Yamato.Base>().enabled = false;
        }

        private void ActivateYamato()
        {
            HeroController.instance.GetComponent<Yamato.Base>().enabled = true;
        }

        private void WeaponSwitch()
        {
            DeactivateWeapons();
            if (weapon == "Force Edge")
            {
                weapon = "Yamato";
            } 
            else if (weapon == "Yamato") 
            {
                weapon = "Beowulf";
                ActivateYamato();
            } 
            else if (weapon == "Beowulf" || weapon == null)
            {
                weapon = "Force Edge";
            }
        }

    }
}