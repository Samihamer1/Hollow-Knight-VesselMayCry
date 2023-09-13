using Json.Net;
using Newtonsoft.Json;

namespace VesselMayCry
{
    internal class Charms : MonoBehaviour
    {
        public TextAsset jsonFile;
        public CharmTextData fulldata;
        private string[] fsmlist = {"Thorn Counter", "Enemy Recoil Up", "Hatchling Spawn", "Set Spell Cost", "Spawn Orbit Shield", "Weaverling Control", "Spawn Grimmchild", "Pool Flukes", "Pool Flukes"};

        private float autochargetimer = 0;
        private float autochargelimit = 1;
        private float bloodthirstycount = 0;
        private float bloodthirstydamagetimer = 0;
        public static bool bloodthirstybuff = false;

        public static bool deephealingequipped = false;
        public static bool morepowerequipped = false;
        public static bool slashscopeequipped = false;
        public static bool purefocusequipped = false;
        public static bool strongmindequipped = false;
        public static bool stillvoidequipped = false;
        public static bool quickfistsequipped = false;
        public static bool safetynetequipped = false;
        public static bool bloodthirstyequipped = false;
        public static bool consistencykeyequipped = false;
        public static bool hivebloodequipped = false;

        private TextAsset LoadJson(string path)
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(path);
            if (stream == null)
            {
                return null;
            }

            var String = new StreamReader(stream).ReadToEnd();
            TextAsset asset = new TextAsset(String);
            stream.Dispose();
            return asset;
        }

        private void Start()
        {
            DisableCharmEffects();
            HeroController.instance.NAIL_CHARGE_TIME_CHARM = 0.2f;
            HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = 0.2f;
            

            jsonFile = LoadJson("VesselMayCry.Resources.charmtext.json");
            fulldata = JsonConvert.DeserializeObject<CharmTextData>(jsonFile.text);

            ModHooks.LanguageGetHook += ChangeText;
            ModHooks.CharmUpdateHook += CharmValChanges;
            On.HutongGames.PlayMaker.Actions.SetFloatValue.OnEnter += FocusSpeed;
            On.HutongGames.PlayMaker.Actions.FloatCompare.OnEnter += HivebloodTimers;
            On.HealthManager.SendDeathEvent += Bloodthirsty;

            //notch changes
            On.GameManager.CalculateNotchesUsed += ChangeNotchCost;
        }

        private void HivebloodTimers(On.HutongGames.PlayMaker.Actions.FloatCompare.orig_OnEnter orig, FloatCompare self)
        {
            if (self.Fsm.GameObject.name == "Health" && self.Fsm.Name == "Hive Health Regen" && self.State.Name.StartsWith("Recover "))
            {
                self.float2.Value = 999;
            }
            else if (self.Fsm.GameObject.name == "Blue Health Hive(Clone)" && self.Fsm.Name == "blue_health_display" && self.State.Name.StartsWith("Regen "))
            {
                self.float2.Value = 999;
            }

            orig(self);
        }

        private void Bloodthirsty(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            if (self.gameObject.name != "Knight")
            {
                bloodthirstycount++;

                HeroController.instance.AddMPCharge(11);
                if (bloodthirstycount % 2 == 0)
                {
                    bloodthirstydamagetimer = 15;
                }

                if (bloodthirstycount % 3 == 0)
                {
                    Style.AddToMeter(20);
                    Concentration.AddConcentration(10);
                } 
            }
            orig.Invoke(self);
        }

        private void ChangeNotchCost(On.GameManager.orig_CalculateNotchesUsed orig, GameManager self)
        {
            PlayerData data = HeroController.instance.playerData;
            data.charmCost_19 = 2;
            data.charmCost_33 = 1;
            data.charmCost_15 = 1;
            data.charmCost_12 = 1;
            data.charmCost_39 = 2;
            data.charmCost_38 = 2;
            data.charmCost_11 = 2;
        }

