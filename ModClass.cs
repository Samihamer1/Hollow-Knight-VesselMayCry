using VesselMayCry.Helpers;
using VesselMayCry.Weapons;

namespace VesselMayCry
{
    public class VesselMayCry : Mod
    {
        new public string GetName() => "Vessel May Cry";
        public override string GetVersion() => "v1.0.0.1";


        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            On.HeroController.Start += BecomeTheStormThatIsApproaching;
            SpritePositions.Initialise();
            FXHelper.Setup(preloadedObjects);
            CanvasHandler.Initialise();

            
        }

        public static string weapon;
        private void BecomeTheStormThatIsApproaching(On.HeroController.orig_Start orig, HeroController self)
        {

            //Hey, change to local position for the guis.
            orig.Invoke(self);

            FXHelper.Initialise();

            //anims
            Anims.AnimationInit();

            //weapon hooks and base value changes
            CreateSwitch();
            YamatoBase yamato = HeroController.instance.gameObject.AddComponent<YamatoBase>();
            BeowulfBase beowulf = HeroController.instance.gameObject.AddComponent<BeowulfBase>();
            MirageEdgeBase mirageedge = HeroController.instance.gameObject.AddComponent<MirageEdgeBase>();

            yamato.enabled = false;
            beowulf.enabled = false;
            mirageedge.enabled = false;
            
            //Quake Invul is now only used for super moves. One of the main compatiblity issues? Maybe try to fix. Not urgent
            HeroController.instance.INVUL_TIME_QUAKE = 3f;

            //concentration
            HeroController.instance.gameObject.AddComponent<Concentration>();

            //style
            HeroController.instance.gameObject.AddComponent<Style>();

            //vessel trigger
            HeroController.instance.gameObject.AddComponent<VesselTrigger>();

            //Charms
            HeroController.instance.gameObject.AddComponent<Charms>();

            //switch
            WeaponSwitch();

            //damage mods
            On.HealthManager.Hit += ModifyNailDamage;

            //reset stuff on mp clear
            On.HeroController.ClearMP += ClearAll;


        }

        private void ClearAll(On.HeroController.orig_ClearMP orig, HeroController self)
        {
            orig.Invoke(self);
            Style.Reset();
            VesselTrigger.Reset();
            Concentration.Reset();
        }

        private void ModifyNailDamage(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            //concentration buff - listen i know i said nail damage but like im not changing the damage type of scream so
            if (hitInstance.AttackType is AttackTypes.Nail || hitInstance.AttackType is AttackTypes.Spell)
            {               
                hitInstance.Multiplier += (Concentration.concentrationvalue / Concentration.concentrationmax) * 0.4f;

                //style buff
                hitInstance.Multiplier *= (0.2f + (Style.rank*0.2f));

                //vt buff
                if (VesselTrigger.inVesselTrigger && !Charms.hivebloodequipped)
                {
                    hitInstance.Multiplier *= 1.3f;
                }               
                if (VesselTrigger.inVesselTrigger && Charms.hivebloodequipped)
                {
                    hitInstance.Multiplier *= 1.1f;
                }
                
                //scream isnt meant to be that good. its only a notch.
                if (hitInstance.AttackType is AttackTypes.Spell)
                {
                    hitInstance.Multiplier *= 0.55f;
                }

                //bloodthirsty
                if (Charms.bloodthirstybuff)
                {
                    hitInstance.Multiplier *= 1.1f;
                }

                //overkill
                if (Charms.overkillvowequipped && !Charms.hivebloodequipped)
                {
                    hitInstance.Multiplier *= 1.35f;
                } else if (Charms.overkillvowequipped && Charms.hivebloodequipped)
                {
                    hitInstance.Multiplier *= 1.15f;
                }

            }

            orig(self, hitInstance);
        }

        private void CreateSwitch()
        {
            //Neutral Check is a check for if no directional keys are being held when a spell is triggered.
            //This triggers the weapon switch mechanic.
            FsmState neutralstate = HeroController.instance.spellControl.CreateState("Neutral Check");

            neutralstate.InsertMethod(0, () =>
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
            HeroController.instance.spellControl.ChangeTransition("Has Fireball?", "CANCEL", "Switch State");
            neutralstate.AddTransition("SWITCH", "Switch State");
            neutralstate.AddTransition("REGULAR", "Can Cast? QC");
            switchstate.AddTransition("SWITCHED", "Inactive");

        }


        private void DeactivateWeapons()
        {
            HeroController.instance.GetComponent<YamatoBase>().enabled = false;
            HeroController.instance.GetComponent<BeowulfBase>().enabled = false;
            HeroController.instance.GetComponent<MirageEdgeBase>().enabled = false;
        }

        private void ActivateYamato()
        {
            HeroController.instance.GetComponent<YamatoBase>().enabled = true;
        }

        private void ActivateBeowulf()
        {
            HeroController.instance.GetComponent<BeowulfBase>().enabled = true;
        }

        private void ActivateMirageEdge()
        {
            HeroController.instance.GetComponent<MirageEdgeBase>().enabled = true;
        }

        private void WeaponSwitch()
        {
            DeactivateWeapons();
            if (weapon == "Mirage Edge")
            {
                weapon = "Yamato";
                ActivateYamato();
            } 
            else if (weapon == "Yamato") 
            {
                weapon = "Beowulf";
                ActivateBeowulf();
            } 
            else if (weapon == "Beowulf" || weapon == null)
            {
                weapon = "Mirage Edge";
                ActivateMirageEdge();
            }
            FXHelper.PlayAudio("WeaponSwitch", 1.5f);
            
        }

    }
}