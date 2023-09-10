using Vasi;

namespace VesselMayCry
{
    internal class VesselTrigger : MonoBehaviour
    {
        public static float vtval = 0;
        public static float vtmax = 5;
        ParticleSystem newpart;
        public static bool inVesselTrigger = false;

        private void Start()
        {
            AddVT();


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

       public static void Reset()
        {
            inVesselTrigger = false;
            SetVTVal(0);
        }

       private IEnumerator DeactivateVesselTrigger()
        {
            inVesselTrigger = true;
            FXHelper.PlayAudio("VTStart", 0.5f);
            for (int i = 1; i < 19; i++)
            {
                if (!inVesselTrigger)
                {
                    break;
                }

                if (i % 3 == 0)
                {
                    HeroController.instance.AddHealth(1);
                    SetVTVal(vtval -= 1);
                }
                yield return new WaitForSeconds(1);
            }
            newpart.Stop();
            inVesselTrigger = false;
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
                if (vtval == vtmax)
                {
                    ActivateVesselTrigger();
                } else
                {
                    SetVTVal(vtval += 1);
                }               
            });
            
        }


    }
}
