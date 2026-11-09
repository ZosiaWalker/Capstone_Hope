using System;
using UnityEngine;

namespace Pathfinding2D
{
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public bool reachClosestWalkableNode;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, bool _reachClosestWalkableNode, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            reachClosestWalkableNode = _reachClosestWalkableNode;
            callback = _callback;
        }
    }
}