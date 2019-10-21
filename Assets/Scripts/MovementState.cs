public enum MovementState {
    IDLE = 0,
    DUCK = 1,
    WALK = 2,
    ROLL = 3,
    JUMP = 4,
    FALL = 5
}


public static class MovementStateExtensions {
    public static bool CanGoToJump(this MovementState state) => state != MovementState.DUCK && state != MovementState.JUMP;
    public static bool CanGoToRoll(this MovementState state) => state == MovementState.IDLE || state == MovementState.WALK;
}