namespace VesselMayCry
{
    internal class Style: MonoBehaviour
    {
        Dictionary<string, int> attacklist = new Dictionary<string, int>();
        public static int rank = 0;
        public static float meter = 0;
        public static float metermax = 60;
        float meterloss = 7.5f;
        float[] meterlevels = { 20, 25, 35, 40, 50, 60, 70 };
        public static string[] ranks = {"D","C","B","A","S","SS","SSS"};

        private Dictionary<string, float> MeterGain = new Dictionary<string, float>()
        {
            {"Judgement Cut", 15},
            {"Upper Slash", 10 },
            {"Judgement Cut End", 30 },
            {"Aerial Cleave", 15 },
            {"Combo C", 8},
            {"Hell On Earth", 30 },
            {"Strong Punch", 10 },
            {"Strong PunchC", 25 },
            {"Dash Punch", 10 },
            {"Dash PunchC", 20 },
            {"Starfall", 15 },
            {"Uppercut", 20 },
            {"UppercutC", 25 },
            {"Lunar Phase", 15 },
            {"Deep Stinger", 15 },
            {"Stinger Strike", 30 },
            {"Overdrive", 15 },
            {"Drive", 10 },
            {"Round Trip", 15 },
            {"Blistering Swords", 10 },
            {"Spiral Swords", 10 },
            {"Slash", 8 },
            {"AltSlash", 8 },
            {"UpSlash", 8 },
            {"DownSlash", 8 }
        };


        private void Start()
        {
            On.HealthManager.TakeDamage += AddToList;
            ModHooks.TakeHealthHook += LargeLoss;

            
        }

        private int LargeLoss(int damage)
        {
            float loss = 50;
            attacklist.Clear();
            while (meter - loss <= 0)
            {
                float totalmeter = 0;
                for (int i = 0; i < rank; i++)
                {
                    totalmeter += meterlevels[i];
                }

                float newtotalmeter = totalmeter - loss;
                if (newtotalmeter < 0)
                {
                    meter = 0;
                    rank = 0;
                    return damage;
                }

                int newrank = 0;
                for (int i = 0; i < meterlevels.Length; i++)
                {
                    if (meterlevels[i] < newtotalmeter)
                    {
                        newtotalmeter -= meterlevels[i];
                        newrank = i;
                    } else
                    {
                        break;
                    }
                }

                meter = newtotalmeter;
                rank = newrank;
            }
            return damage;
        }

        private void Update()
        {
            if (!HeroController.instance.cState.isPaused)
            {
                meter -= (meterloss * Time.deltaTime);
                metermax = meterlevels[rank];
                if (meter < 0)
                {
                    if (rank > 0)
                    {
                        rank -= 1;
                        meter = metermax;
                    }
                    else
                    {
                        meter = 0;

                    }
                }
            }
        }
        private void AddToList(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig(self, hitInstance);
            string attackname = hitInstance.Source.name;
            string fullname = attackname + VesselMayCry.weapon;

            if (attacklist.ContainsKey(fullname))
            {
                //add to move if already there
                attacklist[fullname] += 1;
            } else if (attacklist.Count < 6) 
            {
                //add move if not full
                attacklist.Add(fullname, 1);
            } else if (attacklist.Count == 6)
            {
                //replace highest used move if full
                string key = attacklist.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                attacklist.Remove(key);
                attacklist.Add(fullname, 1);
               
            }

            //add to meter.
            int maxuses = 5;
            if (attacklist[fullname] < maxuses) {
                if (MeterGain.ContainsKey(attackname))
                {
                    meter += MeterGain[attackname];
                } else
                {
                    meter += 15;
                }
                
                if (meter > metermax && rank < 6)
                {
                    meter = meter - metermax;
                    rank += 1;
                } else if (meter > metermax && rank == 6)
                {
                    meter = metermax;
                }
            }
        }
    }
}
