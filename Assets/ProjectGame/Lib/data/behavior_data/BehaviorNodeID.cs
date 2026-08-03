
namespace GameCore.Behavior
{
    public enum BehaviorNodeID
    {
        None = 0,
        Root,
        Sequence,
        Selector,
        Parallel,
        Race,
        Repeater,
        Delay,
        Timeout,
        Inverter,
        Failer,
        Limiter,
        RepeatUntilSuccess,
        Cooldown,
        Action,
        Condition,
        BlackboardCondition
    }
}
