namespace VesselMayCry
{
    internal class Style: MonoBehaviour
    {
        Dictionary<string, int> attacklist = new Dictionary<string, int>();
        public static int rank = 0;
        public static float meter = 0;
        public static float metermax = 60;
        float meterloss = 5.5f;
        float[] meterlevels = { 30, 40, 60, 70, 85, 100, 130 };
        public static string[] ranks = {"D","C","B","A","S","SS","SSS"};
        public static string[] ranksCharm = { "B", "A"};

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

        private void OnDisable()
        {
            On.HealthManager.TakeDamage -= AddToList;
            ModHooks.TakeHealthHook -= LargeLoss;
        }


        private int LargeLoss(int damage)
        {
            if (Charms.consistencykeyequipped)
            {
                rank = 2;
                meter = 0;
                attacklist.Clear();
            } else
            {
                if (rank - 2 < 0)
                {
                    rank = 0;
                    meter = 0;
                    attacklist.Clear();
                }
                else
                {
                    rank -= 2;
                }
                metermax = meterlevels[rank];
                meter = metermax * 0.3f;
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
                    //lazy
                    if (Charms.consistencykeyequipped)
                    {
                        if (rank > 2)
                        {

                            rank -= 1;
                            meter = metermax;
                        }
                        else
                        {
                            meter = 0;

                        }
                        if (rank < 2)
                        {
                            rank = 2;
                            meter = 0;
                        }
                    } else
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
                    AddToMeter(MeterGain[attackname]);
                } else
                {
                    AddToMeter(15);
                }
                
            }
        }

        public static void AddToMeter(float val)
        {
            meter += val;

            //im lazy
            if (Charms.consistencykeyequipped)
            {
                if (meter > metermax && rank < 3)
                {
                    meter = meter - metermax;
                    rank += 1;
                }
                else if (meter > metermax && rank == 3)
                {
                    meter = metermax;
                }
            } else
            {
                if (meter > metermax && rank < 6)
                {
                    meter = meter - metermax;
                    rank += 1;
                }
                else if (meter > metermax && rank == 6)
                {
                    meter = metermax;
                }
            }
           
        }
    }
}
