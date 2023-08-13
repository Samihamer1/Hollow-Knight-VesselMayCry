namespace VesselMayCry
{
    internal class Concentration: MonoBehaviour
    {
        private static bool gainconcentration = true;
        public static float concentrationvalue = 0;
        public static float concentrationmax = 100;
        private static float nailconcentration = 7;
        private GameObject concentrationcanvas;
        private GameObject concentrationfg;
        private GameObject concentrationbg;
        public static Image concentrationbar;

        private void Start()
        {
            UpdateBar();
            ModHooks.SlashHitHook += Gain;
            ModHooks.TakeHealthHook += ConcentrationReset;
            ModHooks.DoAttackHook += ResetGain;
            ModHooks.AfterAttackHook += Lose;

            //bar hooks
            On.HeroController.LeaveScene += TransparentBars;
            On.HeroController.FinishedEnteringScene += CreateBars;

        }

        private void OnDisable()
        {
            ModHooks.SlashHitHook -= Gain;
            ModHooks.TakeHealthHook -= ConcentrationReset;
            ModHooks.DoAttackHook -= ResetGain;
            ModHooks.AfterAttackHook -= Lose;

            //bar hooks
            On.HeroController.FinishedEnteringScene -= CreateBars;
            On.HeroController.LeaveScene -= TransparentBars;
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
                    concentrationvalue += nailconcentration;
                    if (concentrationvalue > concentrationmax)
                    {
                        concentrationmax = concentrationvalue;
                    }
                    UpdateBar();
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
            
            if (gainconcentration)
            {
                concentrationvalue -= nailconcentration;
                if (concentrationvalue < 0)
                {
                    concentrationvalue = 0;
                }
            }
            UpdateBar();
        }
        private void Lose(AttackDirection direction)
        {
            StartCoroutine(WaitForAttackEnd());
        }
        private int ConcentrationReset(int damage)
        {
            concentrationvalue = 0;
            UpdateBar();
            return damage;
        }

        private void ResetGain()
        {
            gainconcentration = true;
        }
        public void UpdateBar()
        {
            if (concentrationbar != null) { 
                float value = concentrationvalue / concentrationmax;
                concentrationbar.fillAmount = value;
            }
        }

        //bar stuff
        private void TransparentBars(On.HeroController.orig_LeaveScene orig, HeroController self, GatePosition? gate)
        {
            orig.Invoke(self, gate);
            if (concentrationcanvas != null)
            {
                concentrationcanvas.SetActive(false);
            }
        }

        private void CreateBars(On.HeroController.orig_FinishedEnteringScene orig, HeroController self, bool setHazardMarker, bool preventRunBob)
        {
            orig.Invoke(self, setHazardMarker, preventRunBob);
            float scale = 0.6f;

            //canvas
            concentrationcanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280f, 720f));
            concentrationcanvas.GetComponent<Canvas>().sortingOrder = 0;

            //bg
            Texture2D bgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.concentrationbg.png");
            Sprite bgsprite = Sprite.Create(bgtexture, new Rect(0, 0, bgtexture.width, bgtexture.height), new Vector2(0.5f, 0.5f));
            concentrationbg = CanvasUtil.CreateImagePanel(concentrationcanvas, bgsprite, new CanvasUtil.RectData(bgsprite.rect.size * scale, new Vector2(-180f, 615f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));

            //fg
            Texture2D fgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.concentrationfg.png");
            Sprite fgsprite = Sprite.Create(fgtexture, new Rect(0, 0, fgtexture.width, fgtexture.height), new Vector2(0.5f, 0.5f));
            concentrationfg = CanvasUtil.CreateImagePanel(concentrationcanvas, fgsprite, new CanvasUtil.RectData(fgsprite.rect.size * scale, new Vector2(-180f, 615f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
            //bar settings
            concentrationbar = concentrationfg.GetComponent<Image>();
            concentrationbar.type = Image.Type.Filled;
            concentrationbar.fillMethod = Image.FillMethod.Horizontal;
            concentrationbar.preserveAspect = false;
            UpdateBar();
        }

        private void Update()
        {
            if (HeroController.instance.cState.isPaused && concentrationcanvas != null)
            {
                concentrationcanvas.SetActive(false);
            } else if (concentrationcanvas != null)
            {
                concentrationcanvas.SetActive(true);
            }
        }
    }
}
