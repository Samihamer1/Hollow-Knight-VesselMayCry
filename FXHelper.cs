using FrogCore.Ext;
using System;
using System.Security.Policy;
using UnityEngine;

namespace VesselMayCry
{
    internal class FXHelper : MonoBehaviour
    {

        public static tk2dSprite slashsprite;
        public static tk2dSprite slashaltsprite;
        public static tk2dSprite wallslashsprite;
        public static tk2dSprite upslashsprite;
        public static tk2dSprite downslashsprite;

        public static GameObject whiteburst;
        public static GameObject soundobject;
        public static GameObject knightattacks;
        public static GameObject bursteffect;
        public static GameObject knighteffects;
        public static GameObject nachargeeffect;
        public static GameObject sharpflash;
        public static GameObject sharpflashmini;
        public static GameObject sharpflashmed;
        public static GameObject hellburst;
        public static GameObject hellburst2;
        public static GameObject soulburst;
        public static GameObject gslam;
        public static GameObject cyclonecopy;
        public static GameObject grubberflyr;
        public static GameObject grubberflyl;

        public static AudioSource knightaudio;

        private static Dictionary<string, AudioClip> audioclips = new Dictionary<string, AudioClip> ();
        private static Dictionary<string, GameObject> effects = new Dictionary<string, GameObject> ();
        private static string[] cameralevels = new string[3];

        public static void Initialise()
        {
            //Something must suffer being messy, if the rest can be clean as a result.
            audioclips = new Dictionary<string, AudioClip>();
            effects = new Dictionary<string, GameObject>();

            knightattacks = HeroController.instance.gameObject.Child("Attacks");
            slashsprite = knightattacks.Child("Slash").GetComponent<tk2dSprite>();
            slashaltsprite = knightattacks.Child("AltSlash").GetComponent<tk2dSprite>();
            wallslashsprite = knightattacks.Child("WallSlash").GetComponent<tk2dSprite>();
            upslashsprite = knightattacks.Child("UpSlash").GetComponent<tk2dSprite>();
            downslashsprite = knightattacks.Child("DownSlash").GetComponent<tk2dSprite>();

            knighteffects = HeroController.instance.gameObject.Child("Effects");
            bursteffect = knighteffects.Child("SD Burst Glow");
            whiteburst = Instantiate(bursteffect, HeroController.instance.transform);
            whiteburst.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            nachargeeffect = knighteffects.Child("NA Charged");
            hellburst = Instantiate(knighteffects.Child("Soul Burst"), knighteffects.transform);
            hellburst.transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);
            Material newmat = new Material(hellburst.GetComponent<SpriteRenderer>().materials[0]);
            newmat.color = new Color(1, 0.4f, 1, 1);
            hellburst.GetComponent<SpriteRenderer>().material = newmat;
            hellburst2 = Instantiate(hellburst, knighteffects.transform);
            hellburst2.transform.localScale = new Vector3(4f, 4f, 4f);
            newmat = new Material(hellburst.GetComponent<SpriteRenderer>().materials[0]);
            newmat.color = new Color(1, 0.7f, 1, 1);
            hellburst.GetComponent<SpriteRenderer>().material = newmat;
            soulburst = knighteffects.Child("Soul Burst");
            sharpflash = HeroController.instance.gameObject.Child("Effects").Child("SD Sharp Flash");
            sharpflashmed = Instantiate(sharpflash, HeroController.instance.transform);
            sharpflashmed.transform.localScale = new Vector3(4, 4, 4);
            sharpflashmini = Instantiate(sharpflash, HeroController.instance.transform);
            sharpflashmini.transform.localScale = new Vector3(2, 2, 2);
            gslam = Instantiate(HeroController.instance.gameObject.Child("Spells").Child("Q Slam"), knighteffects.transform);
            gslam.Child("Hit L").Destroy();
            gslam.Child("Hit R").Destroy();
            gslam.LocateMyFSM("HitboxControl").Destroy();
            cyclonecopy = Instantiate(knightattacks.Child("Cyclone Slash"), HeroController.instance.transform);
            cyclonecopy.Child("Hits").Destroy();
            cyclonecopy.GetComponent<PlayMakerFSM>().Destroy();

