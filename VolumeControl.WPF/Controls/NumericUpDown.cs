using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VolumeControl.WPF.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class NumericUpDown : Control
    {
        #region Properties

        #region Value

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(NumericUpDown),
                                        new PropertyMetadata(0m, OnValueChanged, CoerceValue));

        public decimal Value
        {
            get => Convert.ToDecimal(this.GetValue(ValueProperty));
            set => this.SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control && control.TextBox != null)
            {
                control.TextBox.UndoLimit = 0;
                control.TextBox.UndoLimit = 1;
            }
        }

        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            decimal value = Convert.ToDecimal(baseValue);
            if (element is NumericUpDown control)
            {
                control.CoerceValueToBounds(ref value);

                // Get the text representation of Value
                string? valueString = value.ToString(control.Culture);

                // Count all decimal places
                int decimalPlaces = control.GetDecimalPlacesCount(valueString);

                if (decimalPlaces > control.DecimalPlaces)
                {
                    if (control.IsDecimalPointDynamic)
                    {
                        // Assigning DecimalPlaces will coerce the number
                        control.DecimalPlaces = decimalPlaces;

                        // If the specified number of decimal places is still too much
                        if (decimalPlaces > control.DecimalPlaces)
                            value = control.TruncateValue(valueString, control.DecimalPlaces);
                    }
                    else
                    {
                        // Remove all overflowing decimal places
                        value = control.TruncateValue(valueString, decimalPlaces);
                    }
                }
                else if (control.IsDecimalPointDynamic)
                {
                    control.DecimalPlaces = decimalPlaces;
                }

                if (control.IsThousandSeparatorVisible)
                {
                    if (control.TextBox != null)
                        control.TextBox.Text = value.ToString("N", control.Culture);
                }
                else
                {
                    if (control.TextBox != null)
                        control.TextBox.Text = value.ToString("F", control.Culture);
                }
            }
            return value;
        }

        #endregion

        #region MaxValue

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(100000000m, OnMaxValueChanged, CoerceMaxValue));

        public decimal MaxValue
        {
            get => Convert.ToDecimal(this.GetValue(MaxValueProperty));
            set => this.SetValue(MaxValueProperty, value);
        }

        private static void OnMaxValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
            {
                decimal maxValue = Convert.ToDecimal(e.NewValue);
                // If maxValue steps over MinValue, shift it
                if (maxValue < control.MinValue)
                    control.MinValue = maxValue;

                if (maxValue <= control.Value)
                    control.Value = maxValue;
            }
        }

        private static object CoerceMaxValue(DependencyObject element, object baseValue)
        {
            decimal maxValue = (decimal)baseValue;

            return maxValue == decimal.MaxValue ? DependencyProperty.UnsetValue : maxValue;
        }

        #endregion

        #region MinValue

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(NumericUpDown),
                                        new PropertyMetadata(0m, OnMinValueChanged,
                                                             CoerceMinValue));

        public decimal MinValue
        {
            get => Convert.ToDecimal(this.GetValue(MinValueProperty));
            set => this.SetValue(MinValueProperty, value);
        }

        private static void OnMinValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
            {
                decimal minValue = (decimal)e.NewValue;

                // If minValue steps over MaxValue, shift it
                if (minValue > control.MaxValue)
                    control.MaxValue = minValue;

                if (minValue >= control.Value)
                    control.Value = minValue;
            }
        }

        private static object CoerceMinValue(DependencyObject element, object baseValue)
        {
            decimal minValue = (decimal)baseValue;

            return minValue == decimal.MinValue ? DependencyProperty.UnsetValue : minValue;
        }

        #endregion

        #region DecimalPlaces

        public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnDecimalPlacesChanged, CoerceDecimalPlaces));

        public int DecimalPlaces
        {
            get => Convert.ToInt32(this.GetValue(DecimalPlacesProperty));
            set => this.SetValue(DecimalPlacesProperty, value);
        }

        private static void OnDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
            {
                int decimalPlaces = (int)e.NewValue;

                control.Culture.NumberFormat.NumberDecimalDigits = decimalPlaces;

                if (control.IsDecimalPointDynamic)
                {
                    control.IsDecimalPointDynamic = false;
                    control.InvalidateProperty(ValueProperty);
                    control.IsDecimalPointDynamic = true;
                }
                else
                {
                    control.InvalidateProperty(ValueProperty);
                }
            }
        }

        private static object CoerceDecimalPlaces(DependencyObject element, object baseValue)
        {
            int decimalPlaces = (int)baseValue;
            if (element is NumericUpDown control)
            {
                if (decimalPlaces < control.MinDecimalPlaces)
                    decimalPlaces = control.MinDecimalPlaces;
                else if (decimalPlaces > control.MaxDecimalPlaces)
                {
                    decimalPlaces = control.MaxDecimalPlaces;
                }
            }
            return decimalPlaces;
        }

        #endregion

        #region MaxDecimalPlaces

        public static readonly DependencyProperty MaxDecimalPlacesProperty = DependencyProperty.Register("MaxDecimalPlaces", typeof(int), typeof(NumericUpDown), new PropertyMetadata(28, OnMaxDecimalPlacesChanged, CoerceMaxDecimalPlaces));

        public int MaxDecimalPlaces
        {
            get => Convert.ToInt32(this.GetValue(MaxDecimalPlacesProperty));
            set => this.SetValue(MaxDecimalPlacesProperty, value);
        }

        private static void OnMaxDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
                control.InvalidateProperty(DecimalPlacesProperty);
        }

        private static object CoerceMaxDecimalPlaces(DependencyObject element, object baseValue)
        {
            int maxDecimalPlaces = (int)baseValue;
            if (element is NumericUpDown control)
            {
                if (maxDecimalPlaces > 28)
                    maxDecimalPlaces = 28;
                else if (maxDecimalPlaces < 0)
                {
                    maxDecimalPlaces = 0;
                }
                else if (maxDecimalPlaces < control.MinDecimalPlaces)
                {
                    control.MinDecimalPlaces = maxDecimalPlaces;
                }
            }
            return maxDecimalPlaces;
        }

        #endregion

        #region MinDecimalPlaces

        public static readonly DependencyProperty MinDecimalPlacesProperty = DependencyProperty.Register("MinDecimalPlaces", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnMinDecimalPlacesChanged, CoerceMinDecimalPlaces));

        public int MinDecimalPlaces
        {
            get => Convert.ToInt32(this.GetValue(MinDecimalPlacesProperty));
            set => this.SetValue(MinDecimalPlacesProperty, value);
        }

        private static void OnMinDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
                control.InvalidateProperty(DecimalPlacesProperty);
        }

        private static object CoerceMinDecimalPlaces(DependencyObject element, object baseValue)
        {
            int minDecimalPlaces = (int)baseValue;
            if (element is NumericUpDown control)
            {
                if (minDecimalPlaces < 0)
                    minDecimalPlaces = 0;
                else if (minDecimalPlaces > 28)
                {
                    minDecimalPlaces = 28;
                }
                else if (minDecimalPlaces > control.MaxDecimalPlaces)
                {
                    control.MaxDecimalPlaces = minDecimalPlaces;
                }
            }
            return minDecimalPlaces;
        }

        #endregion

        #region IsDecimalPointDynamic

        public static readonly DependencyProperty IsDecimalPointDynamicProperty = DependencyProperty.Register("IsDecimalPointDynamic", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        public bool IsDecimalPointDynamic
        {
            get => (bool)this.GetValue(IsDecimalPointDynamicProperty);
            set => this.SetValue(IsDecimalPointDynamicProperty, value);
        }

        #endregion

        #region MinorDelta

        public static readonly DependencyProperty MinorDeltaProperty = DependencyProperty.Register("MinorDelta", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(1m, OnMinorDeltaChanged, CoerceMinorDelta));

        public decimal MinorDelta
        {
            get => (decimal)this.GetValue(MinorDeltaProperty);
            set => this.SetValue(MinorDeltaProperty, value);
        }

        private static void OnMinorDeltaChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
            {
                decimal minorDelta = (decimal)e.NewValue;

                if (minorDelta > control.MajorDelta)
                    control.MajorDelta = minorDelta;
            }
        }

        private static object CoerceMinorDelta(DependencyObject element, object baseValue)
        {
            decimal minorDelta = (decimal)baseValue;

            return minorDelta;
        }

        #endregion

        #region MajorDelta

        public static readonly DependencyProperty MajorDeltaProperty = DependencyProperty.Register("MajorDelta", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(10m, OnMajorDeltaChanged, CoerceMajorDelta));

        public decimal MajorDelta
        {
            get => (decimal)this.GetValue(MajorDeltaProperty);
            set => this.SetValue(MajorDeltaProperty, value);
        }

        private static void OnMajorDeltaChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
            {
                decimal majorDelta = (decimal)e.NewValue;

                if (majorDelta < control.MinorDelta)
                    control.MinorDelta = majorDelta;
            }
        }

        private static object CoerceMajorDelta(DependencyObject element, object baseValue)
        {
            decimal majorDelta = (decimal)baseValue;

            return majorDelta;
        }

        #endregion

        #region IsThousandSeparatorVisible

        public static readonly DependencyProperty IsThousandSeparatorVisibleProperty = DependencyProperty.Register("IsThousandSeparatorVisible", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false, OnIsThousandSeparatorVisibleChanged));

        public bool IsThousandSeparatorVisible
        {
            get => (bool)this.GetValue(IsThousandSeparatorVisibleProperty);
            set => this.SetValue(IsThousandSeparatorVisibleProperty, value);
        }

        private static void OnIsThousandSeparatorVisibleChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is NumericUpDown control)
                control.InvalidateProperty(ValueProperty);
        }

        #endregion

        #region IsAutoSelectionActive

        public static readonly DependencyProperty IsAutoSelectionActiveProperty = DependencyProperty.Register("IsAutoSelectionActive", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        public bool IsAutoSelectionActive
        {
            get => (bool)this.GetValue(IsAutoSelectionActiveProperty);
            set => this.SetValue(IsAutoSelectionActiveProperty, value);
        }

        #endregion

        #region IsValueWrapAllowed

        public static readonly DependencyProperty IsValueWrapAllowedProperty = DependencyProperty.Register("IsValueWrapAllowed", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        public bool IsValueWrapAllowed
        {
            get => (bool)this.GetValue(IsValueWrapAllowedProperty);
            set => this.SetValue(IsValueWrapAllowedProperty, value);
        }

        #endregion

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NumericUpDown), new(new CornerRadius(0)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }

        #endregion CornerRadius

        #endregion

        #region Fields

        protected readonly CultureInfo Culture;
        protected RepeatButton DecreaseButton;
        protected RepeatButton IncreaseButton;
        protected TextBox TextBox;

        #endregion

        #region Commands

        private readonly RoutedUICommand _minorDecreaseValueCommand = new("MinorDecreaseValue", "MinorDecreaseValue", typeof(NumericUpDown));
        private readonly RoutedUICommand _minorIncreaseValueCommand = new("MinorIncreaseValue", "MinorIncreaseValue", typeof(NumericUpDown));
        private readonly RoutedUICommand _majorDecreaseValueCommand = new("MajorDecreaseValue", "MajorDecreaseValue", typeof(NumericUpDown));
        private readonly RoutedUICommand _majorIncreaseValueCommand = new("MajorIncreaseValue", "MajorIncreaseValue", typeof(NumericUpDown));
        private readonly RoutedUICommand _updateValueStringCommand = new("UpdateValueString", "UpdateValueString", typeof(NumericUpDown));
        private readonly RoutedUICommand _cancelChangesCommand = new("CancelChanges", "CancelChanges", typeof(NumericUpDown));

        #endregion

        #region Constructors

        static NumericUpDown() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

#       pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NumericUpDown()
#       pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            Culture.NumberFormat.NumberDecimalDigits = this.DecimalPlaces;

            Loaded += this.OnLoaded;
        }

        #endregion

        #region Event handlers

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AttachToVisualTree();
            this.AttachCommands();
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs) => this.UpdateValue();

        private void TextBoxOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (this.IsAutoSelectionActive)
                TextBox.SelectAll();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) => this.InvalidateProperty(ValueProperty);

        private void ButtonOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs) => this.Value = 0;

        #endregion

        #region Utility Methods

        #region Attachment

        private void AttachToVisualTree()
        {
            this.AttachTextBox();
            this.AttachIncreaseButton();
            this.AttachDecreaseButton();
        }

        private void AttachTextBox()
        {
            // A null check is advised
            if (this.GetTemplateChild("PART_TextBox") is TextBox textBox)
            {
                TextBox = textBox;
                TextBox.LostFocus += this.TextBoxOnLostFocus;
                TextBox.PreviewMouseLeftButtonUp += this.TextBoxOnPreviewMouseLeftButtonUp;

                TextBox.UndoLimit = 1;
                TextBox.IsUndoEnabled = true;
            }
        }

        private void AttachIncreaseButton()
        {
            if (this.GetTemplateChild("PART_IncreaseButton") is RepeatButton increaseButton)
            {
                IncreaseButton = increaseButton;
                IncreaseButton.Focusable = false;
                IncreaseButton.Command = _minorIncreaseValueCommand;
                IncreaseButton.PreviewMouseLeftButtonDown += (sender, args) => this.RemoveFocus();
                IncreaseButton.PreviewMouseRightButtonDown += this.ButtonOnPreviewMouseRightButtonDown;
            }
        }

        private void AttachDecreaseButton()
        {
            if (this.GetTemplateChild("PART_DecreaseButton") is RepeatButton decreaseButton)
            {
                DecreaseButton = decreaseButton;
                DecreaseButton.Focusable = false;
                DecreaseButton.Command = _minorDecreaseValueCommand;
                DecreaseButton.PreviewMouseLeftButtonDown += (sender, args) => this.RemoveFocus();
                DecreaseButton.PreviewMouseRightButtonDown += this.ButtonOnPreviewMouseRightButtonDown;
            }
        }

        private void AttachCommands()
        {
            _ = this.CommandBindings.Add(new CommandBinding(_minorIncreaseValueCommand, (a, b) => this.IncreaseValue(true)));
            _ = this.CommandBindings.Add(new CommandBinding(_minorDecreaseValueCommand, (a, b) => this.DecreaseValue(true)));
            _ = this.CommandBindings.Add(new CommandBinding(_majorIncreaseValueCommand, (a, b) => this.IncreaseValue(false)));
            _ = this.CommandBindings.Add(new CommandBinding(_majorDecreaseValueCommand, (a, b) => this.DecreaseValue(false)));
            _ = this.CommandBindings.Add(new CommandBinding(_updateValueStringCommand, (a, b) => this.UpdateValue()));
            _ = this.CommandBindings.Add(new CommandBinding(_cancelChangesCommand, (a, b) => this.CancelChanges()));

            _ = TextBox.InputBindings.Add(new KeyBinding(_minorIncreaseValueCommand, new KeyGesture(Key.Up)));
            _ = TextBox.InputBindings.Add(new KeyBinding(_minorDecreaseValueCommand, new KeyGesture(Key.Down)));
            _ = TextBox.InputBindings.Add(new KeyBinding(_majorIncreaseValueCommand, new KeyGesture(Key.PageUp)));
            _ = TextBox.InputBindings.Add(new KeyBinding(_majorDecreaseValueCommand, new KeyGesture(Key.PageDown)));
            _ = TextBox.InputBindings.Add(new KeyBinding(_updateValueStringCommand, new KeyGesture(Key.Enter)));
            _ = TextBox.InputBindings.Add(new KeyBinding(_cancelChangesCommand, new KeyGesture(Key.Escape)));
        }

        #endregion

        #region Data retrieval and deposit

        private static decimal ParseStringToDecimal(string source)
        {
            _ = decimal.TryParse(source, out decimal value);

            return value;
        }

        public int GetDecimalPlacesCount(string valueString) => valueString.SkipWhile(c => c.ToString(Culture) != Culture.NumberFormat.NumberDecimalSeparator).Skip(1).Count();

        private decimal TruncateValue(string valueString, int decimalPlaces)
        {
            int endPoint = valueString.Length - (decimalPlaces - this.DecimalPlaces);
            endPoint++;

            string? tempValueString = valueString[..endPoint];

            return decimal.Parse(tempValueString, Culture);
        }

        #endregion

        #region SubCoercion

        private void CoerceValueToBounds(ref decimal value)
        {
            if (value < this.MinValue)
                value = this.MinValue;
            else if (value > this.MaxValue)
            {
                value = this.MaxValue;
            }
        }

        #endregion

        #endregion

        #region Methods

        private void UpdateValue() => this.Value = ParseStringToDecimal(TextBox.Text);

        private void CancelChanges() => _ = TextBox.Undo();

        private void RemoveFocus()
        {
            // Passes focus here and then just deletes it
            this.Focusable = true;
            _ = this.Focus();
            this.Focusable = false;
        }

        private void IncreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            decimal value = ParseStringToDecimal(TextBox.Text);

            // Coerce the value to min/max
            this.CoerceValueToBounds(ref value);

            // Only change the value if it has any meaning
            if (value >= this.MinValue)
            {
                if (minor)
                {
                    if (this.IsValueWrapAllowed && value + this.MinorDelta > this.MaxValue)
                        value = this.MinValue;
                    else
                    {
                        value += this.MinorDelta;
                    }
                }
                else
                {
                    if (this.IsValueWrapAllowed && value + this.MajorDelta > this.MaxValue)
                        value = this.MinValue;
                    else
                    {
                        value += this.MajorDelta;
                    }
                }
            }

            this.Value = value;
        }

        private void DecreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            decimal value = ParseStringToDecimal(TextBox.Text);

            // Coerce the value to min/max
            this.CoerceValueToBounds(ref value);

            // Only change the value if it has any meaning
            if (value <= this.MaxValue)
            {
                if (minor)
                {
                    if (this.IsValueWrapAllowed && value - this.MinorDelta < this.MinValue)
                        value = this.MaxValue;
                    else
                    {
                        value -= this.MinorDelta;
                    }
                }
                else
                {
                    if (this.IsValueWrapAllowed && value - this.MajorDelta < this.MinValue)
                        value = this.MaxValue;
                    else
                    {
                        value -= this.MajorDelta;
                    }
                }
            }

            this.Value = value;
        }

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}