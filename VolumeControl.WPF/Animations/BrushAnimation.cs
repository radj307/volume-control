using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VolumeControl.WPF.Animations
{
    /// <summary>
    /// Animation type that transitions between two arbitrary <see cref="Brush"/> instances.
    /// </summary>
    /// <remarks>
    /// Contains code from <see href="https://stackoverflow.com/a/29659723/8705305">stackoverflow</see>.
    /// </remarks>
    public class BrushAnimation : AnimationTimeline
    {
        #region FromProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="From"/>.
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            nameof(From),
            typeof(Brush),
            typeof(BrushAnimation));
        /// <summary>
        /// Gets or sets the animation's starting brush.
        /// </summary>
        /// <returns>The starting <see cref="Brush"/> of this animation. The default is <see langword="null"/>.</returns>
        public Brush From
        {
            get => (Brush)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }
        #endregion FromProperty

        #region ToProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="To"/>.
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            nameof(To),
            typeof(Brush),
            typeof(BrushAnimation));
        /// <summary>
        /// Gets or sets the animation's ending brush.
        /// </summary>
        /// <returns>The ending <see cref="Brush"/> of this animation. The default is <see langword="null"/>.</returns>
        public Brush To
        {
            get => (Brush)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }
        #endregion ToProperty

        #region Override Properties
        /// <summary>
        /// The type of property that can be animated.
        /// </summary>
        /// <returns><see cref="Brush"/></returns>
        public override Type TargetPropertyType => typeof(Brush);
        #endregion Override Properties

        #region Override Methods
        /// <inheritdoc/>
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            var defaultOriginBrush = (Brush)defaultOriginValue;
            var defaultDestinationBrush = (Brush)defaultDestinationValue;

            if (!animationClock.CurrentProgress.HasValue)
                return Brushes.Transparent;

            //use the standard values if From and To are not set 
            //(it is the value of the given property)
            defaultOriginValue = this.From ?? defaultOriginValue;
            defaultDestinationValue = this.To ?? defaultDestinationValue;

            if (animationClock.CurrentProgress.Value == 0)
                return defaultOriginValue;
            if (animationClock.CurrentProgress.Value == 1)
                return defaultDestinationValue;

            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = defaultOriginBrush,
                Child = new Border()
                {
                    Background = defaultDestinationBrush,
                    Opacity = animationClock.CurrentProgress.Value,
                }
            });
        }
        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }
        #endregion Override Methods
    }
}
