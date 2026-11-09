using UnityEngine;

namespace Pathfinding2D
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        public abstract void Detect();

        public abstract (float[] danger, float[] interest) GetSteering(SteeringBehaviourManager aiData, Vector2 desiredDirection);
    }
}