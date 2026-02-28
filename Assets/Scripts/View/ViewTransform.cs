using UnityEngine;

namespace View
{
    public record ViewTransform(Vector3 Position, float Yaw, float Pitch)
    {
        private const float MaxSpeed = 0.1f;
        private const float MaxAngularSpeed = 0.7f;
        public Vector3 Position { get; set; } = Position;
        public float Yaw { get; set; } = Yaw;
        public float Pitch { get; set; } = Pitch;

        private Quaternion Rotation => Quaternion.Euler(Pitch, Yaw, 0);

        private ViewTransform(Vector3 position, Vector3 bodyEulerAngles) :
            this(position, bodyEulerAngles.y, bodyEulerAngles.x) {}

        private ViewTransform(Vector3 position, Quaternion rotation) :
            this(position, rotation.eulerAngles) {}

        public static ViewTransform Lerp(ViewTransform a, ViewTransform b, float t)
        {
            var position = Vector3.Lerp(a.Position, b.Position, t);
            var rotation = Quaternion.Slerp(a.Rotation, b.Rotation, t);
            return new ViewTransform(position, rotation);
        }

        public static bool AreMatched(ViewTransform a, ViewTransform b, Tolerance tolerance)
        {
            return Vector3.Distance(a.Position, b.Position) <= tolerance.Distance
                   && Quaternion.Angle(a.Rotation, b.Rotation) <= tolerance.Angle;
        }

        public static float TimeDifference(ViewTransform a, ViewTransform b)
        {
            var posTime = Vector3.Distance(a.Position, b.Position) / MaxSpeed;
            var angTime = Quaternion.Angle(a.Rotation, b.Rotation) / MaxAngularSpeed;
            return Mathf.Max(posTime, angTime);
        }
    }
}