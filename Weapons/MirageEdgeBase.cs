using FrogCore.Ext;
using VesselMayCry.Helpers;

namespace VesselMayCry.Weapons
{
    internal class MirageEdgeBase : MonoBehaviour
    {
        
        private bool init = false;
        private bool spiralactive = false;
        private bool blisteringactive = false;
        private bool roundtripactive = false;
        private bool roundtripmoving = false;
        private bool overdriveactive = false;

        private int[] spiraldmg = { 5, 8 };
        private int[] blisteringdmg = { 7, 10 };
        private int roundtripdmg = 25;
        private int overdrivedmg = 12;
        private int drivedmg = 12;
        private int[] deepstingerdmg = {8,15};
        private int finalhitdmg = 30;

        public void Awake()
        {
            CreateSpiralSwords();
            CreateBlisteringSwords();
            CreateRoundTrip();
            CreateDrive();
            CreateOverdrive();
            CreateDeepStinger();

            
            init = true;

        }
        public void OnEnable()
        {
            CreateSpiralSwords();
            CreateBlisteringSwords();
            CreateRoundTrip();
            CreateDrive();
            CreateOverdrive();
            CreateDeepStinger();

            On.HeroController.DoAttack += RoundTripActive;

            tk2dSpriteAnimator animator = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip slashclip = animator.GetClipByName("Slash");
            slashclip.CopyFrom(animator.GetClipByName("MirageSlash"));
            slashclip.name = "Slash";
            tk2dSpriteAnimationClip slashaltclip = animator.GetClipByName("SlashAlt");
            slashaltclip.CopyFrom(animator.GetClipByName("MirageSlashAlt"));
            slashaltclip.name = "SlashAlt";
        }

        private void RoundTripActive(On.HeroController.orig_DoAttack orig, HeroController self)
        {
            if (roundtripactive && VesselMayCry.weapon == "Mirage Edge")
            {
                return;
            }
            orig.Invoke(self);
        }

        public void OnDisable()
        {
            HeroController.instance.AffectedByGravity(true);
            On.HeroController.DoAttack -= RoundTripActive;
        }