            soundobject = new GameObject("SoundObject");
            soundobject.transform.parent = HeroController.instance.gameObject.Child("Sounds").transform;
            knightaudio = soundobject.AddComponent<AudioSource>();

            //effects
            effects.Add("SharpFlash", sharpflash);
            effects.Add("WhiteBurst", whiteburst);
            effects.Add("PinkBurst", bursteffect);
            effects.Add("SharpFlashM", sharpflashmed);
            effects.Add("SharpFlashS", sharpflashmini);
            effects.Add("HellOnEarthBurst", hellburst);
            effects.Add("HellOnEarthBurst2", hellburst2);
            effects.Add("SoulBurst", soulburst);
            effects.Add("Q Slam", gslam);

            //audio
            audioclips.Add("JCEStart", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.JCEStart.wav"));
            audioclips.Add("JCEEnd", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.JCEEnd.wav"));
            audioclips.Add("JC", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.JC.wav"));
            audioclips.Add("Shockwave", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.Shockwave.wav"));
            audioclips.Add("HellOnEarthBurst", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.HellOnEarthBurst.wav"));
            audioclips.Add("HellOnEarthSpark", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.HellOnEarthSpark.wav"));
            audioclips.Add("Shockwave2", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.Shockwave2.wav"));
            audioclips.Add("VTEnd", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.VTEnd.wav"));
            audioclips.Add("VTStart", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.VTStart.wav"));
            audioclips.Add("BeowulfSecondCharge", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.BeowulfSecondCharge.wav"));
            audioclips.Add("ButterflyBlade", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.ButterflyBlade.wav"));
            audioclips.Add("WeaponSwitch", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.WeaponSwitch.wav"));
            audioclips.Add("GreatSlash", knightattacks.Child("Dash Slash").GetComponent<AudioSource>().clip);
            audioclips.Add("DeepStinger", ResourceLoader.LoadAudioClip("VesselMayCry.Resources.Sounds.DeepStinger.wav"));

            //camerastuff
            cameralevels[0] = "SmallShake";
            cameralevels[1] = "AverageShake";
            cameralevels[2] = "BigShake";
            
        }

        public static void PlayAudio(string name, float volumemultiplier) 
        {
            if (audioclips.ContainsKey(name))
            {
                knightaudio.pitch = 1f;
                knightaudio.PlayOneShot(audioclips[name], GameManager.instance.GetImplicitCinematicVolume() * volumemultiplier);
            }
        } 

        public static void PlayAudioRandomPitch(string name, float volumemultiplier)
        {
            if (audioclips.ContainsKey(name))
            {
                float random = UnityEngine.Random.Range(0.8f, 1.2f);
                knightaudio.pitch = random;
                knightaudio.PlayOneShot(audioclips[name], GameManager.instance.GetImplicitCinematicVolume() * volumemultiplier);
            }
        }

        public static void ActivateEffect(string name)
        {
            if (effects.ContainsKey(name))
            {
                effects[name].SetActive(true);
            }
        }

        public static void CameraShake(int level)
        {
            GameCameras.instance.gameObject.Child("CameraParent").GetComponent<PlayMakerFSM>().SendEvent(cameralevels[level]);
        }

        public static void Setup(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            foreach (var o in ObjectPool.instance.startupPools)
            {
                switch (o.prefab.name)
                {
                    case "Grubberfly BeamR":
                        grubberflyr = Instantiate(o.prefab);
                        grubberflyr.SetActive(false);
                        DontDestroyOnLoad(grubberflyr);
                        break;
                    case "Grubberfly BeamL":
                        grubberflyl = Instantiate(o.prefab);
                        grubberflyl.SetActive(false);
                        DontDestroyOnLoad(grubberflyl);
                        break;
                }
            }

        }
    }
}
