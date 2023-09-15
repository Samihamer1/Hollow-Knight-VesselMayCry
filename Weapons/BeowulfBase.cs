using FrogCore.Ext;
using VesselMayCry.Helpers;

namespace VesselMayCry.Weapons
{
    internal class BeowulfBase : MonoBehaviour
    {
        private bool charged = false;
        private float chargetimer;
        private float chargetime = 1.2f;
        private bool init = false;

        public static bool beowulfcharging = false;

        private int strongpunchdmg = 30;
        private int[] lunarphasedmg = {8, 15};
        private int uppercutdmg = 25;
        private int dashpunchdmg = 20;
        private int[] starfalldmg = {15, 30};
        private int[] hellonearthdmg = {200, 300};
        private float level2multiplier = 2.5f;

        public void Awake()
        {
            CreateStrongPunch();
            CreateLunarPhase();
            CreateUppercut();
            CreateStarfall();
            CreateDashPunch();
            CreateHellOnEarth();
            init = true;

        }
        public void OnEnable()
        {
            CreateStrongPunch();
            CreateLunarPhase();
            CreateUppercut();
            CreateStarfall();
            CreateDashPunch();
            CreateHellOnEarth();


            tk2dSpriteAnimator animator = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip slashclip = animator.GetClipByName("Slash");
            slashclip.CopyFrom(animator.GetClipByName("BeowulfSlash"));
            slashclip.name = "Slash";
            tk2dSpriteAnimationClip slashaltclip = animator.GetClipByName("SlashAlt");
            slashaltclip.CopyFrom(animator.GetClipByName("BeowulfSlashAlt"));
            slashaltclip.name = "SlashAlt";
        }

        public void OnDisable()
        {
            HeroController.instance.AffectedByGravity(true);
            FXHelper.nachargeeffect.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.791f);
        }

