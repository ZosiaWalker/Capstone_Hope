using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding2D
{
    /// <summary>
    /// Manages steering behaviours for AI agents.
    /// Combines multiple behaviours to determine the final movement direction
    /// while considering obstacles, avoidance, and other movement rules.
    /// </summary>
    public class SteeringBehaviourManager : MonoBehaviour
    {
        /// <summary>
        /// The number of rays used to sample possible movement directions.
        /// Higher values increase accuracy at the cost of performance.
        /// </summary>
        [NumberOfRaysPreview]
        public int numberOfRays = 8;

        /// <summary>
        /// Calculates the final movement direction for an agent by combining
        /// the influence of multiple steering behaviours.
        /// </summary>
        /// <param name="behaviours">
        /// A list of steering behaviours that influence movement
        /// (e.g., obstacle avoidance, enemy avoidance, target seeking).
        /// </param>
        /// <param name="desiredDirection">
        /// The agent's preferred direction (calculated from A*) before steering adjustments.
        /// </param>
        /// <returns>
        /// A normalized <see cref="Vector2"/> representing the chosen
        /// movement direction. Returns <c>Vector2.zero</c> if no valid
        /// direction is available.
        /// </returns>
        public Vector2 GetFinalDirection(List<SteeringBehaviour> behaviours, Vector2 desiredDirection)
        {
            float[] danger = new float[numberOfRays];
            float[] interest = new float[numberOfRays];

            foreach (SteeringBehaviour behaviour in behaviours)
            {
                var (tempDanger, tempInterest) = behaviour.GetSteering(this, desiredDirection);

                for (int i = 0; i < numberOfRays; i++)
                {
                    danger[i] = Mathf.Max(danger[i], tempDanger[i]);
                    interest[i] = Mathf.Max(interest[i], tempInterest[i]);
                }
            }
            for (int i = 0; i < numberOfRays; i++)
            {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }

            Vector2[] directions = Directions;
            int indexOfMaxInterest = Array.IndexOf(interest, interest.Max());
            Vector2 outputDirection = (interest[indexOfMaxInterest] == 0f) ? Vector2.zero : directions[indexOfMaxInterest];

            return outputDirection.normalized;

            //float bestScore = float.MinValue;
            //Vector2 bestDirection = Vector2.zero;

            //for (int i = 0; i < numberOfRays; i++)
            //{
            //    float alignment = Vector2.Dot(directions[i], desiredDirection);
            //    float score = interest[i] - danger[i] + alignment * 0.2f;

            //    if (score > bestScore)
            //    {
            //        bestScore = score;
            //        bestDirection = directions[i];
            //    }
            //}

            //return bestDirection.normalized;
        }

        /// <summary>
        /// An array of evenly distributed direction vectors
        /// around a circle, based on the <see cref="numberOfRays"/>.
        /// </summary>
        public Vector2[] Directions
        {
            get
            {
                Vector2[] directions = new Vector2[numberOfRays];
                for (int i = 0; i < numberOfRays; i++)
                {
                    float angle = i * Mathf.PI * 2f / numberOfRays;
                    Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    directions[i] = direction;
                }
                return directions;
            }
        }
    }
}