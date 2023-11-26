using PropertyChanged;
using radj307.IconExtractor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VolumeControl.TypeExtensions;

namespace VolumeControl.WPF.CustomMessageBox
{
    public class CustomAdorner<T> : CustomAdorner where T : UIElement
    {
        public CustomAdorner(T adornedElement, Brush? fill, Pen? stroke, DrawMethod drawMethod) : base(adornedElement, fill, stroke, null!)
        {
            Draw = drawMethod;
        }

        #region Delegate Definitions
        /// <summary>
        /// Represents a method that uses the provided <paramref name="drawingContext"/> to draw the specified <paramref name="adorner"/>.
        /// </summary>
        /// <param name="adorner">The <see cref="CustomAdorner{T}"/> instance to draw.</param>
        /// <param name="drawingContext">The <see cref="DrawingContext"/> object to use for drawing the <paramref name="adorner"/>.</param>
        public new delegate void DrawMethod(CustomAdorner<T> adorner, DrawingContext drawingContext);
        #endregion Delegate Definitions

        /// <summary>
        /// Gets the <typeparamref name="T"/> instance that this <see cref="CustomAdorner{T}"/> is bound to.
        /// </summary>
        /// <inheritdoc cref="Adorner.AdornedElement"/>
        public new T AdornedElement => (T)base.AdornedElement;
        /// <inheritdoc cref="CustomAdorner.Draw"/>
        public new DrawMethod Draw { get; set; }

        public static void DrawRectangleAdorner(CustomAdorner<T> adorner, DrawingContext drawingContext)
            => drawingContext.DrawRectangle(adorner.Fill, adorner.Stroke, new Rect(adorner.AdornedElement.RenderSize));

        #region (Override) OnRender
        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext) => Draw(this, drawingContext);
        #endregion (Override) OnRender

