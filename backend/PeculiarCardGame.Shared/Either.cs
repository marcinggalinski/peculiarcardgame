namespace PeculiarCardGame.Shared;

public readonly struct Either<TLeft, TRight>
{
    private readonly TLeft? _left;
    private readonly TRight? _right;

    public readonly bool IsLeft;
    public readonly bool IsRight;

    public TLeft Left => IsLeft ? _left! : throw new InvalidOperationException("This Either is Right");
    public TRight Right => IsRight ? _right! : throw new InvalidOperationException("This Either is Left");
    
    public static implicit operator Either<TLeft, TRight>(TLeft left) => new(left);
    public static implicit operator Either<TLeft, TRight>(TRight right) => new(right);

    public Either(TLeft left)
    {
        _left = left;
        IsLeft = true;
    }

    public Either(TRight right)
    {
        _right = right;
        IsRight = true;
    }
}
