using Robust.Client.Animations;
using Robust.Client.UserInterface;
using Robust.Shared.Animations;
using Robust.Shared.Log;


namespace Content.StyleSheetify.Client.Utils;

public sealed class AnimationExtend<T> : Control
{
    public TimeSpan Length { get; private set; }
    public Animation? Animation { get; private set; }
    public Action? AnimationIsCompleted;

    private T _realValue = default!;
    private AnimationTrackControlProperty? _track;
    private readonly Action<T> _action;
    private readonly Guid _guid = Guid.NewGuid();

    public AnimationTrackControlProperty? Track
    {
        get => _track;
        private init
        {
            if(value is null) return;
            _track = value;
            _track.Property = nameof(Value);

            Length = _track.KeyFrames.Aggregate(TimeSpan.Zero,
                (span, frame) => span.Add(TimeSpan.FromSeconds(frame.KeyTime)));

            Animation = new()
            {
                Length = Length, AnimationTracks = { _track }
            };
        }
    }

    [Animatable]
    public T Value
    {
        get => _realValue;
        set
        {
            _action(value);
            _realValue = value;
        }
    }

    public AnimationExtend(
        Action<T> action,
        Control parent,
        AnimationInterpolationMode interpolationMode,
        List<AnimationTrackProperty.KeyFrame> keyFrames
        )
    {
        _action = action;
        parent.AddChild(this);
        var track = new AnimationTrackControlProperty()
        {
            InterpolationMode = interpolationMode,
        };

        foreach (var frame in keyFrames)
        {
            track.KeyFrames.Add(frame);
        }

        Track = track;

        AnimationCompleted += OnAnimationCompleted;
    }

    private void OnAnimationCompleted(string obj)
    {
        if(obj == _guid.ToString()) AnimationIsCompleted?.Invoke();
    }

    public void PlayAnimation()
    {
        if(Animation is null) throw new Exception("Animation is null");
        PlayAnimation(Animation,_guid.ToString());
    }

    public bool HasRunningAnimation()
    {
        return HasRunningAnimation(_guid.ToString());
    }

}
