using Modding.Menu;

namespace VesselMayCry
{
    internal class Style: MonoBehaviour
    {
        Dictionary<string, int> attacklist = new Dictionary<string, int>();
        public static int rank = 0;
        float meter = 0;
        float metermax = 60;
        float meterloss = 7.5f;
        float[] meterlevels = { 20, 25, 35, 40, 50, 60, 70 };
        string[] ranks = {"D","C","B","A","S","SS","SSS"};
        private GameObject stylecanvas;
        private GameObject stylemeterfg;
        private GameObject ranktext;
        //private GameObject stylemeterbg;
        public static Image stylemeterbar;

        private void Start()
        {
            On.HealthManager.TakeDamage += AddToList;
            ModHooks.TakeHealthHook += LargeLoss;
            //bar hooks
            On.HeroController.FinishedEnteringScene += CreateBars;
            On.HeroController.LeaveScene += TransparentBars;
            
        }

        private int LargeLoss(int damage)
        {
            float loss = 50;
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
            attacklist = new Dictionary<string, int>();
            return damage;
        }

        private void Update()
        {
            if (!HeroController.instance.cState.isPaused)
            {
                if (stylecanvas != null)
                {
                    stylecanvas.SetActive(true);
                }
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

                UpdateBar();
            } else
            {
                if (stylecanvas != null)
                {
                    stylecanvas.SetActive(false);
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
            if (attacklist[fullname] < 5) {
                meter += 15;
                if (meter > metermax && rank < 7)
                {
                    meter = meter - metermax;
                    rank += 1;
                } else if (meter > metermax && rank == 7)
                {
                    meter = metermax;
                }
            }
        }
        public void UpdateBar()
        {
            if (stylemeterbar != null)
            {
                float value = meter / metermax;
                stylemeterbar.fillAmount = value;
            }
            if (ranktext != null)
            {
                ranktext.GetComponent<Text>().text = ranks[rank];
                if (rank == 0 && meter == 0)
                {
                    ranktext.GetComponent<Text>().text = "";
                }
            }
        }

        private void TransparentBars(On.HeroController.orig_LeaveScene orig, HeroController self, GatePosition? gate)
        {
            orig.Invoke(self, gate);
            if (stylecanvas != null) {
                stylecanvas.SetActive(false);
            }
        }

        private void CreateBars(On.HeroController.orig_FinishedEnteringScene orig, HeroController self, bool setHazardMarker, bool preventRunBob)
        {
            orig.Invoke(self, setHazardMarker, preventRunBob);
            float scale = 0.15f;

            //canvas
            stylecanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280f, 720f));
            stylecanvas.GetComponent<Canvas>().sortingOrder = 0;

            //fg
            Texture2D fgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.stylemeter.png");
            Sprite fgsprite = Sprite.Create(fgtexture, new Rect(0, 0, fgtexture.width, fgtexture.height), new Vector2(0.5f, 0.5f));
            stylemeterfg = CanvasUtil.CreateImagePanel(stylecanvas, fgsprite, new CanvasUtil.RectData(fgsprite.rect.size * scale, new Vector2(550f, 330f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
            //bar settings
            stylemeterbar = stylemeterfg.GetComponent<Image>();
            stylemeterbar.type = Image.Type.Filled;
            stylemeterbar.fillMethod = Image.FillMethod.Horizontal;
            stylemeterbar.preserveAspect = false;

            ranktext = CanvasUtil.CreateTextPanel(stylecanvas,"",15,TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(100,100), new Vector2(550f, 280f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)), MenuResources.TrajanBold);
            UpdateBar();
        }
    }
}
