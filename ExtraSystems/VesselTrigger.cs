using Vasi;
using static UnityEngine.ParticleSystem;

namespace VesselMayCry
{
    internal class VesselTrigger : MonoBehaviour
    {
        public static float vtval = 0;
        public static float vtmax = 5;
        private static ParticleSystem newpart;
        public static bool inVesselTrigger = false;


        private void Start()
        {
            AddVT();
            ModHooks.TakeDamageHook += SafetyNet;

        }

        private void OnDisable()
        {
            ModHooks.TakeDamageHook -= SafetyNet;
        }

        private int SafetyNet(ref int hazardType, int damage)
        {
            if (inVesselTrigger && damage > HeroController.instance.playerData.health + HeroController.instance.playerData.healthBlue && !Charms.hivebloodequipped)
            {
                float postval = vtval - damage;
                if (postval < 0)
                {
                    SetVTVal(postval);
                    return 0;
                }
            }
            return damage;
        }

        public static void SetVTVal(float val)
        {
            //The HP ints are now used for VT values as focus is no longer dependent on your max hp.
            vtval = val;
            HeroController.instance.spellControl.GetOrCreateInt("HP").Value = (int)val;
            HeroController.instance.spellControl.GetOrCreateInt("Max HP").Value = (int)vtmax;
        }

        private void ActivateVesselTrigger()
        {
            if (!inVesselTrigger)
            {
                ParticleSystem particles = HeroController.instance.gameObject.Child("Charm Effects").Child("Fury").GetComponent<ParticleSystem>();
                if (newpart == null)
                {
                    newpart = Instantiate(particles, HeroController.instance.transform);
                    // i do not care
#pragma warning disable CS0618 // Type or member is obsolete
                    newpart.startColor = new Color(0, 0.240f, 1, 1);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                newpart.Play();
                StartCoroutine(DeactivateVesselTrigger());
            }
           
        }

        public static void Reset()
        {
            if (!Charms.hivebloodequipped)
            {
                inVesselTrigger = false;
                SetVTVal(0);
            }            
        }

        private static void AddHealth(int hp)
        {
            if (Charms.deephealingequipped)
            {
                HeroController.instance.AddHealth(hp);
            }
            HeroController.instance.AddHealth(hp);
        }

        private IEnumerator DeactivateVesselTrigger()
        {
            inVesselTrigger = true;
            FXHelper.PlayAudio("VTStart", 0.5f);
            int count = 0;
            while (inVesselTrigger && !Charms.hivebloodequipped)
            {
                if (count % 3 == 0)
                {
                    AddHealth(1);
                    SetVTVal(vtval -= 1);
                    if (vtval == 0)
                    {
                        break;
                    }
                }
                count++;
                if (Charms.morepowerequipped)
                {
                    yield return new WaitForSeconds(1);
                }
                yield return new WaitForSeconds(1);
            }
            newpart.Stop();
            Destroy(newpart);
            inVesselTrigger = false;
            if (vtval < 0)
            {
                SetVTVal(0);
            }
            FXHelper.PlayAudio("VTEnd", 0.5f);
        } 
        
        private void AddVT()
        {
            FsmState focusheal = HeroController.instance.spellControl.GetState("Focus Heal");
            SetIntValue setaction = new SetIntValue();
            setaction.intValue = 0;
            setaction.intVariable = HeroController.instance.spellControl.GetOrCreateInt("Health Increase");

            focusheal.InsertAction(0, setaction);

            focusheal.AddMethod(() =>
            {
                if (vtval == vtmax && !inVesselTrigger)
                {
                    ActivateVesselTrigger();
                } else if (inVesselTrigger && !Charms.hivebloodequipped)
                {
                    inVesselTrigger = false;
                } else if (inVesselTrigger && Charms.hivebloodequipped)
                {
                    AddHealth(1);
                }
                else
                {
                    SetVTVal(vtval += 1);
                }
            });
            
        }

        public static IEnumerator ActivateHiveTrigger()
        {
            ParticleSystem particles = HeroController.instance.gameObject.Child("Charm Effects").Child("Fury").GetComponent<ParticleSystem>();
            newpart = Instantiate(particles, HeroController.instance.transform);
            // i do not care
#pragma warning disable CS0618 // Type or member is obsolete
            newpart.startColor = new Color(1f, 0.95f, 0.05f, 1);
#pragma warning restore CS0618 // Type or member is obsolete
            newpart.Play();
            float count = 0;
            while (Charms.hivebloodequipped)
            {
                inVesselTrigger = true;

                count++;
                if (count % 7 == 0)
                {
                    AddHealth(1);
                }

                yield return new WaitForSeconds(1);
            }
            Destroy(newpart);
            inVesselTrigger = false;
        }

    }
}
