using FrogCore;
using FrogCore.Ext;

namespace VesselMayCry.Yamato
{
    internal class Base : MonoBehaviour
    {
        private bool isattacking = false;
        private Vector3 pos;
        private bool init = false;
        private float colorR = 1f;
        private float colorG = 1f;
        private float colorB = 1f;
        private float maxColorR = 1f;
        private float maxColorG = 0.5f;

        private tk2dSprite slash;
        private tk2dSprite slashalt;
        private tk2dSprite wallslash;
        private tk2dSprite upslash;
        private tk2dSprite downslash;

        public void Awake()
        {
            CreateJCut();
            CreateComboC();
            CreateUpperSlash();
            init = true;

            slash = HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<tk2dSprite>();
            slashalt = HeroController.instance.gameObject.Child("Attacks").Child("AltSlash").GetComponent<tk2dSprite>();
            wallslash = HeroController.instance.gameObject.Child("Attacks").Child("WallSlash").GetComponent<tk2dSprite>();
            upslash = HeroController.instance.gameObject.Child("Attacks").Child("UpSlash").GetComponent<tk2dSprite>();
            downslash = HeroController.instance.gameObject.Child("Attacks").Child("DownSlash").GetComponent<tk2dSprite>();
        }
        public void OnEnable()
        {
            pos = HeroController.instance.transform.position;
            ModHooks.AttackHook += Levitate;
            CreateJCut();
            CreateComboC();
            CreateUpperSlash();
            
        }

        public void OnDisable()
        {
            ModHooks.AttackHook -= Levitate;
            HeroController.instance.AffectedByGravity(true);
        }

        public void Update()
        {
            if (HeroController.instance.cState.jumping)
            {
                pos = HeroController.instance.transform.position;
            }

            if ((HeroController.instance.GetState("attacking") && (!HeroController.instance.cState.onGround &&!HeroController.instance.cState.downAttacking && !HeroController.instance.cState.upAttacking)) || isattacking)
            {
                slash.color = new Color(colorR, colorG, colorB);
                slashalt.color = new Color(colorR, colorG, colorB);
                wallslash.color = new Color(colorR, colorG, colorB);
                upslash.color = new Color(colorR, colorG, colorB);
                downslash.color = new Color(colorR, colorG, colorB);
                HeroController.instance.transform.position = pos;
                HeroController.instance.AffectedByGravity(false);

            } else 
            {
                HeroController.instance.AffectedByGravity(true);
            }

            //color set
            float concentrationpercent = Concentration.concentrationvalue / Concentration.concentrationmax;
            colorR = 1f - (maxColorR * concentrationpercent);
            colorG = 1f - (maxColorG * concentrationpercent);
        }

        private void Levitate(AttackDirection direction)
        {
            pos = HeroController.instance.transform.position;
        }

        private IEnumerator timedattacktoggle(float time)
        {
            isattacking = true;
            pos = HeroController.instance.transform.position;
            yield return new WaitForSeconds(time);
            isattacking = false;

        }
        private IEnumerator DestroyAfter(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(obj);
            HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
            HeroController.instance.spellControl.SendEvent("NEXT");
            HeroController.instance.StartAnimationControl();
        }

        private GameObject CreateAttackTemplate(string name, float scalex, float scaley, Vector3 position, bool coloured)
        {
            GameObject cut = new GameObject(name);
            cut.transform.SetScaleX(scalex * HeroController.instance.transform.GetScaleX());
            cut.transform.SetScaleY(scaley);
            cut.transform.position = position;
            PolygonCollider2D collider = cut.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;
            SpriteRenderer render = cut.AddComponent<SpriteRenderer>();
            if (coloured)
            {
                render.color = new Color(colorR, colorG, colorB);
            }           
            cut.layer = (int)PhysLayers.HERO_ATTACK;

            return cut;
        }

