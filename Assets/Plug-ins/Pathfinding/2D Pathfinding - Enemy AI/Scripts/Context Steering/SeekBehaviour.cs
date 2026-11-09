using UnityEngine;

namespace Pathfinding2D
{
    public class SeekBehaviour : SteeringBehaviour
    {
        [SerializeField]
        private AreaOfSight areaOfSight;

        [SerializeField]
        private bool showGizmos = true;

        private float[] interestTemp;
        private Vector2[] directions;

        public override void Detect() { }

        public override (float[] danger, float[] interest) GetSteering(SteeringBehaviourManager behaviourManager, Vector2 desiredDirection)
        {
            float[] danger = new float[behaviourManager.numberOfRays];
            float[] interest = new float[behaviourManager.numberOfRays];
            directions = behaviourManager.Directions;

            for (int i = 0; i < interest.Length; i++)
            {
                float alignment = Vector2.Dot(directions[i], desiredDirection.normalized);

                if (alignment >= areaOfSight.Alignment)
                {
                    float value = alignment;
                    if (value > interest[i])
                    {
                        interest[i] = value;
                    }
                }
            }

            interestTemp = interest;
            return (danger, interest);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            if (showGizmos == false)
                return;

            if (Application.isPlaying && interestTemp != null)
            {
                if (interestTemp != null)
                {
                    Gizmos.color = Color.green;
                    for (int i = 0; i < interestTemp.Length; i++)
                    {
                        Gizmos.DrawRay(transform.position, directions[i] * interestTemp[i] * 2);
                    }
                }
            }
        }
#endif
    }
}