        private void CreateDeepStinger()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {

                FsmState combostate = spellcontrol.CreateState("Deep Stinger Check");
                combostate.RemoveTransition("FINISHED");
                combostate.AddMethod(() =>
                {
                    if ((Concentration.concentrationvalue >= Concentration.concentrationlevel2) && (VesselTrigger.inVesselTrigger || VesselTrigger.vtval == VesselTrigger.vtmax))
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

                FsmState anticstate = spellcontrol.CreateState("Deep Stinger Antic");
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
                eventaction.clipName = "NA Cyclone Start";
                eventaction.animationCompleteEvent = spellcontrol.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).animationCompleteEvent;


                SetVelocity2d velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 0;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                anticstate.AddAction(eventaction);
                anticstate.AddAction(velocityaction);

                FsmState spinstate = spellcontrol.CreateState("Deep Stinger");
                spinstate.AddMethod(() =>
                {
                    FXHelper.ActivateEffect("SharpFlashS");
                    FXHelper.CameraShake(2);
                    //FXHelper.PlayAudio("HellOnEarthSpark", 1f);

                    GameObject damage = GeneralHelper.CreateAttackTemplate("Deep Stinger", 1f, 1f, HeroController.instance.transform, new Vector3(0, 0), new Vector2(5, 5), new Vector2(0, 0));
                    DamageEnemies dmg = damage.AddComponent<DamageEnemies>();
                    dmg.attackType = AttackTypes.Nail;
                    int damagevalue = deepstingerdmg[0];
                    if (HeroController.instance.playerData.screamLevel > 1)
                    {
                        damagevalue = deepstingerdmg[1];
                    }

                    dmg.damageDealt = damagevalue;
                    dmg.magnitudeMult = 0;
                    dmg.ignoreInvuln = false;

                    damage.GetComponent<Rigidbody2D>().Destroy();

                    FXHelper.PlayAudio("DeepStinger", 0.75f);

                    StartCoroutine(StingerCorou(damage));

                    if (HeroController.instance.transform.GetScaleX() == 1)
                    {
                        HeroController.instance.spellControl.SendEvent("LEFT");
                    }
                    HeroController.instance.spellControl.SendEvent("RIGHT");
                });

                //Antic Events
                eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = "DeepStinger";

                spinstate.AddAction(eventaction);

                FsmState leftstate = spellcontrol.CreateState("Stinger Left");

                leftstate.AddMethod(HeroController.instance.FaceLeft);
                
                ListenForRight listenaction2 = new ListenForRight();
                listenaction2.eventTarget = spellcontrol.GetAction<ListenForUp>("QC", 2).eventTarget;
                listenaction2.isPressed = spellcontrol.GetAction<ListenForRight>("Focus S", 13).isPressed;

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = -10;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                leftstate.AddAction(listenaction2);
                leftstate.AddAction(eventaction);
                leftstate.AddAction(velocityaction);

                FsmState rightstate = spellcontrol.CreateState("Stinger Right");
                rightstate.AddMethod(HeroController.instance.FaceRight);

                ListenForLeft listenaction = new ListenForLeft();
                listenaction.eventTarget = spellcontrol.GetAction<ListenForUp>("QC", 2).eventTarget;
                listenaction.isPressed = spellcontrol.GetAction<ListenForLeft>("Focus S", 14).isPressed;

                velocityaction = new SetVelocity2d();
                velocityaction.gameObject = knightowner;
                velocityaction.x = 10;
                velocityaction.y = 0;
                velocityaction.vector = new Vector2(0, 0);
                velocityaction.everyFrame = true;

                rightstate.AddAction(eventaction);
                rightstate.AddAction(velocityaction);
                rightstate.AddAction(listenaction);

                FsmState finalstrike = spellcontrol.CreateState("Stinger Strike");
                //yeah thats right. i copied aerial cleave for hte final hit. what are you gonna do about it?
                finalstrike.AddMethod(() =>
                {
                    GameObject cut = GeneralHelper.CreateAttackTemplate("Stinger Cleave", 3f, -3f, HeroController.instance.transform.position + new Vector3(-3 * HeroController.instance.transform.GetScaleX(), 0), true);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();
                    render.color = new Color(0, 0.55f, 1, 1);

                    //tempdamage
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    damage.SetDamage(finalhitdmg);
                    damage.SetKnockback(1.4f);
                    cut.SetActive(true);

                    FXHelper.CameraShake(0);
                    FXHelper.PlayAudio("GreatSlash", 0.5f);
                    FXHelper.ActivateEffect("SharpFlash");


                    StartCoroutine(GeneralHelper.DestroyAfter(cut, 0.15f));
                    StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                });

                leftstate.AddTransition("RIGHT", "Stinger Right");
                rightstate.AddTransition("LEFT", "Stinger Left");
                leftstate.AddTransition("NEXT", "Stinger Strike");
                rightstate.AddTransition("NEXT", "Stinger Strike");
                finalstrike.AddTransition("NEXT", "Spell End");
                anticstate.AddTransition("ANIM END", "Deep Stinger");
                combostate.AddTransition("NEXT", "Deep Stinger Antic");
                combostate.AddTransition("SCREAM", "Scream Get?");
                combostate.AddTransition("CANCEL", "Spell End");
                spinstate.AddTransition("LEFT", "Stinger Left");
                spinstate.AddTransition("RIGHT", "Stinger Right");
            }
            spellcontrol.ChangeTransition("Has Scream?", "CAST", "Deep Stinger Check");

        }


