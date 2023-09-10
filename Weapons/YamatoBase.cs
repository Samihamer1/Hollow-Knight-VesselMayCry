namespace VesselMayCry.Weapons
{
    internal class YamatoBase : MonoBehaviour
    {
        private bool isattacking = false;
        private bool levitateattack = false;
        private Vector3 pos;
        private bool init = false;
        public static float colorR = 1f;
        public static float colorG = 1f;
        public static float colorB = 1f;
        private float maxColorR = 1f;
        private float maxColorG = 0.5f;

        private int jcutdamage = 30;
        private int combocdamage = 15;
        private int upperslashdamage = 25;
        private int jcedamage = 200;
        private int aerialcleavedamage = 50;

        //notes
        //remmeber to add color for the mark of pride thing

        public void Awake()
        {
            CreateJCut();
            CreateComboC();
            CreateUpperSlash();
            CreateJudgementCutEnd();
            CreateAerialCleave();
            ModifyDashSlash();
            init = true;

        }
        public void OnEnable()
        {
            pos = HeroController.instance.transform.position;
            ModHooks.AttackHook += Levitate;

            CreateJCut();
            CreateComboC();
            CreateUpperSlash();
            CreateJudgementCutEnd();
            CreateAerialCleave();
            ModifyDashSlash();

            tk2dSpriteAnimator animator = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip slashclip = animator.GetClipByName("Slash");
            slashclip.CopyFrom(animator.GetClipByName("YamatoSlash"));
            slashclip.name = "Slash";
            tk2dSpriteAnimationClip slashaltclip = animator.GetClipByName("SlashAlt");
            slashaltclip.CopyFrom(animator.GetClipByName("YamatoSlashAlt"));
            slashaltclip.name = "SlashAlt";
        }

        public void OnDisable()
        {
            ModHooks.AttackHook -= Levitate;
            HeroController.instance.AffectedByGravity(true);

            FXHelper.slashsprite.color = new Color(1, 1, 1);
            FXHelper.slashaltsprite.color = new Color(1, 1, 1);
            FXHelper.wallslashsprite.color = new Color(1, 1, 1);
            FXHelper.upslashsprite.color = new Color(1, 1, 1);
            FXHelper.downslashsprite.color = new Color(1, 1, 1);
        }

        public void Update()
        {
            if (HeroController.instance.cState.jumping)
            {
                pos = HeroController.instance.transform.position;
            }

            if (HeroController.instance.GetState("attacking") && levitateattack && !HeroController.instance.cState.onGround && !HeroController.instance.cState.downAttacking && !HeroController.instance.cState.upAttacking)
            {
                FXHelper.slashsprite.color = new Color(colorR, colorG, colorB);
                FXHelper.slashaltsprite.color = new Color(colorR, colorG, colorB);
                FXHelper.wallslashsprite.color = new Color(colorR, colorG, colorB);
                FXHelper.upslashsprite.color = new Color(colorR, colorG, colorB);
                FXHelper.downslashsprite.color = new Color(colorR, colorG, colorB);
                HeroController.instance.transform.position = pos;
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.ResetHardLandingTimer();

            }
            else
            {
                if (HeroController.instance.cState.GetState("transitioning") == false)
                {
                    HeroController.instance.AffectedByGravity(true);
                }
            }

            //color set
            float concentrationpercent = Concentration.concentrationvalue / Concentration.concentrationmax;
            colorR = 1f - maxColorR * concentrationpercent;
            colorG = 1f - maxColorG * concentrationpercent;
        }

        private void Levitate(AttackDirection direction)
        {
            levitateattack = true;
            if (HeroController.instance.cState.onGround)
            {
                levitateattack = false;
            }
            pos = HeroController.instance.transform.position;
        }


        private GameObject CreateSingleJCut()
        {
            GameObject cut = GeneralHelper.CreateAttackTemplate("Judgement Cut", 1.3f, 1.3f, HeroController.instance.transform.position + new Vector3(-5 * HeroController.instance.transform.GetScaleX(), 0), false);
            SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

            //tempdamage
            ContactDamage damage = cut.AddComponent<ContactDamage>();
            damage.SetDamage(jcutdamage);
            cut.SetActive(true);

            StartCoroutine(Anims.PlayAnimation("JudgementCut", render, 0.4f));
            StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.4f));

            return cut;
        }

        private void CreateJCut()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.          
                FsmState newstate = GeneralHelper.CreateTemplateState("Judgement Cut", nailartfsm, "JC");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    GameObject cut = CreateSingleJCut();

                    //find a target....
                    HealthManager[] obj = FindObjectsOfType<HealthManager>();
                    GameObject target = null;
                    float distance = 70f;
                    for (int j = 0; j < obj.Length; j++)
                    {
                        if (!VesselTrigger.inVesselTrigger)
                        {
                            float mag = (obj[j].gameObject.transform.position - cut.transform.position).magnitude;
                            if (mag < distance)
                            {
                                distance = mag;
                                target = obj[j].gameObject;
                            }
                        }
                        else
                        {
                            float mag = (obj[j].gameObject.transform.position - cut.transform.position).magnitude;
                            if (mag < distance)
                            {
                                GameObject newcut = CreateSingleJCut();
                                newcut.transform.position = obj[j].gameObject.transform.position;
                            }
                        }

                    }

                    if (target != null && !VesselTrigger.inVesselTrigger)
                    {
                        Vector3 vector3 = new Vector3(target.transform.position.x, target.transform.position.y, HeroController.instance.transform.position.z);
                        cut.transform.position = vector3;
                    }

                    //audio
                    FXHelper.PlayAudio("JC", 0.65f);

                    //anim test
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    animator.Stop();
                    animator.Play("JC");

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has G Slash?", "FINISHED", "Judgement Cut");
        }

        private void ModifyDashSlash()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                nailartfsm.GetState("DSlash Start").AddMethod(() =>
                {
                    HeroController.instance.gameObject.Child("Attacks").Child("Dash Slash").GetComponent<tk2dSprite>().color = new Color(colorR, colorB, colorG);
                });
            }
            else
            {
                nailartfsm.ChangeTransition("Has Dash?", "FINISHED", "Dash Slash Ready");
            }

        }

        private void CreateUpperSlash()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Upper Slash", nailartfsm, "UpperSlash");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    GameObject cut = GeneralHelper.CreateAttackTemplate("Upper Slash", 3f, 3f, HeroController.instance.transform.position + new Vector3(-1 * HeroController.instance.transform.GetScaleX(), 0), true);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(upperslashdamage);
                    damage.SetKnockback(1.4f);

                    cut.SetActive(true);

                    StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.15f));

                    FXHelper.PlayAudio("GreatSlash", 0.5f);
                    FXHelper.ActivateEffect("SharpFlash");

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Cyclone?", "FINISHED", "Upper Slash");
        }

        private void CreateJudgementCutEnd()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                FsmState combostate = spellcontrol.CreateState("Judgement Cut End");
                combostate.RemoveTransition("FINISHED");
                combostate.AddMethod(() =>
                {
                    if (Concentration.concentrationvalue >= Concentration.concentrationlevel2 && (VesselTrigger.inVesselTrigger || VesselTrigger.vtval == VesselTrigger.vtmax))
                    {

                        VesselTrigger.Reset();

                        HeroController.instance.RelinquishControl();

                        float timescale = 0.001f;

                        //canvas
                        GameObject jcecanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280f, 720f));
                        jcecanvas.GetComponent<Canvas>().sortingOrder = 0;

                        //t t t t tiiiime scale (i cant make it 0, so im doing this instead. will it cause problems? dunno.)
                        Time.timeScale = timescale;

                        //bg
                        StartCoroutine(JCECorou(jcecanvas, 4 * timescale, timescale));

                        //immunity
                        HeroController.instance.QuakeInvuln();


                        //thats a lotta coroutines
                        // StartCoroutine(Anims.PlayAnimation("JudgementCutEnd", render, 5f));
                        StartCoroutine(GeneralHelper.DestroyAfter(jcecanvas, 4 * timescale));



                    }
                    else
                    {
                        HeroController.instance.spellControl.SendEvent("NEXT");
                    }
                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Scream?", "CAST", "Judgement Cut End");

        }

        private IEnumerator JCEAnimate(tk2dSpriteAnimator animator, float timescale)
        {
            float timetowait = 4f * timescale - animator.GetClipByName("JCEFinish").Duration;
            yield return new WaitForSeconds(timetowait);
            animator.Play("JCEFinish");
        }

        private IEnumerator JCECorou(GameObject canvas, float time, float scale)
        {
            //dont get rid of this random ass wait. for some god forsaken reason the knight animation plays before the timescale is set even though it is a SEQUENCE OF EVENTS
            //and i got so annoyed i just added a random wait and it fixed it, so yeah
            yield return new Wait();

            //anim
            tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            HeroController.instance.StopAnimationControl();
            animator.Play("JCEDash");

            //if its possible to overuse coroutines, ive done it
            StartCoroutine(JCEAnimate(animator, scale));

            //also audio
            FXHelper.PlayAudio("JCEStart", 1f);

            Texture2D bgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.YamatoAnims.JudgementCutEnd.1.png");
            Sprite bgsprite = Sprite.Create(bgtexture, new Rect(0, 0, bgtexture.width, bgtexture.height), new Vector2(0f, 0f));
            GameObject jceimage = CanvasUtil.CreateImagePanel(canvas, bgsprite, new CanvasUtil.RectData(bgsprite.rect.size * 2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)));

            int count = 45;
            for (int i = 1; i < count; i++)
            {
                if (canvas != null)
                {
                    bgtexture = ResourceLoader.LoadTexture2D("VesselMayCry.Resources.YamatoAnims.JudgementCutEnd." + i + ".png");
                    bgsprite = Sprite.Create(bgtexture, new Rect(0, 0, bgtexture.width, bgtexture.height), new Vector2(0f, 0f));
                    Destroy(jceimage);
                    jceimage = CanvasUtil.CreateImagePanel(canvas, bgsprite, new CanvasUtil.RectData(bgsprite.rect.size * 2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)));
                    yield return new WaitForSeconds(time / count);
                }
            }

            Time.timeScale = 1f;

            FXHelper.PlayAudio("JCEEnd", 1f);
            FXHelper.ActivateEffect("SharpFlash");
            FXHelper.ActivateEffect("WhiteBurst");

            //shake
            FXHelper.CameraShake(2);

            //also damage even if under animate name heehee

            HealthManager[] obj = FindObjectsOfType<HealthManager>();
            float distance = 45f;
            for (int j = 0; j < obj.Length; j++)
            {

                float mag = (obj[j].gameObject.transform.position - HeroController.instance.transform.position).magnitude;
                if (mag < distance)
                {
                    HitInstance hitInstance = new HitInstance();
                    hitInstance.AttackType = AttackTypes.Nail;
                    hitInstance.IgnoreInvulnerable = true;
                    hitInstance.Multiplier = 1;
                    hitInstance.MagnitudeMultiplier = 0;
                    hitInstance.MoveDirection = true;
                    hitInstance.CircleDirection = false;
                    hitInstance.Source = gameObject;

                    hitInstance.DamageDealt = jcedamage;
                    HitTaker.Hit(obj[j].gameObject, hitInstance);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        private void CreateAerialCleave()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                FsmState anticstate = spellcontrol.CreateState("Cleave Antic");
                anticstate.RemoveTransition("FINISHED");
                anticstate.AddMethod(() =>
                {
                    HeroController.instance.RelinquishControl();
                    HeroController.instance.StopAnimationControl();

                });

                FsmState fallstate = spellcontrol.CreateState("Cleave Fall");
                fallstate.RemoveTransition("FINISHED");

                FsmOwnerDefault knightowner = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).gameObject;


                //Antic Events
                Tk2dPlayAnimationWithEvents eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "AerialCleaveAntic";
                eventaction.animationCompleteEvent = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).animationCompleteEvent;


                SetVelocity2d velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 5;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;


                anticstate.AddAction(eventaction);
                anticstate.AddAction(velocityaction);

                //Fall Events
                eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "AerialCleaveFall";

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = -80;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;


                GetVelocity2d getvelocityaction = new GetVelocity2d();
                getvelocityaction.gameObject = knightowner;
                getvelocityaction.vector = new Vector2(0, 0);
                getvelocityaction.x = 0;
                getvelocityaction.y = spellcontrol.FsmVariables.GetFsmFloat("Y Speed");
                getvelocityaction.space = 0;
                getvelocityaction.everyFrame = true;

                FloatCompare compareaction = new FloatCompare();
                compareaction.everyFrame = true;
                compareaction.float1 = spellcontrol.FsmVariables.GetFsmFloat("Y Speed");
                compareaction.float2 = 0f;
                compareaction.tolerance = 0.2f;
                compareaction.equal = new FsmEvent("HERO LANDED");

                fallstate.AddAction(eventaction);
                fallstate.AddAction(velocityaction);
                fallstate.AddAction(getvelocityaction);
                fallstate.AddAction(compareaction);


                FsmState landstate = spellcontrol.CreateState("Cleave Land");
                landstate.RemoveTransition("FINISHED");
                landstate.AddMethod(() =>
                {
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    animator.Stop();
                    animator.Play("AerialCleaveSlash");

                    GameObject cut = GeneralHelper.CreateAttackTemplate("Aerial Cleave", 3f, -3f, HeroController.instance.transform.position + new Vector3(-1 * HeroController.instance.transform.GetScaleX(), 0), true);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();
                    cut.AddComponent<QuakeBreak>();

                    //tempdamage
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(aerialcleavedamage);
                    damage.SetKnockback(1.4f);
                    cut.SetActive(true);

                    FXHelper.CameraShake(0);
                    FXHelper.PlayAudio("GreatSlash", 0.5f);
                    FXHelper.ActivateEffect("SharpFlash");


                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.15f));
                    StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                });


                anticstate.AddTransition("ANIM END", "Cleave Fall");
                fallstate.AddTransition("HERO LANDED", "Cleave Land");
                landstate.AddTransition("NEXT", "Spell End");

            }
            spellcontrol.ChangeTransition("Has Quake?", "CAST", "Cleave Antic");
        }

        private void CreateComboC()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                FsmState combostate = GeneralHelper.CreateTemplateState("Combo C", spellcontrol, "ComboC");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    GameObject slices = GeneralHelper.CreateAttackTemplate("ComboC", 4f, 2.4f, HeroController.instance.transform.position, true);
                    SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = slices.AddComponent<ContactDamage>();
                    damage.SetDamage(combocdamage);
                    slices.SetActive(true);

                    //thats a lotta coroutines
                    StartCoroutine(Anims.PlayAnimation("ComboC", render, 0.4f));
                    StartCoroutine(ComboCAudio());
                    StartCoroutine(GeneralHelper.DestroyAfter(slices, 0.4f));
                    StartCoroutine(ComboCHitboxCycle(slices));

                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Fireball?", "CAST", "Combo C");

        }

        private IEnumerator ComboCAudio()
        {
            for (int i = 0; i < 5; i++)
            {
                HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<AudioSource>().Play();
                yield return new WaitForSeconds(0.08f);
            }
        }

        private IEnumerator ComboCHitboxCycle(GameObject slices)
        {
            ContactDamage damage = slices.GetComponent<ContactDamage>();
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.12f);
                slices.name = "ComboC" + i.ToString();
                if (damage != null)
                {
                    damage.HitAgain();
                }
            }
        }
    }
}
