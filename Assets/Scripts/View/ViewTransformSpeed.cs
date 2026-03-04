namespace View
{
    public record ViewTransformSpeed(float Speed, float AngularSpeed)
    {
        public float Speed { get; set; } = Speed;
        public float AngularSpeed { get; set; } = AngularSpeed;

        public static ViewTransformSpeed Snap => new (0.1f, 0.7f);
        public static ViewTransformSpeed Move => new (7f, 400f);
    }
}