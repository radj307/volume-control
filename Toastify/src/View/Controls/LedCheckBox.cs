using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Toastify.View.Controls
{
    public class LedCheckBox : CheckBox
    {
        #region Public Properties

        public double BorderStrokeThickness
        {
            get { return (double)this.GetValue(BorderStrokeThicknessProperty); }
            set { this.SetValue(BorderStrokeThicknessProperty, value); }
        }

        public Color CenterGlowColor
        {
            get { return (Color)this.GetValue(CenterGlowColorProperty); }
            set { this.SetValue(CenterGlowColorProperty, value); }
        }

        public Color CornerLightColor
        {
            get { return (Color)this.GetValue(CornerLightColorProperty); }
            set { this.SetValue(CornerLightColorProperty, value); }
        }

        public Brush OnColor
        {
            get { return (Brush)this.GetValue(OnColorProperty); }
            set { this.SetValue(OnColorProperty, value); }
        }

        public Brush OffColor
        {
            get { return (Brush)this.GetValue(OffColorProperty); }
            set { this.SetValue(OffColorProperty, value); }
        }

        public Brush ThirdStateColor
        {
            get { return (Brush)this.GetValue(ThirdStateColorProperty); }
            set { this.SetValue(ThirdStateColorProperty, value); }
        }

        public bool IsReadonly
        {
            get { return (bool)this.GetValue(IsReadonlyProperty); }
            set { this.SetValue(IsReadonlyProperty, value); }
        }

        #endregion

        static LedCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LedCheckBox), new FrameworkPropertyMetadata(typeof(LedCheckBox)));
        }

        #region Dependency Properties

        public static readonly DependencyProperty BorderStrokeThicknessProperty = DependencyProperty.Register(nameof(BorderStrokeThickness), typeof(double), typeof(LedCheckBox), new PropertyMetadata(1.0));
        public static readonly DependencyProperty CenterGlowColorProperty = DependencyProperty.Register(nameof(CenterGlowColor), typeof(Color), typeof(LedCheckBox), new PropertyMetadata(Colors.White));
        public static readonly DependencyProperty CornerLightColorProperty = DependencyProperty.Register(nameof(CornerLightColor), typeof(Color), typeof(LedCheckBox), new PropertyMetadata(Colors.White));

        public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register(nameof(OnColor), typeof(Brush), typeof(LedCheckBox), new PropertyMetadata(Brushes.Green));
        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register(nameof(OffColor), typeof(Brush), typeof(LedCheckBox), new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty ThirdStateColorProperty = DependencyProperty.Register(nameof(ThirdStateColor), typeof(Brush), typeof(LedCheckBox), new PropertyMetadata(Brushes.Orange));

        public static readonly DependencyProperty IsReadonlyProperty = DependencyProperty.Register(nameof(IsReadonly), typeof(bool), typeof(LedCheckBox), new PropertyMetadata(false));

        #endregion
    }
}