        #region Show
        public static CustomAdorner<T> Show(T uiElement, Brush? fill, Pen? stroke, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner<T>(uiElement, fill, stroke, drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        public static CustomAdorner<T> Show(T uiElement, Brush stroke, double strokeThickness, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner<T>(uiElement, null, new Pen(stroke, strokeThickness), drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        public static CustomAdorner<T> Show(T uiElement, Color strokeColor, double strokeThickness, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner<T>(uiElement, null, new Pen(new SolidColorBrush(strokeColor), strokeThickness), drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        #endregion Show
    }
    public class CustomAdorner : Adorner
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="CustomAdorner"/> instance with the specified <paramref name="adornedElement"/>, <paramref name="fill"/>, <paramref name="stroke"/>, and <paramref name="drawMethod"/>.
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="fill">The brush to use for filling the adornment.</param>
        /// <param name="stroke">The pen to use for the border of the adornment.</param>
        /// <param name="drawMethod">The <see cref="DrawMethod"/> method to use for drawing the adorner.</param>
        public CustomAdorner(UIElement adornedElement, Brush? fill, Pen? stroke, DrawMethod drawMethod) : base(adornedElement)
        {
            Fill = fill;
            Stroke = stroke;
            Draw = drawMethod;
        }
        #endregion Constructor

        #region Delegate Definitions
        /// <summary>
        /// Represents a method that uses the provided <paramref name="drawingContext"/> to draw the specified <paramref name="adorner"/>.
        /// </summary>
        /// <param name="adorner">The <see cref="CustomAdorner"/> instance to draw.</param>
        /// <param name="drawingContext">The <see cref="DrawingContext"/> object to use for drawing the <paramref name="adorner"/>.</param>
        public delegate void DrawMethod(CustomAdorner adorner, DrawingContext drawingContext);
        #endregion Delegate Definitions

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to use for filling the adornment shape.
        /// </summary>
        public virtual Brush? Fill { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Pen"/> to use for the adornment shape's border.
        /// </summary>
        public virtual Pen? Stroke { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="DrawMethod"/> method to use for drawing the adornment.
        /// </summary>
        public virtual DrawMethod Draw { get; set; }
        #endregion Properties

        #region Default DrawCustomAdorner Methods
        /// <summary>
        /// Draws a rectangle using the AdornedElement's RenderSize.
        /// </summary>
        /// <param name="adorner"></param>
        /// <param name="drawingContext"></param>
        public static void DrawRectangleAdorner(CustomAdorner adorner, DrawingContext drawingContext)
            => drawingContext.DrawRectangle(adorner.Fill, adorner.Stroke, new Rect(adorner.AdornedElement.RenderSize));
        #endregion Default DrawCustomAdorner Methods

        #region (Override) OnRender
        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext) => Draw(this, drawingContext);
        #endregion (Override) OnRender

        #region Show
        public static CustomAdorner Show(UIElement uiElement, Brush? fill, Pen? stroke, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner(uiElement, fill, stroke, drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        public static CustomAdorner Show(UIElement uiElement, Brush stroke, double strokeThickness, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner(uiElement, null, new Pen(stroke, strokeThickness), drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        public static CustomAdorner Show(UIElement uiElement, Color strokeColor, double strokeThickness, DrawMethod? drawMethod = null)
        {
            var inst = new CustomAdorner(uiElement, null, new Pen(new SolidColorBrush(strokeColor), strokeThickness), drawMethod ?? DrawRectangleAdorner);
            AdornerLayer.GetAdornerLayer(uiElement).Add(inst); //< apply the adorner
            return inst;
        }
        #endregion Show
    }

    /// <summary>
    /// Defines the appearance of a <see cref="CustomMessageBox"/> window.
    /// </summary>
    public sealed class CustomMessageBoxStyle : INotifyPropertyChanged
    {
        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        public static readonly CustomMessageBoxStyle Default = new();
        /// <summary>
        /// The minimum allowed value for <see cref="MaxWidth"/>. Values below this number cannot be set.
        /// </summary>
        public const double MinimumAllowedMaxWidthValue = 30.0;
        #endregion Fields

        #region Properties
        // main / misc
        public Brush BackgroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
        public Brush ForegroundBrush { get; set; } = Brushes.WhiteSmoke;
        public Brush BorderBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50));
        public Thickness BorderThickness { get; set; } = new(3);
        public CornerRadius CornerRadius { get; set; } = new(5);
        /// <summary>
        /// Gets or sets the maximum width of the <see cref="CustomMessageBox"/> window. The actual width of the window cannot be less than 100.
        /// </summary>
        /// <remarks>
        /// Setting this to a non-<see langword="null"/> value will unset <see cref="MaxWidthScreenPercentage"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value was less than 0.</exception>
        public double? MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxWidth)} expects a value greater than 0; actual value was {value}");
                else if (value < MinimumAllowedMaxWidthValue)
                    value = MinimumAllowedMaxWidthValue;

                _maxWidth = value;

                // if MaxWidth was set to non-null value, unset MaxWidthScreenPercentage:
                if (_maxWidth != null) MaxWidthScreenPercentage = null;

                NotifyPropertyChanged();
            }
        }
        private double? _maxWidth;
        /// <summary>
        /// Gets or sets the maximum width of the <see cref="CustomMessageBox"/> window, as a percentage of the width of the screen it was initially displayed on, in the range 0.0 - 1.0.
        /// </summary>
        /// <remarks>
        /// Setting this to a non-<see langword="null"/> value will unset <see cref="MaxWidth"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value was less than 0 or greater than 1.</exception>
        /// <returns>The percentage of the screen width that the window can use, where 1.0 is 100%.</returns>
        public double? MaxWidthScreenPercentage
        {
            get => _maxWidthScreenPercentage;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxWidthScreenPercentage)} expects a value within the range 0.0-1.0; actual value was {value}");

                _maxWidthScreenPercentage = value;

                // if MaxWidthScreenPercentage was set to non-null value, unset MaxWidth:
                if (_maxWidthScreenPercentage != null) MaxWidth = null;

                NotifyPropertyChanged();
            }
        }
        private double? _maxWidthScreenPercentage = 0.4;

        // caption
        public Brush CaptionBackgroundBrush { get; set; } = Brushes.Transparent;
        public Brush CaptionForegroundBrush { get; set; } = Brushes.LightGray;
        public Brush CaptionButtonMouseOverBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
        public Brush CaptionButtonPressedBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0x44, 0xFF, 0xFF, 0xFF));
        public double CaptionFontSize { get; set; } = 14.0;
        public double CaptionHeight { get; set; } = 22.0;

        // buttons
        public Brush ButtonBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        public Brush ButtonForegroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
        public Brush ButtonMouseOverBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
        public Brush ButtonPressedBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
        public Brush ButtonBorderBrush { get; set; } = Brushes.Transparent;
        public Brush ButtonBorderBrushFocused { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0xBB, 0xEE));
        public Thickness ButtonBorderThickness { get; set; } = new(1.5);
        public CornerRadius ButtonCornerRadius { get; set; } = new(5);
        /// <summary>
        /// Gets or sets the horizontal alignment of the buttons panel.
        /// </summary>
        public HorizontalAlignment ButtonPanelAlignment { get; set; } = HorizontalAlignment.Right;
        public double ButtonFontSize { get; set; } = 12.0;
        public FontWeight ButtonFontWeight { get; set; } = FontWeights.Bold;
        public Thickness ButtonPadding { get; set; } = new(6, 2.5, 6, 2.5);

        // content
        /// <summary>
        /// Gets or sets the side of the window that the content panel is docked to.
        /// </summary>
        public Dock ContentPanelDock { get; set; } = Dock.Bottom;
        public HorizontalAlignment ContentPanelAlignment { get; set; } = HorizontalAlignment.Stretch;
        public double ContentFontSize { get; set; } = 12.0;

        // message
        /// <summary>
        /// Gets or sets the text alignment of the Message text.
        /// </summary>
        public TextAlignment MessageTextAlignment { get; set; } = TextAlignment.Left;
        /// <summary>
        /// Gets or sets the font size of the Message text.
        /// </summary>
        public double MessageFontSize { get; set; } = 12.0;
        #endregion Properties
    }
    /// <summary>
    /// Data model for <see cref="CustomMessageBox"/> windows.
    /// </summary>
    public sealed class CustomMessageBoxData : INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance without specifying any parameters.
        /// </summary>
        public CustomMessageBoxData() { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        public CustomMessageBoxData(Window owner)
        {
            Owner = owner;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        public CustomMessageBoxData(Window owner, object content)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, and <paramref name="caption"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Buttons.AddRange(buttons.Where(b => b != null));

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, ImageSource icon, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Icon = icon;
            Buttons.AddRange(buttons.Where(b => b != null));

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxImage icon, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));
            Buttons.AddRange(buttons.Where(b => b != null));

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(Window owner, object content, string caption, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Buttons.AddRange(buttons.Where(b => b != null));
            DefaultResult = Buttons.FirstOrDefault(b => b.Equals(defaultResult)) ?? throw new ArgumentOutOfRangeException(nameof(defaultResult), defaultResult, $"The specified {nameof(defaultResult)} string does not match the name of any of the specified {nameof(buttons)}!");

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(Window owner, object content, string caption, ImageSource icon, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Icon = icon;
            Buttons.AddRange(buttons.Where(b => b != null));
            DefaultResult = Buttons.FirstOrDefault(b => b.Equals(defaultResult)) ?? throw new ArgumentOutOfRangeException(nameof(defaultResult), defaultResult, $"The specified {nameof(defaultResult)} string does not match the name of any of the specified {nameof(buttons)}!");

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxImage icon, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));
            Buttons.AddRange(buttons.Where(b => b != null));
            DefaultResult = Buttons.FirstOrDefault(b => b.Equals(defaultResult)) ?? throw new ArgumentOutOfRangeException(nameof(defaultResult), defaultResult, $"The specified {nameof(defaultResult)} string does not match the name of any of the specified {nameof(buttons)}!");

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, and <paramref name="icon"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, ImageSource icon)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = icon;

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, ImageSource icon, MessageBoxResult defaultResult)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = icon;
            DefaultResult = Enum.GetName(defaultResult);

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, and <paramref name="icon"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, MessageBoxImage icon)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));
            DefaultResult = Enum.GetName(defaultResult);

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        /// <param name="options">Specifies special display options for the <see cref="CustomMessageBox"/> window.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult, CustomMessageBoxOptions options)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));
            DefaultResult = Enum.GetName(defaultResult);
            Options = options;

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="owner"/>, <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Window"/> to set as the owner of the <see cref="CustomMessageBox"/> dialog window.</param>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        /// <param name="options">Specifies special display options for the <see cref="CustomMessageBox"/> window. <see cref="MessageBoxOptions.DefaultDesktopOnly"/> &amp; <see cref="MessageBoxOptions.ServiceNotification"/> are not supported.</param>
        public CustomMessageBoxData(Window owner, object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            Owner = owner;
            if (content is string message)
                Message = message;
            else Content = content;
            Title = caption;
            MessageBoxButton = buttons;
            Icon = IconExtractor.ExtractFromHandle(GetIconHandle(icon));
            DefaultResult = Enum.GetName(defaultResult);
            MessageBoxOptions = options;

            Buttons.CollectionChanged += this.Buttons_CollectionChanged;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance without specifying any parameters.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        public CustomMessageBoxData(object content) : this(owner: null!, content) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance without specifying any parameters.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        public CustomMessageBoxData(object content, string caption) : this(owner: null!, content, caption) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, ImageSource icon, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, icon, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxImage icon, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, icon, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(object content, string caption, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, defaultResult, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(object content, string caption, ImageSource icon, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, icon, defaultResult, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="icon"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox. This can be a <see cref="string"/>, <see cref="char"/>, <see cref="MessageBoxResult"/>, or <see cref="CustomMessageBoxButton"/>, but it must match the Name of one of the specified <paramref name="buttons"/> or an <see cref="ArgumentOutOfRangeException"/> is thrown. It does not have to be the same instance.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="defaultResult"/> didn't match any of the <paramref name="buttons"/>.</exception>
        public CustomMessageBoxData(object content, string caption, MessageBoxImage icon, CustomMessageBoxButton? defaultResult, params CustomMessageBoxButton?[] buttons) : this(owner: null!, content, caption, icon, defaultResult, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified  <paramref name="content"/>, <paramref name="caption"/>, and <paramref name="buttons"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons) : this(owner: null!, content, caption, buttons) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, and <paramref name="icon"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, ImageSource icon) : this(owner: null!, content, caption, buttons, icon) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, ImageSource icon, MessageBoxResult defaultResult) : this(owner: null!, content, caption, buttons, icon, defaultResult) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, and <paramref name="icon"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, MessageBoxImage icon) : this(owner: null!, content, caption, buttons, icon) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult) : this(owner: null!, content, caption, buttons, icon, defaultResult) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        /// <param name="options">Specifies special display options for the <see cref="CustomMessageBox"/> window.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult, CustomMessageBoxOptions options) : this(owner: null!, content, caption, buttons, icon, defaultResult, options) { }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxData"/> instance with the specified <paramref name="content"/>, <paramref name="caption"/>, <paramref name="buttons"/>, <paramref name="icon"/>, and <paramref name="defaultResult"/>.
        /// </summary>
        /// <param name="content">The content to display in the message box. This can be any type, including <see cref="string"/> &amp; <see cref="System.Windows.Controls.Control"/>.<br/>When a <see cref="string"/> is specified, the Message property is set instead of the Content property.</param>
        /// <param name="caption">The title of the window.</param>
        /// <param name="buttons">The buttons to display in the window.</param>
        /// <param name="icon">The icon to display in the window.</param>
        /// <param name="defaultResult">The default result of the messagebox.</param>
        /// <param name="options">Specifies special display options for the <see cref="CustomMessageBox"/> window. <see cref="MessageBoxOptions.DefaultDesktopOnly"/> &amp; <see cref="MessageBoxOptions.ServiceNotification"/> are not supported.</param>
        public CustomMessageBoxData(object content, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) : this(owner: null!, content, caption, buttons, icon, defaultResult, options) { }
        #endregion Constructors

        #region Fields
        private bool _isSettingButtons = false;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="CustomMessageBoxStyle"/> instance that defines the appearance &amp; layout of the <see cref="CustomMessageBox"/>.
        /// </summary>
        public CustomMessageBoxStyle Appearance { get; set; } = CustomMessageBoxStyle.Default;
        /// <summary>
        /// Gets the owner window.
        /// </summary>
        public Window? Owner { get; set; }
        /// <summary>
        /// Gets or sets the message string to display in the <see cref="CustomMessageBox"/>.
        /// </summary>
        /// <remarks>
        /// This can be specified at the same time as the Content property.
        /// </remarks>
        public string? Message { get; set; }
        /// <summary>
        /// Gets or sets the extra content (such as controls) to display in the <see cref="CustomMessageBox"/>.
        /// </summary>
        /// <remarks>
        /// This can be specified at the same time as the Content property.
        /// </remarks>
        public object? Content { get; set; }
        /// <summary>
        /// Gets or sets the title of the <see cref="CustomMessageBox"/> window.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether the caption bar is visible or not.
        /// </summary>
        /// <remarks>
        /// When <see langword="false"/>, the Title &amp; Icon properties are ignored. The close button will not be visible either.
        /// </remarks>
        public bool ShowCaptionBar { get; set; } = true;
        /// <summary>
        /// Gets or sets the icon to display in the <see cref="CustomMessageBox"/> window.
        /// </summary>
        public ImageSource? Icon { get; set; }
        /// <summary>
        /// Gets or sets the buttons to show in the <see cref="CustomMessageBox"/> window.
        /// </summary>
        public ObservableCollection<CustomMessageBoxButton> Buttons { get; } = new();
        /// <summary>
        /// Gets or sets the buttons to show in the <see cref="CustomMessageBox"/> window, as a legacy enum value.
        /// </summary>
        /// <remarks>
        /// Setting this will overwrite the <see cref="Buttons"/> if there are any.
        /// </remarks>
        public MessageBoxButton? MessageBoxButton
        {
            get => _messageBoxButton;
            set
            {
                _messageBoxButton = value;

                _isSettingButtons = true;
                Buttons.Clear();
                if (_messageBoxButton.HasValue)
                {
                    Buttons.AddRange(GetMessageBoxButtons(_messageBoxButton.Value));
                }
                _isSettingButtons = false;

                NotifyPropertyChanged();
            }
        }
        private MessageBoxButton? _messageBoxButton;
        /// <summary>
        /// Gets or sets the default button for the <see cref="CustomMessageBox"/>.
        /// </summary>
        public CustomMessageBoxButton? DefaultResult { get; set; }
        /// <summary>
        /// Gets the button that the user pressed.
        /// </summary>
        /// <returns>The <see cref="CustomMessageBoxButton"/> instance that was clicked by the user, or <see langword="null"/> if the user didn't click a button.</returns>
        public CustomMessageBoxButton? Result => _result ?? DefaultResult;
        private CustomMessageBoxButton? _result;
        /// <summary>
        /// Gets or sets the special display options for the <see cref="CustomMessageBox"/>.
        /// </summary>
        public CustomMessageBoxOptions Options
        {
            get => _options;
            set
            {
                _options = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(MessageBoxOptions));
            }
        }
        private CustomMessageBoxOptions _options = CustomMessageBoxOptions.None;
        /// <summary>
        /// Gets or sets the special display options for the <see cref="CustomMessageBox"/> as the legacy enum type.
        /// </summary>
        /// <remarks>
        /// <see cref="MessageBoxOptions.DefaultDesktopOnly"/> &amp; <see cref="MessageBoxOptions.ServiceNotification"/> are not supported and will trigger an <see cref="InvalidOperationException"/> if you attempt to set them.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The specified value included <see cref="MessageBoxOptions.DefaultDesktopOnly"/> and/or <see cref="MessageBoxOptions.ServiceNotification"/>, which are not supported.</exception>
        public MessageBoxOptions MessageBoxOptions
        {
            get => (MessageBoxOptions)Options;
            set => Options = GetCustomMessageBoxOptions(value); //< don't notify PropertyChanged since Options notifies for us
        }
        /// <summary>
        /// Gets or sets whether pressing the Escape key will close the window (with the default result) or not.
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public bool EnableEscapeKey { get; set; } = false;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods

        #region (Private/Static) GetIconHandle
        private static IntPtr GetIconHandle(MessageBoxImage icon)
            => icon switch
            {
                // Error|Hand
                MessageBoxImage.Error => System.Drawing.SystemIcons.Error.Handle,
                // Question
                MessageBoxImage.Question => System.Drawing.SystemIcons.Question.Handle,
                // Exclamation|Warning
                MessageBoxImage.Exclamation => System.Drawing.SystemIcons.Exclamation.Handle,
                // Asterisk|Information
                MessageBoxImage.Asterisk => System.Drawing.SystemIcons.Asterisk.Handle,
                _ => throw new InvalidEnumArgumentException(nameof(icon), (int)icon, typeof(MessageBoxImage)),
            };
        #endregion (Private/Static) GetIconHandle

        #region (Private/Static) GetMessageBoxButtons
        private static CustomMessageBoxButton[] GetMessageBoxButtons(MessageBoxButton buttons)
            => buttons switch
            {
                System.Windows.MessageBoxButton.OK => new CustomMessageBoxButton[] { "OK" },
                System.Windows.MessageBoxButton.OKCancel => new CustomMessageBoxButton[] { "OK", "Cancel" },
                System.Windows.MessageBoxButton.YesNoCancel => new CustomMessageBoxButton[] { "Yes", "No", "Cancel" },
                System.Windows.MessageBoxButton.YesNo => new CustomMessageBoxButton[] { "Yes", "No" },
                _ => Array.Empty<CustomMessageBoxButton>(),
            };
        #endregion (Private/Static) GetMessageBoxButtons

        #region (Private/Static) GetCustomMessageBoxOptions
        private static CustomMessageBoxOptions GetCustomMessageBoxOptions(MessageBoxOptions messageBoxOptions, bool throwOnUnsupported = true)
        {
            if (messageBoxOptions == MessageBoxOptions.None)
                return CustomMessageBoxOptions.None;
            else if (throwOnUnsupported && (messageBoxOptions.HasFlag(MessageBoxOptions.DefaultDesktopOnly) || messageBoxOptions.HasFlag(MessageBoxOptions.ServiceNotification)))
                throw new InvalidOperationException($"The {nameof(MessageBoxOptions.DefaultDesktopOnly)} & {nameof(MessageBoxOptions.ServiceNotification)} flags are not supported by {typeof(CustomMessageBoxData).FullName}!");

            CustomMessageBoxOptions result = CustomMessageBoxOptions.None;

            if (messageBoxOptions.HasFlag(MessageBoxOptions.RightAlign))
                result |= CustomMessageBoxOptions.RightAlign;
            if (messageBoxOptions.HasFlag(MessageBoxOptions.RtlReading))
                result |= CustomMessageBoxOptions.RtlReading;

            return result;
        }
        #endregion (Private/Static) GetCustomMessageBoxOptions

        #region (Private) ShowSTA
        private void ShowSTA() => new CustomMessageBox(this) { Owner = Owner }.ShowDialog();
        #endregion (Private) ShowSTA

        #region (Internal) Show
        internal CustomMessageBoxButton? Show()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                ShowSTA();
            }
            else
            {
                var thread = new Thread(ShowSTA);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }

            return Result;
        }
        #endregion (Internal) Show

        #region (Internal) SetResult
        /// <summary>
        /// Sets the <see cref="Result"/> to the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The button that was clicked.</param>
        /// <exception cref="ArgumentNullException"/>
        internal void SetResult(CustomMessageBoxButton result)
        {
            ArgumentNullException.ThrowIfNull(result);

            _result = result;
            NotifyPropertyChanged(nameof(Result));
        }
        #endregion (Internal) SetResult

        #endregion Methods

        #region EventHandlers

        #region Buttons
        private void Buttons_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        { // set the MessageBoxButtons property to null
            if (_isSettingButtons) return;

            MessageBoxButton = null;
        }
        #endregion Buttons

        #endregion EventHandlers
    }
}