        public void Update()
        {
            if (Charms.quickfistsequipped)
            {
                chargetime = 0.6f;
            } else
            {
                chargetime = 1.2f;
            }
            if (FXHelper.nachargeeffect.activeSelf)
            {
                beowulfcharging = true;
                chargetimer += Time.deltaTime;
                if (chargetimer > chargetime)
                {
                    FXHelper.nachargeeffect.GetComponent<tk2dSprite>().color = new Color(1, 0.7f, 1, 0.791f);
                    if (!charged)
                    {
                        charged = true;
                        FXHelper.ActivateEffect("PinkBurst");
                        FXHelper.PlayAudio("BeowulfSecondCharge", 0.75f);
                    }
                } else
                {
                    charged = false;
                    FXHelper.nachargeeffect.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.791f);
                }
            } else
            {
                beowulfcharging = false;
                charged = false;
                chargetimer = 0;
            }
        }

        

        private void CreateHellOnEarth()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {


                FsmState combostate = spellcontrol.CreateState("Hell On Earth Check");
                combostate.RemoveTransition("FINISHED");
                combostate.AddMethod(() =>
                {
                    if ((Concentration.concentrationvalue >= Concentration.concentrationlevel2) && (VesselTrigger.inVesselTrigger || VesselTrigger.vtval == VesselTrigger.vtmax) && HeroController.instance.cState.onGround)
                    {
                        HeroController.instance.RelinquishControl();
                        HeroController.instance.StopAnimationControl();
                        HeroController.instance.spellControl.SendEvent("NEXT");
                    }
                    else
                    {
                        if (Charms.stillvoidequipped)
                        {
                            HeroController.instance.spellControl.SendEvent("SCREAM");
                        }
                        HeroController.instance.spellControl.SendEvent("CANCEL");
                    }
                });

                FsmState anticstate = spellcontrol.CreateState("Hell On Earth Antic");
                anticstate.RemoveTransition("FINISHED");
                anticstate.AddMethod(() =>
                {
                    //immunity
                    HeroController.instance.QuakeInvuln();
                });
                FsmOwnerDefault knightowner = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).gameObject;
                //Antic Events
                Tk2dPlayAnimationWithEvents eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "HellOnEarthAntic";
                eventaction.animationCompleteEvent = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).animationCompleteEvent;


                SetVelocity2d velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                anticstate.AddAction(eventaction);
                anticstate.AddAction(velocityaction);

                FsmState punchstate = spellcontrol.CreateState("Hell On Earth Punch");
                punchstate.AddMethod(() =>
                {
                    FXHelper.ActivateEffect("SharpFlashS");
                    FXHelper.PlayAudio("HellOnEarthSpark", 1f);
                });

                //Antic Events
                eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "HellOnEarthPunch";
                eventaction.animationCompleteEvent = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).animationCompleteEvent;


                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                punchstate.AddAction(eventaction);
                punchstate.AddAction(velocityaction);

                FsmState shockwavestate = spellcontrol.CreateState("Hell On Earth");
                shockwavestate.RemoveTransition("FINISHED");
                shockwavestate.AddMethod(() =>
                {
                    //swap it
                    VesselTrigger.Reset();
                    Concentration.concentrationvalue = 0;


                    FXHelper.ActivateEffect("HellOnEarthBurst");
                    FXHelper.ActivateEffect("HellOnEarthBurst2");
                    FXHelper.ActivateEffect("PinkBurst");
                    FXHelper.ActivateEffect("SoulBurst");
                    FXHelper.CameraShake(2);
                    FXHelper.ActivateEffect("Q Slam");
                    FXHelper.PlayAudio("HellOnEarthBurst", 1f);

                    GameObject damage = GeneralHelper.CreateAttackTemplate("Hell On Earth", 1f, 1f, HeroController.instance.transform.position, new Vector2(18.5f,10), new Vector2(0,0));
                    ContactDamage dmg = damage.AddComponent<ContactDamage>();
                    int damagevalue = hellonearthdmg[0];
                    if (HeroController.instance.playerData.screamLevel > 1)
                    {
                        damagevalue = hellonearthdmg[1];
                    }

                    dmg.SetDamage(damagevalue);
                    StartCoroutine(GeneralHelper.DestroyAfter(damage,0.1f));
                });

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                shockwavestate.AddAction(velocityaction);

                anticstate.AddTransition("ANIM END", "Hell On Earth Punch");
                punchstate.AddTransition("ANIM END", "Hell On Earth");
                combostate.AddTransition("NEXT", "Hell On Earth Antic");
                combostate.AddTransition("CANCEL", "Spell End");
                combostate.AddTransition("SCREAM", "Level Check 3");
                shockwavestate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Scream?", "CAST", "Hell On Earth Check");

        }


        private void CreateStrongPunch()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init) {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Strong Punch", nailartfsm, "StrongPunch");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    string name = "Strong Punch";
                    float multiplier = 1f;

                    if (charged)
                    {
                        multiplier = 1.6f;
                        FXHelper.ActivateEffect("SharpFlashM");
                        FXHelper.CameraShake(1);
                        name = "Strong PunchC";
                    } else
                    {
                        FXHelper.ActivateEffect("SharpFlashS");
                        FXHelper.CameraShake(0);
                    }

                    Vector3 pos = HeroController.instance.transform.position + new Vector3(-0.6f * HeroController.instance.transform.GetScaleX(), -0.3f, -0.004f);
                    Vector2[] points = new Vector2[5];
                    points[0] = new Vector2(0, 1);
                    points[1] = new Vector2(-0.9511f, 0.309f);
                    points[2] = new Vector2(-0.5878f, -0.809f);
                    points[3] = new Vector2(1.6511f, -0.809f);
                    points[4] = new Vector2(1.6511f, 0.309f);

                    GameObject cut = GeneralHelper.CreateAttackTemplate(name, -2f * multiplier, 2f * multiplier, pos, points);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

                    //tempdamage
                    float damagevalue = strongpunchdmg * level2multiplier;
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(((int)damagevalue));
                    damage.SetKnockback(1.4f * multiplier);

                    cut.SetActive(true);

                    StartCoroutine(Anims.PlayAnimation("Shockwave1", render, 0.15f));
                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.15f));

                    FXHelper.PlayAudioRandomPitch("Shockwave", 0.2f);

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has G Slash?", "FINISHED", "Strong Punch");
        }

        private void CreateDashPunch()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Dash Punch", nailartfsm, "DashPunch");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    string name = "Dash Punch";
                    float multiplier = 1f;
                    if (charged)
                    {
                        multiplier = 1.6f;
                        FXHelper.ActivateEffect("SharpFlashM");
                        FXHelper.CameraShake(1);
                        name = "Dash PunchC";
                    }
                    else
                    {
                        FXHelper.ActivateEffect("SharpFlashS");
                        FXHelper.CameraShake(0);
                    }

                    Vector3 pos = HeroController.instance.transform.position + new Vector3(-0.6f * HeroController.instance.transform.GetScaleX(), -0.3f, -0.004f);
                    Vector2[] points = new Vector2[5];
                    points[0] = new Vector2(0, 1);
                    points[1] = new Vector2(-0.9511f, 0.309f);
                    points[2] = new Vector2(-0.5878f, -0.809f);
                    points[3] = new Vector2(1.6511f, -0.809f);
                    points[4] = new Vector2(1.6511f, 0.309f);

                    GameObject cut = GeneralHelper.CreateAttackTemplate(name, -2.8f * multiplier, 1.4f * multiplier, pos, points);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

                    //tempdamage
                    float damagevalue = dashpunchdmg * level2multiplier;
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(((int)damagevalue));
                    damage.SetKnockback(1.4f * multiplier);

                    cut.SetActive(true);

                    StartCoroutine(Anims.PlayAnimation("Shockwave1", render, 0.2f));
                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.2f));

                    FXHelper.PlayAudioRandomPitch("Shockwave", 0.2f);

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Dash?", "FINISHED", "Dash Punch");
        }

        private void CreateStarfall()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                List<FsmFloat> flt = spellcontrol.FsmVariables.FloatVariables.ToList();
                flt.Add(new FsmFloat("X Speed"));
                spellcontrol.FsmVariables.FloatVariables = flt.ToArray();

                FsmState anticstate = spellcontrol.CreateState("StarfallAntic");
                anticstate.RemoveTransition("FINISHED");
                anticstate.AddMethod(() =>
                {
                    if (!HeroController.instance.cState.onGround)
                    {
                        HeroController.instance.RelinquishControl();
                        HeroController.instance.StopAnimationControl();
                    } else
                    {
                        spellcontrol.SendEvent("CANCEL");
                    }
                });

                FsmOwnerDefault knightowner = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).gameObject;

                //Antic Events
                Tk2dPlayAnimationWithEvents eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "StarfallAntic";
                eventaction.animationCompleteEvent = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).animationCompleteEvent;


                SetVelocity2d velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;


                anticstate.AddAction(eventaction);
                anticstate.AddAction(velocityaction);

                FsmState combostate = spellcontrol.CreateState("Starfall");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    HeroController.instance.RelinquishControl();

                    GameObject slices = GeneralHelper.CreateAttackTemplate("Starfall", 2f, 2f, HeroController.instance.transform, new Vector3(0, -1f, 0));
                    slices.GetComponent<Rigidbody2D>().Destroy();
                    SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = slices.AddComponent<ContactDamage>();
                    int damagevalue = starfalldmg[0];
                    if (HeroController.instance.playerData.quakeLevel > 1)
                    {
                        damagevalue = starfalldmg[1];
                    }

                    damage.SetDamage(damagevalue);
                    damage.SetPogo();
                    slices.SetActive(true);

                    //fx
                    FXHelper.ActivateEffect("SharpFlashS");

                    combostate.GetAction<SetVelocity2d>().x = -15 * HeroController.instance.transform.GetScaleX();

                    HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<UnityEngine.AudioSource>().Play();

                    StartCoroutine(DestroyStarfallHitbox(slices));

                    //anim
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    HeroController.instance.StopAnimationControl();
                    animator.Play("StarfallFall");
                });

                FsmState poststate = spellcontrol.CreateState("StarfallVelo");
                poststate.RemoveTransition("FINISHED");
                poststate.AddMethod(() =>
                {
                    StartCoroutine(Pogo());
                });

                
                FsmEvent herolandedevent = spellcontrol.GetState("Quake2 Down").GetAction<CheckCollisionSide>(8).bottomHitEvent;

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 20;
                velocityaction.y = -20;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                GetVelocity2d getvelocityaction = new GetVelocity2d();
                getvelocityaction.gameObject = knightowner;
                getvelocityaction.vector = new Vector2(0, 0);
                getvelocityaction.x = spellcontrol.FsmVariables.GetFsmFloat("X Speed");
                getvelocityaction.y = spellcontrol.FsmVariables.GetFsmFloat("Y Speed");
                getvelocityaction.space = 0;
                getvelocityaction.everyFrame = true;

                FloatCompare compareaction = new FloatCompare();
                compareaction.everyFrame = true;
                compareaction.float1 = spellcontrol.FsmVariables.GetFsmFloat("Y Speed");
                compareaction.float2 = 0f;
                compareaction.tolerance = 0.2f;
                compareaction.equal = herolandedevent;

                FloatCompare compareactionx = new FloatCompare();
                compareaction.everyFrame = true;
                compareaction.float1 = spellcontrol.FsmVariables.GetFsmFloat("X Speed");
                compareaction.float2 = 0f;
                compareaction.tolerance = 0.2f;
                compareaction.equal = herolandedevent;

                combostate.AddAction(velocityaction);
                combostate.AddAction(getvelocityaction);
                combostate.AddAction(compareaction);
                combostate.AddAction(compareactionx);

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 40;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = false;

                Wait waitaction = new Wait();
                waitaction.time = 0f;
                waitaction.realTime = false;
                waitaction.finishEvent = new FsmEvent("FINISHED");

                //poststate.AddAction(velocityaction);
                poststate.AddAction(waitaction);

                anticstate.AddTransition("CANCEL", "Spell End");
                anticstate.AddTransition("ANIM END", "Starfall");
                poststate.AddTransition("FINISHED", "Spell End");
                combostate.AddTransition("HERO LANDED", "Spell End");
                combostate.AddTransition("VELO", "StarfallVelo");
            }
            spellcontrol.ChangeTransition("Has Quake?", "CAST", "StarfallAntic");
        }

        private IEnumerator Pogo()
        {
            while (HeroController.instance.spellControl.ActiveStateName != "Inactive")
            {
                yield return new Wait();
            }
            HeroController.instance.BounceHigh();
        }

        public void EndStarfall()
        {
            HeroController.instance.spellControl.SendEvent("VELO");
        }

        private IEnumerator DestroyStarfallHitbox(GameObject slices)
        {
            while (HeroController.instance.spellControl.ActiveStateName == "Starfall")
            {
                yield return new Wait();
            }
            slices.Destroy();
        }

        private void CreateUppercut()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Uppercut", nailartfsm, "BeastUppercut");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    GameObject cut;
                    HeroController.instance.AffectedByGravity(true);

                    Vector3 pos = HeroController.instance.transform.position + new Vector3(-0.6f * HeroController.instance.transform.GetScaleX(), -1f, -0.004f);
                    Vector2[] points = new Vector2[5];
                    points[0] = new Vector2(0, 1);
                    points[1] = new Vector2(-0.9511f, 0.309f);
                    points[2] = new Vector2(-0.5878f, -0.809f);
                    points[3] = new Vector2(1.6511f, -0.809f);
                    points[4] = new Vector2(1.6511f, 0.309f);

                    float multiplier = 1f;
                    if (charged)
                    {
                        multiplier = 1.6f;
                        FXHelper.ActivateEffect("SharpFlashM");
                        FXHelper.CameraShake(1);
                        newstate.GetAction<SetVelocity2d>().y = 40;
                        cut = GeneralHelper.CreateAttackTemplate("UppercutC", -2.3f * multiplier, 1.4f * multiplier, HeroController.instance.transform, new Vector3(-0.6f, -3f, -0.004f));
                        cut.GetComponent<Rigidbody2D>().Destroy();
                    }
                    else
                    {
                        FXHelper.ActivateEffect("SharpFlashS");
                        FXHelper.CameraShake(0);
                        newstate.GetAction<SetVelocity2d>().y = 0;
                        cut = GeneralHelper.CreateAttackTemplate("Uppercut", -2.3f * multiplier, 1.4f * multiplier, pos, points);
                    }


                    float rotation = 80;
                    if (HeroController.instance.transform.GetScaleX() == 1)
                    {
                        rotation = 290;
                    }
                    cut.transform.SetRotation2D(rotation);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

                    //tempdamage
                    float damagevalue = uppercutdmg * level2multiplier;
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(((int)damagevalue));
                    damage.SetKnockback(1.4f * multiplier);

                    cut.SetActive(true);

                    if (charged)
                    {
                        StartCoroutine(UppercutSecondHit(damage));
                    }
                    StartCoroutine(Anims.PlayAnimation("Shockwave1", render, 0.15f));
                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.15f));

                    FXHelper.PlayAudioRandomPitch("Shockwave2", 1.3f);

                });            

                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Cyclone?", "FINISHED", "Uppercut");
        }

        private IEnumerator UppercutSecondHit(ContactDamage damage)
        {
            yield return new WaitForSeconds(0.12f);
            damage.HitAgain();
        }

        private void CreateLunarPhase()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            { 
                FsmState combostate = GeneralHelper.CreateTemplateState("Lunar Phase", spellcontrol, "LunarPhase");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    GameObject slices = GeneralHelper.CreateAttackTemplate("Lunar Phase", 2.5f,2.5f, HeroController.instance.transform.position, new Vector2(2.5f,2.5f), new Vector2(0,0));
                    SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = slices.AddComponent<ContactDamage>();
                    int damagevalue = lunarphasedmg[0];
                    if (HeroController.instance.playerData.fireballLevel > 1)
                    {
                        damagevalue = lunarphasedmg[1];
                    }

                    damage.SetDamage(damagevalue);
                    slices.SetActive(true);

                    //fx
                    FXHelper.ActivateEffect("SharpFlashS");

                    //thats a lotta coroutines
                    StartCoroutine(Anims.PlayAnimation("LunarPhase", render, 0.25f));
                    StartCoroutine(LunarPhaseAudio());
                    StartCoroutine(GeneralHelper.DestroyAfter(slices, 0.25f));
                    StartCoroutine(LunarPhaseHitboxCycle(slices));
                    StartCoroutine(LunarPhaseLoop(slices, render));

                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Fireball?", "CAST", "Lunar Phase");

        }

        private IEnumerator LunarPhaseAudio()
        {
            for (int i = 0; i < 3; i++)
            {
                HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<UnityEngine.AudioSource>().Play();
                yield return new WaitForSeconds(0.08f);
            }
        }

        private IEnumerator LunarPhaseHitboxCycle(GameObject slices)
        {
            ContactDamage damage = slices.GetComponent<ContactDamage>();
            for (int i = 0; i < 1; i++)
            {
                yield return new WaitForSeconds(0.12f);
                slices.name = "LunarPhase" + i.ToString();
                if (damage != null)
                {
                    damage.HitAgain();
                }
            }
        }

        private IEnumerator LunarPhaseLoop(GameObject slices, SpriteRenderer render)
        {
            float rotation = 0;
            while (slices)
            {
                slices.transform.SetRotationZ(rotation);
                rotation += 18;
                if (rotation == 361)
                {
                    rotation = 0;
                }
                yield return new Wait();
            }
        }
    }
}
