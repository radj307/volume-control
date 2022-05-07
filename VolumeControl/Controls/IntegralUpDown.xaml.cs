using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace VolumeControl.Controls
{
    /// <summary>
    /// Interaction logic for IntegralUpDown.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Textbox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_bIncrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_bDecrease", Type = typeof(RepeatButton))]
    public partial class IntegralUpDown : UserControl
    {
        public IntegralUpDown()
        {
            Textbox = null!;
            bIncrease = null!;
            bDecrease = null!;
            InitializeComponent();
        }

        protected TextBox Textbox;
        protected RepeatButton bIncrease;
        protected RepeatButton bDecrease;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Attach_Textbox();
            Attach_bIncrease();
            Attach_bDecrease();
            Attach_Commands();
        }

        private void Attach_Textbox()
        {
            if (GetTemplateChild("PART_Textbox") is TextBox tb)
            {
                Textbox = tb;
                Textbox.LostFocus += Textbox_OnLostFocus;
            }
        }
        private void Attach_bIncrease()
        {
            if (GetTemplateChild("PART_bIncrease") is RepeatButton rb)
            {
                bIncrease = rb;
                bIncrease.Focusable = false;
                bIncrease.Command = _minorIncreaseValueCommand;
                bIncrease.PreviewMouseLeftButtonDown += (sender, e) => RemoveFocus();
            }
        }
        private void Attach_bDecrease()
        {
            if (GetTemplateChild("PART_bDecrease") is RepeatButton rb)
            {
                bDecrease = rb;
                bDecrease.Focusable = false;
                bDecrease.Command = _minorDecreaseValueCommand;
                bDecrease.PreviewMouseLeftButtonDown += (sender, e) => RemoveFocus();
            }
        }
        private void Attach_Commands()
        {
            CommandBindings.Add(new(_minorIncreaseValueCommand, (a, b) => IncreaseValue()));
            CommandBindings.Add(new(_minorDecreaseValueCommand, (a, b) => DecreaseValue()));
            CommandBindings.Add(new(_updateValueStringCommand, (a,b) =>
            {
                Value = ParseStringToValue(Textbox.Text);
            }));

            Textbox.InputBindings.Add(new KeyBinding(_minorIncreaseValueCommand, new KeyGesture(Key.Up)));
            Textbox.InputBindings.Add(new KeyBinding(_minorDecreaseValueCommand, new KeyGesture(Key.Down)));
            Textbox.InputBindings.Add(new KeyBinding(_updateValueStringCommand, new KeyGesture(Key.Enter)));
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(IntegralUpDown), new PropertyMetadata(0m, OnValueChanged, CoerceValue));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(IntegralUpDown), new PropertyMetadata(0m, OnMinValueChanged, CoerceMinValue));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(IntegralUpDown), new PropertyMetadata(100m, OnMaxValueChanged, CoerceMaxValue));

        public int Value
        {
            get => Convert.ToInt32(GetValue(ValueProperty));
            set => SetValue(ValueProperty, value);
        }
        public int MinValue
        {
            get => Convert.ToInt32(GetValue(MinValueProperty));
            set => SetValue(MinValueProperty, value);
        }
        public int MaxValue
        {
            get => Convert.ToInt32(GetValue(MaxValueProperty));
            set => SetValue(MaxValueProperty, value);
        }

        private void IncreaseValue() => Value = ClampValue(ParseStringToValue(Textbox.Text) + 1);
        private void DecreaseValue() => Value = ClampValue(ParseStringToValue(Textbox.Text) - 1);

        private static void OnValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnMinValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (IntegralUpDown)element;
            var minValue = Convert.ToInt32(e.NewValue);

            if (minValue > control.MaxValue)
            {
                control.MaxValue = minValue;
            }

            if (minValue >= control.Value)
            {
                control.Value = minValue;
            }
        }
        private static void OnMaxValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (IntegralUpDown)element;
            var maxValue = Convert.ToInt32(e.NewValue);

            if (maxValue < control.MinValue)
            {
                control.MinValue = maxValue;
            }

            if (maxValue <= control.Value)
            {
                control.Value = maxValue;
            }
        }

        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (IntegralUpDown)element;
            var value = control.ClampValue(Convert.ToInt32(baseValue));

            control.Textbox.Text = value.ToString(CultureInfo.CurrentCulture);

            return value;
        }
        private static object CoerceMinValue(DependencyObject element, object baseValue)
        {
            var minValue = Convert.ToInt32(baseValue);
            return minValue;
        }
        private static object CoerceMaxValue(DependencyObject element, object baseValue)
        {
            var maxValue = Convert.ToInt32(baseValue);
            return maxValue;
        }
        private int ClampValue(int value)
        {
            if (value < MinValue)
                value = MinValue;
            else if (value > MaxValue)
                value = MaxValue;
            return value;
        }

        private static int ParseStringToValue(string str)
        {
            _ = int.TryParse(str, out int value); //< always outputs a int number
            return value;
        }

        private readonly RoutedUICommand _minorIncreaseValueCommand = new("MinorIncreaseValue", "MinorIncreaseValue", typeof(IntegralUpDown));
        private readonly RoutedUICommand _minorDecreaseValueCommand = new("MinorDecreaseValue", "MinorDecreaseValue", typeof(IntegralUpDown));
        private readonly RoutedUICommand _updateValueStringCommand = new("UpdateValueString", "UpdateStringValue", typeof(IntegralUpDown));

        private void Textbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Value = ParseStringToValue(Textbox.Text);
        }

        private void RemoveFocus()
        {
            Focusable = true;
            Focus();
            Focusable = false;
        }
    }
}
