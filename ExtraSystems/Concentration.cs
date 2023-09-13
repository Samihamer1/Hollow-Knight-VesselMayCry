using UnityEngine;

namespace VesselMayCry
{
    internal class Concentration: MonoBehaviour
    {
        private static bool gainconcentration = true;
        public static float concentrationvalue = 0;
        public static float concentrationmax = 100;
        public static float concentrationlevel2 = 60;
        private static float nailconcentration = 7;


        private void Start()
        {
            ModHooks.SlashHitHook += Gain;
            ModHooks.TakeHealthHook += ConcentrationReset;
            ModHooks.DoAttackHook += ResetGain;
            ModHooks.AfterAttackHook += Lose;

        }

        private void OnDisable()
        {
            ModHooks.SlashHitHook -= Gain;
            ModHooks.TakeHealthHook -= ConcentrationReset;
            ModHooks.DoAttackHook -= ResetGain;
            ModHooks.AfterAttackHook -= Lose;

        }
        private void Gain(Collider2D otherCollider, GameObject slash)
        {
            //filter collider
            if (otherCollider != null)
            {
                HealthManager manager = otherCollider.gameObject.GetComponent<HealthManager>();
                if (manager != null && gainconcentration == true)
                {
                    gainconcentration = false;
                    float conc = nailconcentration;
                    if (Charms.purefocusequipped)
                    {
                        conc = 2.5f;
                    }
                    AddConcentration(conc);
                }
            }

        }
        private IEnumerator WaitForAttackEnd()
        {

            if (HeroController.instance.playerData.equippedCharm_32)
            {
                yield return new WaitForSeconds(HeroController.instance.ATTACK_COOLDOWN_TIME_CH);
            } else
            {
                yield return new WaitForSeconds(HeroController.instance.ATTACK_COOLDOWN_TIME);
            }
            
            if (gainconcentration && !Charms.slashscopeequipped)
            {
                concentrationvalue -= nailconcentration;
                if (concentrationvalue < 0)
                {
                    concentrationvalue = 0;
                }
            }
        }
        private void Lose(AttackDirection direction)
        {
            StartCoroutine(WaitForAttackEnd());
        }
        private int ConcentrationReset(int damage)
        {
            if (Charms.strongmindequipped)
            {
                if (concentrationvalue - concentrationmax / 2 > 0)
                {
                    concentrationvalue -= concentrationmax / 2;
                } else
                {
                    concentrationvalue = 0;
                }
            }
            else
            {
                concentrationvalue = 0;
            }
            return damage;
        }

        private void ResetGain()
        {
            gainconcentration = true;
        }

        public static void AddConcentration(float amount)
        {
            concentrationvalue += amount;
            if (concentrationvalue > concentrationmax)
            {
                concentrationvalue = concentrationmax;
            }
            if (concentrationvalue < 0)
            {
                concentrationvalue = 0;
            }
        }
    }
}
