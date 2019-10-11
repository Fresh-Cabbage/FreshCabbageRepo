public enum MovementState {
    IDLE = 0,
    DUCK = 1,
    WALK = 2,
    JUMP = 3,
    FALL = 4
}


public static class MovementStateExtensions {
    public static bool CanGoToJump(this MovementState state) => state == MovementState.IDLE || state == MovementState.WALK;
}