        private void CreateJCut()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init) { 
                //state creations.
                FsmState newstate = nailartfsm.CreateState("Judgement Cut");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    GameObject cut = CreateAttackTemplate("Judgement Cut", 1.3f, 1.3f, HeroController.instance.transform.position + new Vector3(-5 * HeroController.instance.transform.GetScaleX(), 0), false);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    cut.SetActive(true);

                    //find a target....
                    HealthManager[] obj = FindObjectsOfType<HealthManager>();
                    GameObject target = null;
                    float distance = 9999999f;
                    for (int j = 0; j < obj.Length; j++)
                    {

                        float mag = (obj[j].gameObject.transform.position - cut.transform.position).magnitude;
                        if (mag < distance)
                        {
                            distance = mag;
                            target = obj[j].gameObject;
                        }
                    }

                    if (target != null) 
                    {
                        Vector3 vector3 = new Vector3(target.transform.position.x, target.transform.position.y, HeroController.instance.transform.position.z);
                        cut.transform.position = vector3;
                    }

                    StartCoroutine(Anims.PlayAnimation("JudgementCut", render, 0.4f));
                    StartCoroutine(DestroyAfter(cut,0.4f));
                    StartCoroutine(timedattacktoggle(0.4f));

                    //anim test
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    animator.Stop();
                    animator.Play("JC");
                
                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has G Slash?", "FINISHED", "Judgement Cut");
        }

        private void CreateUpperSlash()
        {
            PlayMakerFSM nailartfsm = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (!init)
            {
                //state creations.
                FsmState newstate = nailartfsm.CreateState("Upper Slash");
                newstate.RemoveTransition("FINISHED");
                newstate.AddMethod(() =>
                {
                    GameObject cut = CreateAttackTemplate("Upper Slash", 2.5f, 2.5f, HeroController.instance.transform.position, true);
                    SpriteRenderer render = cut.GetComponent<SpriteRenderer>();
                    //tempdamage
                    ContactDamage damage = cut.AddComponent<ContactDamage>();
                    cut.SetActive(true);

                    StartCoroutine(Anims.PlayAnimation("UpperSlash", render, 0.15f));
                    StartCoroutine(DestroyAfter(cut, 0.15f));
                    StartCoroutine(timedattacktoggle(0.15f));

                    //anim test
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    animator.Stop();
                    animator.Play("UpperSlash");

                });
                newstate.AddTransition("NEXT", "Regain Control");
            }
            nailartfsm.ChangeTransition("Has Cyclone?", "FINISHED", "Upper Slash");
        }


        private void CreateComboC()
        {
            PlayMakerFSM spellcontrol = HeroController.instance.spellControl;
            if (!init) { 
                FsmState combostate = spellcontrol.CreateState("Combo C");
                combostate.RemoveTransition("FINISHED");
                spellcontrol.GetState("Can Cast?").GetAction<IntCompare>().integer2 = 0;
                spellcontrol.GetState("Can Cast? QC").GetAction<IntCompare>().integer2 = 0;
                combostate.AddMethod(() =>
                {
                    GameObject slices = CreateAttackTemplate("ComboC", 4f, 2.4f, HeroController.instance.transform.position, true);
                    SpriteRenderer render = slices.GetComponent<SpriteRenderer>();

                    //tempdamage
                    ContactDamage damage = slices.AddComponent<ContactDamage>();
                    damage.SetDamage(15);
                    slices.SetActive(true);

                    //thats a lotta coroutines
                    StartCoroutine(Anims.PlayAnimation("ComboC", render, 0.4f));
                    StartCoroutine(ComboCAudio());
                    StartCoroutine(DestroyAfter(slices, 0.4f));
                    StartCoroutine(timedattacktoggle(0.4f));
                    StartCoroutine(ComboCHitboxCycle(slices));

                    //anim
                    tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
                    HeroController.instance.StopAnimationControl();
                    animator.Play("ComboC");

                });
                combostate.AddTransition("NEXT", "Spell End");
            }
            spellcontrol.ChangeTransition("Has Fireball?", "CAST", "Combo C");

        }

        private IEnumerator ComboCAudio()
        {
            for (int i = 0; i < 5; i++)
            {
                HeroController.instance.gameObject.Child("Attacks").Child("Slash").GetComponent<UnityEngine.AudioSource>().Play();
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
