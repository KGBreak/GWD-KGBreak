using UnityEngine;
namespace Util
{
    public static class DirectionHelper
    {
        public static Vector3 GetWorldDirection(ExitDirection direction)
        {
            return direction switch
            {
                ExitDirection.Forward => Vector3.forward,
                ExitDirection.Backward => Vector3.back,
                ExitDirection.Left => Vector3.left,
                ExitDirection.Right => Vector3.right,
                ExitDirection.Up => Vector3.up,
                ExitDirection.Down => Vector3.down,
                _ => Vector3.zero
            };
        }
    }

    public enum ExitDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    public enum VoiceActor
    {
        Attila,
        Emil,
        Tim,
        David,
        Intercom
    }

    public enum InvestigatingState
    {
        None,
        Suspicious,
        Spotted
    }

    [System.Serializable]
    public class PathPoint
    {
        public Transform position;
        public float timeSpent;
        public Actions action;
        public Vector3 direction;
    }
}