        private IEnumerator StingerCorou(GameObject hitbox)
        {
            for (int i = 0; i < 15; i++)
            {
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.QuakeInvuln();
                yield return new WaitForSeconds(0.33f);
            }
            hitbox.Destroy();
            HeroController.instance.spellControl.SendEvent("NEXT");
            HeroController.instance.AffectedByGravity(true);
            VesselTrigger.Reset();
            Concentration.concentrationvalue = 0;
        }

        private void CreateOverdrive()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Overdrive", nailartfsm, "Dash Slash");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    if (!overdriveactive && !roundtripactive)
                    {

                        //StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                        StartCoroutine(OverdriveCorou());
                        StartCoroutine(GeneralHelper.EarlyControl(0.25f));

                        //FXHelper.PlayAudio("GreatSlash", 0.5f);
                        FXHelper.ActivateEffect("SharpFlashM");
                    }
                    else
                    {
                        HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
                    }


                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Dash?", "FINISHED", "Overdrive");
        }

        private IEnumerator OverdriveCorou()
        {
            overdriveactive = true;
            GameObject beam = GeneralHelper.CreateAttackTemplate("Overdrive", 2f, 2f, HeroController.instance.transform.position, new Vector2(3,2), new Vector2(0,0));
            SpriteRenderer render = beam.GetComponent<SpriteRenderer>();
            render.color = new Color(0, 0.55f, 1, 1);

            //tempdamage
            ContactDamage damage = beam.AddComponent<ContactDamage>();
            damage.SetDamage(overdrivedmg);
            damage.SetExtraOnly();
            beam.SetActive(true);

            StartCoroutine(Anims.PlayAnimation("Overdrive", render, 1f));
            StartCoroutine(OverdriveHitbox(damage));

            float dirmultiplier = HeroController.instance.transform.GetScaleX();
            float posx = beam.transform.GetPositionX();
            float counter = 0;
            while (beam != null && counter < 3f)
            {
                beam.transform.position = new Vector2(posx, beam.transform.position.y);
                posx += (-0.015f * dirmultiplier);
                counter += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }

            StartCoroutine(Dissipate(render));
            overdriveactive = false;
        }

        private IEnumerator Dissipate(SpriteRenderer render)
        {
            StartCoroutine(Anims.PlayAnimation("OverdriveDissipate", render, 0.3f));
            yield return new WaitForSeconds(0.3f);
            render.gameObject.Destroy();
        }

        private IEnumerator OverdriveHitbox(ContactDamage damage)
        {
            while (damage)
            {
                damage.HitAgain();
                FXHelper.PlayAudio("ButterflyBlade", 0.2f);
                yield return new WaitForSeconds(0.25f);

            }
        }