        private void FocusSpeed(On.HutongGames.PlayMaker.Actions.SetFloatValue.orig_OnEnter orig, SetFloatValue self)
        {
            //shoutout to exempt medic
            if (self.Fsm.GameObject.name == "Knight" && self.Fsm.Name == "Spell Control" && self.State.Name == "Set Focus Speed")
            {
                if (self.floatValue.Name == "Time Per MP Drain UnCH")
                {
                    self.floatValue.Value = 0.005f;
                }
                else if (self.floatValue.Name == "Time Per MP Drain CH")
                {
                    self.floatValue.Value = 0.005f;
                }
            }

            orig(self);
        }
        private void DisableCharmEffects()
        {
            GameObject charmfx = HeroController.instance.gameObject.Child("Charm Effects");
            foreach (string name in fsmlist)
            {
                PlayMakerFSM fsm = charmfx.LocateMyFSM(name);
                if (fsm != null)
                {
                    fsm.enabled = false;
                }
            }
        }

        private void CharmValChanges(PlayerData data, HeroController controller)
        {
            DisableCharmEffects();
            //trigger happy
            if (data.equippedCharm_7)
            {
                autochargetimer = 0;
            }
            deephealingequipped = data.equippedCharm_34;
            morepowerequipped = data.equippedCharm_11;
            slashscopeequipped = data.equippedCharm_19;
            purefocusequipped = data.equippedCharm_33;
            strongmindequipped = data.equippedCharm_15;
            stillvoidequipped = data.equippedCharm_12;
            quickfistsequipped = data.equippedCharm_26;
            safetynetequipped = data.equippedCharm_38;
            bloodthirstyequipped = data.equippedCharm_39;
            consistencykeyequipped = data.equippedCharm_22;
            hivebloodequipped = data.equippedCharm_29;
            if (hivebloodequipped)
            {
                StartCoroutine(VesselTrigger.ActivateHiveTrigger());
            }

            bool reset = true;
            foreach (int charm in data.equippedCharms)
            {
                if (charm != 36)
                {
                    reset = false;
                }
            }
            if (reset || data.equippedCharms.Count == 0)
            {
                data.charmSlotsFilled = 0;
            }
        }

        private void FixedUpdate()
        {
            PlayerData data = HeroController.instance.playerData;
            //trigger happy
            if (data.equippedCharm_7)
            {
                autochargetimer += Time.deltaTime;
                if (autochargetimer > autochargelimit && data.MPCharge >= 33 && !VesselTrigger.inVesselTrigger && VesselTrigger.vtval < VesselTrigger.vtmax)
                {
                    autochargetimer = 0;
                    HeroController.instance.TakeMP(33);
                    VesselTrigger.SetVTVal(VesselTrigger.vtval += 1);
                }
            }
            if (bloodthirstyequipped)
            {
                bloodthirstydamagetimer -= Time.deltaTime;
                if (bloodthirstydamagetimer > 0)
                {
                    bloodthirstybuff = true;
                } else
                {
                    bloodthirstybuff = false;
                }
            }
        }
       
        private string ChangeText(string key, string sheetTitle, string orig)
        {
            if (sheetTitle == "UI")
            {
                foreach (CharmText keypair in fulldata.charmtextdata)
                {
                    if (keypair.key == key)
                    {
                        return keypair.text;
                    }
                }
            }
            return orig;
            
        }

        private void OnDisable()
        {
            ModHooks.LanguageGetHook -= ChangeText;
            ModHooks.CharmUpdateHook -= CharmValChanges;
            On.HutongGames.PlayMaker.Actions.SetFloatValue.OnEnter -= FocusSpeed;
            On.HutongGames.PlayMaker.Actions.FloatCompare.OnEnter -= HivebloodTimers;
        }
    }



    public class CharmText
    {
        public string key;
        public string text;
    }

    public class CharmTextData
    {
        public CharmText[] charmtextdata;
    }
}
