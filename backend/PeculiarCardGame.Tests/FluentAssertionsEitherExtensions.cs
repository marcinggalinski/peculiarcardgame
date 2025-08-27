using FluentAssertions;
using FluentAssertions.Execution;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Tests;

public static class FluentAssertionsEitherExtensions
{
    public static EitherAssertions<TLeft, TRight> Should<TLeft, TRight>(this Either<TLeft, TRight> subject)
    {
        return new EitherAssertions<TLeft, TRight>(subject, AssertionChain.GetOrCreate());
    }
}

public class EitherAssertions<TLeft, TRight>
{
    private AssertionChain _chain;
    
    public Either<TLeft, TRight> Subject { get; }

    public EitherAssertions(Either<TLeft, TRight> subject, AssertionChain chain)
    {
        Subject = subject;
        _chain = chain;
    }

    [CustomAssertion]
    public AndConstraint<EitherAssertions<TLeft, TRight>> BeLeft(string because = "", params object[] becauseArgs)
    {
        _chain.BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsLeft)
            .FailWith("Expected {context:either} to be left{reason}, but it was right.", Subject);

        return new AndConstraint<EitherAssertions<TLeft, TRight>>(this);
    }

    [CustomAssertion]
    public AndConstraint<EitherAssertions<TLeft, TRight>> BeRight(string because = "", params object[] becauseArgs)
    {
        _chain.BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsRight)
            .FailWith("Expected {context:either} to be right{reason}, but it was left.", Subject);

        return new AndConstraint<EitherAssertions<TLeft, TRight>>(this);
    }
}

// public static class FluentAssertionsEitherExtensions
// {
//     public static EitherAssertions<TLeft, TRight> Should<TLeft, TRight>(this Either<TLeft, TRight> either)
//     {
//         return new EitherAssertions<TLeft, TRight>(either);
//     }
// }
//
// public class EitherAssertions<TLeft, TRight> : EitherAssertions<TLeft, TRight, EitherAssertions<TLeft, TRight>>
// {
//     public EitherAssertions(Either<TLeft, TRight> either) : base(either)
//     { }
// }
//
// public class EitherAssertions<TLeft, TRight, TAssertions>
//     where TAssertions : EitherAssertions<TLeft, TRight, TAssertions>
// {
//     public Either<TLeft, TRight> Subject { get; }
//         
//     public EitherAssertions(Either<TLeft, TRight> either)
//     {
//         Subject = either;
//     }
//
//     public AndConstraint<TAssertions> BeLeft(string because = "", params object[] becauseArgs)
//     {
//         Execute.Assertion
//             .ForCondition(Subject.IsLeft)
//             .BecauseOf(because, becauseArgs)
//             .FailWith("Expected {context:either} to be left{reason}, but it was right.", Subject);
//
//         return new AndConstraint<TAssertions>((TAssertions)this);
//     }
//
//     public AndConstraint<TAssertions> BeRight(string because = "", params object[] becauseArgs)
//     {
//         Execute.Assertion
//             .ForCondition(Subject.IsRight)
//             .BecauseOf(because, becauseArgs)
//             .FailWith("Expected {context:either} to be right{reason}, but it was left.", Subject);
//
//         return new AndConstraint<TAssertions>((TAssertions)this);
//     }
// }