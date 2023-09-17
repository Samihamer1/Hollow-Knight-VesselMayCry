namespace VesselMayCry.ExtraSystems
{
    internal class QuakeBreak : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<PlayMakerFSM>() != null)
            {
                if (collider.gameObject.GetComponent<PlayMakerFSM>().FsmName == "quake_floor")
                {
                    collider.gameObject.GetComponent<PlayMakerFSM>().SendEvent("QUAKE FALL START");
                    collider.gameObject.GetComponent<PlayMakerFSM>().SendEvent("DESTROY");
                }

                foreach (PlayMakerFSM fsm in collider.gameObject.GetComponentsInParent<PlayMakerFSM>())
                {
                    if (fsm.FsmName == "Detect Quake")
                    {
                        fsm.SetState("Quake Hit");
                    }
                }
            }
        }
    }
}
