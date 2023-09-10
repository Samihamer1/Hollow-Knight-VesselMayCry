using Steamworks;
using UnityEngine;

namespace VesselMayCry
{
    internal class CanvasHandler : MonoBehaviour
    {
        private static CanvasHandler instance;

        private float regularscale = 0.6f;
        private float vtscale = 0.3f;
        private float stylescale = 0.15f;

        private Vector3 concentrationpos = new Vector3(-180, 250);
        private Vector3 stylepos = new Vector3(550f, -100);
        private Vector3 ranktextpos = new Vector3(0, 20);
        private Vector3 vtpos = new Vector3(-325f, 230);
        private Vector3 primarypos = new Vector3(400f, -250f);
        private Vector3 secondarypos = new Vector3(500, -250f);

        private GameObject vmccanvas;
        private GameObject concentrationbg;
        private GameObject concentrationfg;
        private GameObject stylefg;
        private GameObject ranktext;
        private GameObject vtfg;
        private GameObject vtbg;
        private GameObject beowulfimage;
        private GameObject mirageimage;
        private GameObject yamatoimage;

        private PlayMakerFSM blanker;
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
            vmccanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280f, 720f));
            vmccanvas.GetComponent<Canvas>().sortingOrder = 0;
            vmccanvas.transform.position = new Vector3(0, 0);
            UnityEngine.Object.DontDestroyOnLoad(vmccanvas);
            vmccanvas.name = "VMCCanvas";
            
            concentrationbg = CreatePanel("VesselMayCry.Resources.concentrationbg.png", regularscale, concentrationpos, false);
            concentrationfg = CreatePanel("VesselMayCry.Resources.concentrationfg.png", regularscale, concentrationpos, true);
            concentrationfg.name = "ConcentrationFG";

            stylefg = CreatePanel("VesselMayCry.Resources.stylemeter.png", stylescale, stylepos, true);
            ranktext = CanvasUtil.CreateTextPanel(stylefg, "", 15, TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(100, 100), new Vector2(550f, 280f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
            stylefg.name = "StyleFG";

            vtbg = CreatePanel("VesselMayCry.Resources.concentrationbg.png", vtscale, vtpos , false);
            vtfg = CreatePanel("VesselMayCry.Resources.vtfg.png", vtscale, vtpos, true);
            vtfg.name = "VTFG";

            yamatoimage = CreatePanel("VesselMayCry.Resources.Images.Yamato.png", vtscale, primarypos, false);
            beowulfimage = CreatePanel("VesselMayCry.Resources.Images.Beowulf.png", vtscale, primarypos, false);
            mirageimage = CreatePanel("VesselMayCry.Resources.Images.MirageEdge.png", vtscale, primarypos, false);
        }

        private void SetConcentration(bool val)
        {
            if (concentrationfg && concentrationbg)
            {
                concentrationbg.SetActive(val);
                concentrationfg.SetActive(val);
            }
            
        }

        private void SetStyle(bool val)
        {
            if (stylefg && ranktext)
            {
                stylefg.SetActive(val);
                ranktext.SetActive(val);
            }
        }

        private void SetVT(bool val)
        {
            if (vtbg && vtfg)
            {
                vtbg.SetActive(val);
                vtfg.SetActive(val);
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
                    concentrationbg.GetComponent<RectTransform>().localPosition = concentrationpos;
                    concentrationfg.GetComponent<RectTransform>().localPosition = concentrationpos;
                }

                if (trigger == null)
                {
                    SetVT(false);
                    trigger = HeroController.instance.GetComponent<VesselTrigger>();
                } else
                {
                    SetVT(true);
                    SetFill(vtfg, VesselTrigger.vtval / VesselTrigger.vtmax);
                    vtbg.GetComponent<RectTransform>().localPosition = vtpos;
                    vtfg.GetComponent<RectTransform>().localPosition = vtpos;
                }

                if (style == null)
                {
                    SetStyle(false);
                    style = HeroController.instance.GetComponent<Style>();
                } else
                {
                    SetStyle(true);
                    SetFill(stylefg, Style.meter / Style.metermax);
                    ranktext.GetComponent<Text>().text = Style.ranks[Style.rank];
                    stylefg.GetComponent<RectTransform>().localPosition = stylepos;
                    ranktext.GetComponent<RectTransform>().localPosition = ranktextpos;
                    if (Style.rank == 0 && Style.meter == 0)
                    {
                        ranktext.GetComponent<Text>().text = "";
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

            }
            else
            {
                HideAll();
            }
        }

    }
}
