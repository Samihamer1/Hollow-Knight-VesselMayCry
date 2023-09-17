namespace VesselMayCry
{
    internal class CanvasHandler : MonoBehaviour
    {
        private static CanvasHandler instance;

        private float regularscale = 1f;
        private float vtscale = 1f;
        private float weaponscale = 0.6f;
        private float stylescale = 1f;

        private Vector3 concentrationpos = new Vector3(-485, 395);
        private Vector3 stylepos = new Vector3(800, -50);
        private Vector3 vtpos = new Vector3(-320, 370);
        private Vector3 primarypos = new Vector3(620f, -375);
        private Vector3 secondarypos = new Vector3(810, -410f);

        private GameObject vmccanvas;
        private GameObject concentrationbg;
        private GameObject concentrationbg2;
        private GameObject concentrationbg3;
        private GameObject concentrationfg;
        private GameObject concentrationcr;
        private GameObject concentrationcr2;
        private GameObject concentrationcr3;
        private List<GameObject> stylefg = new List<GameObject>();
        private List<GameObject> stylebg = new List<GameObject>();
        private List<GameObject> vtfg = new List<GameObject>();
        private GameObject vtfghive;
        private GameObject vtbg;
        private GameObject beowulfimage;
        private GameObject mirageimage;
        private GameObject yamatoimage;

        private PlayMakerFSM blankerwhite;
        private PlayMakerFSM tk2dblanker;

        private Concentration concentration;
        private VesselTrigger trigger;
        private Style style;

        public static void Initialise()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasHandler>();
                if (instance == null)
                {
                    GameObject CanvasHandler = new GameObject("Canvas Handler");
                    instance = CanvasHandler.AddComponent<CanvasHandler>();
                    DontDestroyOnLoad(CanvasHandler);
                }
            }
        }

        private void Awake()
        {
            //canvas
            vmccanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920f, 1080f));
            vmccanvas.GetComponent<Canvas>().sortingOrder = 0;
            vmccanvas.transform.position = new Vector3(0, 0);
            UnityEngine.Object.DontDestroyOnLoad(vmccanvas);
            vmccanvas.name = "VMCCanvas";

            //concentration
            concentrationbg = CreatePanel("VesselMayCry.Resources.UI.ConcBG1.png", regularscale, concentrationpos, false);
            concentrationbg2 = CreatePanel("VesselMayCry.Resources.UI.ConcBG2.png", regularscale, concentrationpos, false);
            concentrationbg3 = CreatePanel("VesselMayCry.Resources.UI.ConcBG3.png", regularscale, concentrationpos, false);

            concentrationfg = CreatePanel("VesselMayCry.Resources.UI.ConcFG1.png", regularscale, concentrationpos, true);

            concentrationcr = CreatePanel("VesselMayCry.Resources.UI.ConcCR1.png", regularscale, concentrationpos, false);
            concentrationcr2 = CreatePanel("VesselMayCry.Resources.UI.ConcCR2.png", regularscale, concentrationpos, false);
            concentrationcr3 = CreatePanel("VesselMayCry.Resources.UI.ConcCR3.png", regularscale, concentrationpos, false);

            //style

            for (int i = 1; i < 8; i++)
            {
                GameObject bg = CreatePanel("VesselMayCry.Resources.UI.Style" + i + "BG.png", stylescale, stylepos, true);
                bg.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                stylebg.Add(bg);
                stylefg.Add(CreatePanel("VesselMayCry.Resources.UI.Style" + i + "FG.png", stylescale, stylepos, false));
            }

            //vt

            vtbg = CreatePanel("VesselMayCry.Resources.UI.VTBG.png", vtscale, vtpos, false);

            for (int i = 0; i < 6; i++)
            {
                vtfg.Add(CreatePanel("VesselMayCry.Resources.UI.VT"+i+".png", vtscale, vtpos, false));
            }


            vtfghive = CreatePanel("VesselMayCry.Resources.UI.VTFGHive.png", vtscale, vtpos, true);

            yamatoimage = CreatePanel("VesselMayCry.Resources.Images.Yamato.png", weaponscale, primarypos, false);
            beowulfimage = CreatePanel("VesselMayCry.Resources.Images.Beowulf.png", weaponscale, primarypos, false);
            mirageimage = CreatePanel("VesselMayCry.Resources.Images.MirageEdge.png", weaponscale, primarypos, false);
        }

        private void SetConcentration(bool val)
        {
            if (concentrationfg && concentrationbg)
            {
                if (HeroController.instance != null)
                {
                    if (HeroController.instance.playerData.bossRushMode)
                    {
                        concentrationcr2.SetActive(val);
                        concentrationbg2.SetActive(val);
                    }
                    else
                if (HeroController.instance.playerData.permadeathMode == 1)
                    {
                        concentrationcr3.SetActive(val);
                        concentrationbg3.SetActive(val);
                    }
                    else
                    {
                        concentrationbg.SetActive(val);
                        concentrationcr.SetActive(val);
                    }
                    concentrationfg.SetActive(val);
                }               
            }
            
        }

        private void SetStyle(bool val)
        {
            if (stylefg.Count != 0 && stylebg.Count != 0)
            {
                foreach (GameObject obj in stylefg)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in stylebg)
                {
                    obj.SetActive(false);
                }

                stylefg[Style.rank].SetActive(val);
                stylebg[Style.rank].SetActive(val);
            }
        }

        private void SetVT(bool val)
        {
            if (vtbg && vtfg.Count != 0)
            {
                foreach (GameObject obj in vtfg)
                {
                    obj.SetActive(false);
                }
                vtbg.SetActive(val);
                vtfghive.SetActive(val);

                if (val == true)
                {
                    vtfg[VesselTrigger.vtval].SetActive(true);
                }
            }
        }

        private void SetFill(GameObject bar, float val)
        {
            bar.GetComponent<Image>().fillAmount = val;
        }

        private void SetPrimary(GameObject image)
        {
            image.GetComponent<RectTransform>().localPosition = primarypos;
            image.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
        }

        private void SetSecondary(GameObject image)
        {
            image.GetComponent<RectTransform>().localPosition = secondarypos;
            image.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
        }

        private void HideAll()
        {
            SetStyle(false);
            SetConcentration(false);
            SetVT(false);
            if (yamatoimage && beowulfimage && mirageimage)
            {
                yamatoimage.SetActive(false);
                beowulfimage.SetActive(false);
                mirageimage.SetActive(false);
            }
            
        }

        private GameObject CreatePanel(string path, float scale, Vector3 localposition, bool filledImage)
        {
            Texture2D bgtexture = ResourceLoader.LoadTexture2D(path);
            Sprite bgsprite = Sprite.Create(bgtexture, new Rect(0, 0, bgtexture.width, bgtexture.height), new Vector2(0.5f, 0.5f));
            GameObject panel = CanvasUtil.CreateImagePanel(vmccanvas, bgsprite, new CanvasUtil.RectData(bgsprite.rect.size * scale, new Vector2(0, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));          
            if (filledImage)
            {
                Image image = panel.GetComponent<Image>();
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.preserveAspect = false;
            }
            panel.SetActive(false);
            return panel;
        }

        private void Update()
        {
            if (HeroController.instance != null)
            {
                if (concentration == null)
                {
                    SetConcentration(false);
                    concentration = HeroController.instance.GetComponent<Concentration>();
                } else
                {
                    SetConcentration(true);
                    SetFill(concentrationfg, Concentration.concentrationvalue / Concentration.concentrationmax);
                    //dude why am i doing this
                    concentrationbg.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationfg.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationcr.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationbg2.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationcr2.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationbg3.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationcr3.GetComponent<RectTransform>().localPosition = concentrationpos;
                }

                if (trigger == null)
                {
                    SetVT(false);
                    trigger = HeroController.instance.GetComponent<VesselTrigger>();
                } else
                {
                    SetVT(true);
                    vtbg.GetComponent<RectTransform>().localPosition = vtpos;

                    foreach (GameObject obj in vtfg)
                    {
                        obj.GetComponent<RectTransform>().localPosition = vtpos;
                    }

                    vtfghive.GetComponent<RectTransform>().localPosition = vtpos;
                    if (Charms.hivebloodequipped)
                    {
                        vtfghive.SetActive(true);
                        foreach (GameObject obj in vtfg)
                        {
                            obj.SetActive(false);
                        }
                    } else
                    {
                        vtfghive.SetActive(false);
                    }
                }

                if (style == null)
                {
                    SetStyle(false);
                    style = HeroController.instance.GetComponent<Style>();
                } else
                {
                    SetStyle(true);
                    SetFill(stylebg[Style.rank], Style.meter / Style.metermax);

                    foreach (GameObject obj in stylefg)
                    {
                        obj.GetComponent<RectTransform>().localPosition = stylepos;
                    }
                    foreach (GameObject obj in stylebg)
                    {
                        obj.GetComponent<RectTransform>().localPosition = stylepos;
                    }


                    if (Style.rank == 0 && Style.meter == 0)
                    {
                        SetStyle(false);
                    }
                }
                
                //dude at some point you have to ask if its even worth it
                if (yamatoimage && beowulfimage && mirageimage)
                {
                    yamatoimage.SetActive(false);
                    beowulfimage.SetActive(false);
                    mirageimage.SetActive(false);

                    if (VesselMayCry.weapon == "Yamato")
                    {
                        yamatoimage.SetActive(true);
                        beowulfimage.SetActive(true);
                        SetPrimary(yamatoimage);
                        SetSecondary(beowulfimage);
                    }
                    else if (VesselMayCry.weapon == "Beowulf")
                    {
                        beowulfimage.SetActive(true);
                        mirageimage.SetActive(true);
                        SetPrimary(beowulfimage);
                        SetSecondary(mirageimage);
                    }
                    else
                    {
                        mirageimage.SetActive(true);
                        yamatoimage.SetActive(true);
                        SetPrimary(mirageimage);
                        SetSecondary(yamatoimage);
                    }
                }

                vmccanvas.GetComponent<CanvasGroup>().alpha = 1f;
                if (HeroController.instance.cState.isPaused)
                {
                    vmccanvas.GetComponent<CanvasGroup>().alpha = 0.2f;
                }


                //this is really backwards
                if (blankerwhite == null)
                {
                    blankerwhite = GameCameras.instance.hudCamera.gameObject.Child("Blanker White").LocateMyFSM("Blanker Control");
                }
                if (tk2dblanker == null)
                {
                    tk2dblanker = GameCameras.instance.hudCamera.gameObject.Child("2dtk Blanker").LocateMyFSM("Blanker Control");
                }


                if (tk2dblanker != null && blankerwhite != null)
                {
                    if (tk2dblanker.ActiveStateName == "Fade In" || blankerwhite.ActiveStateName == "Fade In")
                    {
                        HideAll();
                    }
                }

                GameObject hudcanvas = GameCameras.instance.hudCamera.gameObject.Child("Hud Canvas");
                PlayMakerFSM hudfsm = hudcanvas.LocateMyFSM("Slide Out");
                if (hudfsm != null)
                {
                    if (hudfsm.ActiveStateName == "Out")
                    {
                        HideAll();
                    }
                }

                GameObject inven = GameCameras.instance.hudCamera.gameObject.Child("Inventory");
                PlayMakerFSM fsm = inven.LocateMyFSM("Inventory Control");
                if (fsm != null)
                {
                    if (fsm.ActiveStateName == "Opened")
                    {
                        HideAll();
                    }
                }
            }
            else
            {
                HideAll();
            }
        }

    }
}
