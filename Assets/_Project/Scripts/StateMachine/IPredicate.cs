using System;

public interface IPredicate
{
    bool Evaluate();
}

/// <summary>
/// Represents a predicate that uses a Func delegate to evaluate a condition.
/// </summary>
public class FuncPredicate : IPredicate {
    readonly Func<bool> func;

    public FuncPredicate(Func<bool> func) {
        this.func = func;
    }

    public bool Evaluate() => func.Invoke();
}

/// <summary>
/// Represents a predicate that encapsulates an action and evaluates to true once the action has been invoked.
/// </summary>
public class ActionPredicate : IPredicate {
    public bool flag;

    public ActionPredicate(ref Action eventReaction) => eventReaction += () => { flag = true; };

    public bool Evaluate() {
        bool result = flag;
        flag = false;
        return result;
    }
}