        private void CreateDrive()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Drive", nailartfsm, "Slash");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    if (!roundtripactive)
                    {

                        //StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                        StartCoroutine(DriveCorou());
                        StartCoroutine(GeneralHelper.EarlyControl(0.25f));

                        //FXHelper.PlayAudio("GreatSlash", 0.5f);
                        FXHelper.ActivateEffect("SharpFlashM");
                    } else
                    {
                       HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
                    }
                    

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has G Slash?", "FINISHED", "Drive");
        }

        private IEnumerator DriveCorou()
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject beam = FXHelper.grubberflyr;
                if (HeroController.instance.transform.GetScaleX() == 1)
                {
                    beam = FXHelper.grubberflyl;
                }
                GameObject clone = Instantiate(beam);
                clone.transform.localScale = clone.transform.localScale * 1.6f;
                clone.transform.position = HeroController.instance.transform.position;
                clone.GetComponent<tk2dSprite>().color = new Color(0, 0.55f, 1, 1);
                clone.LocateMyFSM("damages_enemy").Destroy();
                clone.GetComponent<PlayMakerCollisionEnter2D>().Destroy();
                clone.GetComponent<PlayMakerFixedUpdate>().Destroy();
                clone.Child("Terrain Detector").Destroy();


                if (i == 1)
                {
                    clone.transform.SetScaleY(clone.transform.GetScaleY() * -1);
                }

                ContactDamage damage = clone.AddComponent<ContactDamage>();
                damage.SetDamage(drivedmg);
                damage.SetExtraOnly();
                //FXHelper.PlayAudio("ButterflyBlade", 0.35f);


                clone.SetActive(true);
                yield return new WaitForSeconds(0.1f);

            }
        }

        private void CreateRoundTrip()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = GeneralHelper.CreateTemplateState("Round Trip", nailartfsm, "Slash");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    if (FXHelper.cyclonecopy != null && !roundtripactive)
                    {
                        GameObject cut = Instantiate(FXHelper.cyclonecopy);
                        cut.name = "Round Trip";
                        cut.transform.localScale = cut.transform.localScale / 2;
                        cut.GetComponent<tk2dSprite>().color = new Color(0, 0.55f, 1, 1);
                        cut.transform.parent = null;
                        cut.transform.position = HeroController.instance.transform.position;

                        BoxCollider2D collider = cut.AddComponent<BoxCollider2D>();
                        collider.isTrigger = true;
                        collider.size = new Vector2(10f, 2f);

                        //tempdamage
                        ContactDamage damage = cut.AddComponent<ContactDamage>();
                        damage.SetDamage(roundtripdmg);
                        damage.SetExtra();

                        cut.SetActive(true);

                        //StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                        StartCoroutine(GeneralHelper.DestroyAfter(cut, 6f));
                        StartCoroutine(RoundTripCorou(cut));
                        StartCoroutine(RoundTripHitbox(damage));
                        StartCoroutine(GeneralHelper.EarlyControl(0.15f));

                        //FXHelper.PlayAudio("GreatSlash", 0.5f);
                        FXHelper.ActivateEffect("SharpFlashM");
                    } else
                    {
                        HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
                    }

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Cyclone?", "FINISHED", "Round Trip");
        }

        private IEnumerator RoundTripHitbox(ContactDamage damage)
        {
            while (damage)
            {
                damage.HitAgain();
                HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<UnityEngine.AudioSource>().Play();
                yield return new WaitForSeconds(0.25f);

            }
        }

        private IEnumerator RoundTripCorou(GameObject cut)
        {
            roundtripactive = true;
            roundtripmoving = true;
            float dirmultiplier = HeroController.instance.transform.GetScaleX();
            float posx = cut.transform.GetPositionX();
            float counter = 0;
            while (cut != null && roundtripmoving && counter < 0.5f)
            {
                cut.transform.position = new Vector2(posx, cut.transform.position.y);
                posx += (-0.4f * dirmultiplier);
                counter += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            if (cut != null)
            {
                yield return new WaitForSeconds(4f);
                while (cut != null)
                {
                    Vector2 distancebetween = HeroController.instance.transform.position - cut.transform.position;
                    Vector2 newpos = new Vector2(cut.transform.GetPositionX(), cut.transform.GetPositionY()) + (distancebetween.normalized * 0.6f);
                    cut.transform.position = newpos;
                    if ((cut.transform.position - HeroController.instance.transform.position).magnitude <= 1f)
                    {
                        break;
                    } 
                    yield return new WaitForSeconds(0.01f);
                }
                cut.Destroy();
            }
            roundtripactive = false;
        }

        public void StopRoundTrip()
        {
            roundtripmoving = false;
        }

        private void CreateBlisteringSwords()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                FsmState combostate = spellcontrol.CreateState("Blistering Swords");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    if (!blisteringactive)
                    {
                        blisteringactive = true;

                        //fx
                        FXHelper.ActivateEffect("SharpFlashS");

                        //thats a lotta coroutines
                        StartCoroutine(BlisteringCorou());

                    }
                    spellcontrol.SendEvent("NEXT");
                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Fireball?", "CAST", "Blistering Swords");

        }

        private IEnumerator BlisteringCorou()
        {

            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = HeroController.instance.transform.position + new Vector3(0, 0.3f + (i * -0.2f), -0.003f);
                GameObject slices = GeneralHelper.CreateAttackTemplate("Blistering Swords", 1.5f, 1.5f, pos, new Vector2(1.5f,1f), new Vector2(0,0));
                slices.transform.SetScaleX(HeroController.instance.transform.GetScaleX() * -1.5f);

                SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                //tempdamage
                ContactDamage damage = slices.AddComponent<ContactDamage>();
                int damagevalue = blisteringdmg[0];
                if (HeroController.instance.playerData.fireballLevel > 1)
                {
                    damagevalue = blisteringdmg[1];
                }
                damage.SetDamage(damagevalue);
                damage.SetExtraOnly();

                StartCoroutine(Anims.PlayAnimation("BlisteringSwords", render, 5f));
                StartCoroutine(GeneralHelper.DestroyAfter(slices, 2.5f));
                
                StartCoroutine(BlisteringVelocity(slices,i));
            }
            yield return new WaitForSeconds(1f);
            blisteringactive = false;
        }

        private IEnumerator BlisteringVelocity(GameObject slices,int z)
        {
            Vector3 pos;
            for (int i = 0; i < 30; i++)
            {
                if (slices != null)
                {
                    pos = HeroController.instance.transform.position + new Vector3(0, 0.3f + (z * -0.3f), -0.03f);
                    slices.transform.position = pos;
                    slices.transform.SetScaleX(HeroController.instance.transform.GetScaleX() * -1.5f);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            yield return new WaitForSeconds(0.015f * z);
            if (slices != null)
            {
                if (z == 0)
                {
                    FXHelper.PlayAudio("ButterflyBlade", 0.35f);
                }
                float dirmultiplier = slices.transform.GetScaleX();
                float posx = slices.transform.GetPositionX();
                while (slices)
                {
                    slices.transform.position = new Vector2(posx, slices.transform.position.y);
                    posx += (0.4f * dirmultiplier);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        private void CreateSpiralSwords()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init)
            {
                FsmState combostate = spellcontrol.CreateState("Spiral Swords");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    if (!spiralactive)
                    {
                        spiralactive = true;
                        GameObject slices = GeneralHelper.CreateAttackTemplate("Spiral Swords", 1.5f, 1.5f, HeroController.instance.transform, new Vector3(0,-0.5f), new Vector2(3.5f,3.5f), new Vector2(0,0), true);
                        SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                        //tempdamage
                        ContactDamage damage = slices.AddComponent<ContactDamage>();
                        int damagevalue = spiraldmg[0];
                        if (HeroController.instance.playerData.quakeLevel > 1)
                        {
                            damagevalue = spiraldmg[1];
                        }

                        damage.SetDamage(damagevalue);
                        damage.DisableAuto();
                        damage.SetExtra();
                        

                        //fx
                        FXHelper.ActivateEffect("SharpFlashS");

                        //thats a lotta coroutines
                        StartCoroutine(Anims.PlayAnimation("SpiralSwords", render, 4f));
                        StartCoroutine(SpiralAudio(damage));
                        StartCoroutine(GeneralHelper.DestroyAfter(slices, 4f));
                        StartCoroutine(SpiralLoop(slices, render));

                    }
                    spellcontrol.SendEvent("NEXT");
                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Quake?", "CAST", "Spiral Swords");

        }

        private IEnumerator SpiralAudio(ContactDamage damage)
        {
            for (int i = 0; i < 16; i++)
            {
                damage.HitAgain();
                HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<UnityEngine.AudioSource>().Play();
                yield return new WaitForSeconds(0.25f);
            }
        }

        private IEnumerator SpiralLoop(GameObject slices, SpriteRenderer render)
        {
            float rotation = 0;
            while (slices)
            {
                slices.transform.SetRotationZ(rotation);
                rotation += 6;
                if (rotation == 361)
                {
                    rotation = 0;
                }
                yield return new WaitForSeconds(0.01f);
            }
            spiralactive = false;
        }
    }
}
