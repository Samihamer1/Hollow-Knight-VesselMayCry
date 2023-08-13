namespace VesselMayCry
{
    internal class VesselTrigger : MonoBehaviour
    {
        private GameObject vtcanvas;
        private GameObject vtfg;
        private GameObject vtbg;
        public static Image vtbar;
        private float vtval = 0;
        private float vtmax = 5;
        ParticleSystem newpart;
        public static bool inVesselTrigger = false;

        private void Start()
        {
            UpdateBar();
            AddVT();

            //bar hooks
            On.HeroController.LeaveScene += TransparentBars;
            On.HeroController.FinishedEnteringScene += CreateBars;

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

       private IEnumerator DeactivateVesselTrigger()
        {
            inVesselTrigger = true;
            for (int i = 1; i < 19; i++)
            {
                if (i % 3 == 0)
                {
                    HeroController.instance.AddHealth(1);
                    vtval -= 1;
                    UpdateBar();
                }
                yield return new WaitForSeconds(1);
            }
            newpart.Stop();
            inVesselTrigger = false;
        } 
        
        private void AddVT()
        {
            FsmState focusheal = HeroController.instance.spellControl.GetState("Focus Heal");
            focusheal.RemoveAction(11);
            focusheal.AddMethod(() =>
            {
                if (vtval == vtmax)
                {
                    ActivateVesselTrigger();
                } else
                {
                    vtval += 1;
                }               
                UpdateBar();
            });

        }

        private void OnDisable()
        {

            //bar hooks
            On.HeroController.FinishedEnteringScene -= CreateBars;
            On.HeroController.LeaveScene -= TransparentBars;
        }
        
        public void UpdateBar()
        {
            if (vtbar != null)
            {
                float value = vtval / vtmax;
                vtbar.fillAmount = value;
            }
        }

        //bar stuff
        private void TransparentBars(On.HeroController.orig_LeaveScene orig, HeroController self, GatePosition? gate)
        {
            orig.Invoke(self, gate);
            if (vtcanvas != null)
            {
                vtcanvas.SetActive(false);
            }
        }

        private void CreateBars(On.HeroController.orig_FinishedEnteringScene orig, HeroController self, bool setHazardMarker, bool preventRunBob)
        {
            orig.Invoke(self, setHazardMarker, preventRunBob);
            float scale = 0.3f;

            //canvas
            vtcanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280f, 720f));
            vtcanvas.GetComponent<Canvas>().sortingOrder = 0;
            vtcanvas.name = "VT";

            //bg
            Texture2D bgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.concentrationbg.png");
            Sprite bgsprite = Sprite.Create(bgtexture, new Rect(0, 0, bgtexture.width, bgtexture.height), new Vector2(0.5f, 0.5f));
            vtbg = CanvasUtil.CreateImagePanel(vtcanvas, bgsprite, new CanvasUtil.RectData(bgsprite.rect.size * scale, new Vector2(-330f, 595f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));

            //fg
            Texture2D fgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.vtfg.png");
            Sprite fgsprite = Sprite.Create(fgtexture, new Rect(0, 0, fgtexture.width, fgtexture.height), new Vector2(0.5f, 0.5f));
            vtfg = CanvasUtil.CreateImagePanel(vtcanvas, fgsprite, new CanvasUtil.RectData(fgsprite.rect.size * scale, new Vector2(-330f, 595f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
            //bar settings
            vtbar = vtfg.GetComponent<Image>();
            vtbar.type = Image.Type.Filled;
            vtbar.fillMethod = Image.FillMethod.Horizontal;
            vtbar.preserveAspect = false;
            UpdateBar();
        }
    }
}
