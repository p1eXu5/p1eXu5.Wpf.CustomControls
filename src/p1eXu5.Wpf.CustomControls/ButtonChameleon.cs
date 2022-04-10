/*
 * Changing the IsEnabled property causes the OnRender mehod to be called and disables the control events
 */
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using p1eXu5.Wpf.CustomControls.Helpers;

namespace p1eXu5.Wpf.CustomControls
{
    public sealed class ButtonChameleon : Decorator
    {
        #region Fields

        private const byte HOVER_PERCENT = 12;
        private const byte PRESSED_PERCENT = 25;

        private static readonly Duration _hoverDuration = new Duration( TimeSpan.FromSeconds( 0.3 ) );
        private static readonly Duration _pressedDuration = new Duration( TimeSpan.FromSeconds( 0.1 ) );
        
        private ColorHeights[] _colorHeights;

        #endregion


        #region DynamicProperties

            #region BackgroundProperty

            /// <summary>
            /// DependencyProperty for <see cref="Background" /> property.
            /// </summary>
            public static readonly DependencyProperty BackgroundProperty =
                        Control.BackgroundProperty.AddOwner(
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                        null,
                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                        OnBackgroundPropertyChanged));

            /// <summary>
            /// The Background property defines the brush used to fill the background of the button.
            /// </summary>
            public Brush Background
            {
                get => (Brush)GetValue(BackgroundProperty);
                set => SetValue(BackgroundProperty, value);
            }

            private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                Trace.WriteLine($"OnBackground thread id: {Thread.CurrentThread.ManagedThreadId}");

                ButtonChameleon chameleon = ( ButtonChameleon )d;


                chameleon._normalBackgroundBrush = ( Brush )e.NewValue;

                if ( chameleon._normalBackgroundBrush is SolidColorBrush brush ) {

                    if ( brush.Color.GetColorHeight() == ColorHeights.Higher ) {
                        chameleon._mouseHoverBackgroundBrush = new SolidColorBrush( brush.Color.ToDarken( HOVER_PERCENT ) );
                        chameleon._pressedBackgroundBrush = new SolidColorBrush( brush.Color.ToDarken( PRESSED_PERCENT ) );
                    }
                    else {
                        chameleon._mouseHoverBackgroundBrush = new SolidColorBrush(brush.Color.ToBrighten(HOVER_PERCENT) );
                        chameleon._pressedBackgroundBrush = new SolidColorBrush(brush.Color.ToBrighten(PRESSED_PERCENT)) ;
                    }

                    chameleon._disabledBackgroundBrush = new SolidColorBrush( brush.Color.Desaturate() );

                    chameleon._mouseHoverBackgroundBrush.Freeze();
                    chameleon._pressedBackgroundBrush.Freeze();
                    chameleon._disabledBackgroundBrush.Freeze();
                }
            }

            #endregion


            #region HoveredBackgroundProperty

            public static readonly DependencyProperty HoveredBackgroundProperty =
                        DependencyProperty.RegisterAttached(
                                "HoveredBackground",
                                typeof( Brush ),
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                    null,
                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits ));

            public Brush HoveredBackground
            {
                get => (Brush)GetValue(HoveredBackgroundProperty);
                set => SetValue(HoveredBackgroundProperty, value);
            }

            public static void SetHoveredBackground( DependencyObject element, Brush value )
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof( element ));
                }

                element.SetValue(HoveredBackgroundProperty, value);
            }

            public static Brush GetHoveredBackground(DependencyObject element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                return (Brush)element.GetValue(HoveredBackgroundProperty);
            }

            #endregion


            #region PressedBackgroundProperty

            public static readonly DependencyProperty PressedBackgroundProperty =
                        DependencyProperty.RegisterAttached(
                                "PressedBackground",
                                typeof(Brush),
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                    null,
                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits ));

            public Brush PressedBackground
            {
                get => (Brush)GetValue(PressedBackgroundProperty);
                set => SetValue(PressedBackgroundProperty, value);
            }

            public static void SetPressedBackground(DependencyObject element, Brush value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                element.SetValue(PressedBackgroundProperty, value);
            }

            public static Brush GetPressedBackground(DependencyObject element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                return (Brush)element.GetValue(PressedBackgroundProperty);
            }

            #endregion


            #region BorderBrushProperty

            /// <summary>
            /// DependencyProperty for <see cref="BorderBrush" /> property.
            /// </summary>
            public static readonly DependencyProperty BorderBrushProperty =
                    Border.BorderBrushProperty.AddOwner(
                            typeof(ButtonChameleon),
                            new FrameworkPropertyMetadata(
                                    null,
                                    FrameworkPropertyMetadataOptions.AffectsRender));

            /// <summary>
            /// The BorderBrush property defines the brush used to draw the outer border.
            /// </summary>
            public Brush BorderBrush
            {
                get => (Brush)GetValue(BorderBrushProperty);
                set => SetValue(BorderBrushProperty, value);
            }

            #endregion


            #region CornerRadiusProperty

            public static readonly DependencyProperty CornerRadiusProperty =
                Border.CornerRadiusProperty.AddOwner(typeof(ButtonChameleon),
                                                      new FrameworkPropertyMetadata(default(CornerRadius),
                                                                                     FrameworkPropertyMetadataOptions.AffectsRender));

            public CornerRadius CornerRadius
            {
                get => (CornerRadius)GetValue(CornerRadiusProperty);
                set => SetValue(CornerRadiusProperty, value);
            }

            #endregion


            #region RenderMouseOverProperty

            /// <summary>
            /// DependencyProperty for <see cref="RenderMouseOver" /> property.
            /// </summary>
            public static readonly DependencyProperty RenderMouseOverProperty =
                     DependencyProperty.Register("RenderMouseOver",
                             typeof(bool),
                             typeof(ButtonChameleon),
                             new FrameworkPropertyMetadata(
                                    false,
                                    new PropertyChangedCallback(OnRenderMouseOverChanged)));

            /// <summary>
            /// When true the chrome renders with a mouse over look.
            /// </summary>
            public bool RenderMouseOver
            {
                get => (bool)GetValue(RenderMouseOverProperty);
                set => SetValue(RenderMouseOverProperty, value);
            }

            private static void OnRenderMouseOverChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                ButtonChameleon chameleon = ((ButtonChameleon)o);

                if ( Animates )
                {
                    if ( !chameleon.RenderPressed ) 
                    {
                        if ( (bool)e.NewValue ) 
                        {
                            if ( chameleon._localResouces == null ) 
                            {
                                chameleon._localResouces = new LocalResouces();
                                // ставит в очередь перерисовку
                                chameleon.InvalidateVisual();
                            }

                            var parameters = chameleon.GetHoverAnimationParameters( _hoverDuration );
                            AnimateBrushColor( parameters );
                            AnimateBrushOpacity( parameters.From, _hoverDuration );
                        }
                        // Mouse is not hovered
                        else if (chameleon._localResouces == null ) {
                            chameleon.InvalidateVisual();
                        }
                        else {
                            // TODO to normal state animations:
                            AnimateBackgroundToNormal( chameleon, _pressedDuration );
                        }
                    }
                }
                else {
                    chameleon._localResouces = null;
                    chameleon.InvalidateVisual();
                }

                chameleon.Foreground = chameleon.ForegroundOverlay;
            }



            private static void AnimateBackgroundToNormal( ButtonChameleon chameleon, Duration duration )
            {
                DoubleAnimation da = new DoubleAnimation( 0, duration);

                var bo = chameleon.BackgroundOverlay;

                if ( bo is SolidColorBrush ) {
                    Color c = (( SolidColorBrush )chameleon.NormalBackgroundBrush).Color;
                    ColorAnimation ca = new ColorAnimation( c, duration);
                    ca.Completed += CaOnCompleted;

                    chameleon.BeginAnimation( SolidColorBrush.ColorProperty, ca );

                    void CaOnCompleted( object sender, EventArgs args )
                    {
                        ca.Completed -= CaOnCompleted;
                        (( SolidColorBrush )bo).Color = c;
                    }
                }

                bo.BeginAnimation( Brush.OpacityProperty, da );
            }

            private static void AnimateBrushOpacity( Brush brush, Duration duration )
            {
                DoubleAnimation da = new DoubleAnimation( 1, duration);
                brush.BeginAnimation( Brush.OpacityProperty, da );
            }

            private static void AnimateBrushColor( BrushAnimationPatameters parameters )
            {
                if ( parameters.To == null || parameters.To.GetType() != parameters.From.GetType() ) {
                    if ( parameters.From is SolidColorBrush ) {

                        var normalColor = ((SolidColorBrush)parameters.Default).Color;

                        Color c = parameters.ColorHeights[0] == ColorHeights.Higher
                          ? normalColor.ToDarken( parameters.DefaultPercent )
                          : normalColor.ToBrighten( parameters.DefaultPercent );

                        parameters.From.BeginAnimation( SolidColorBrush.ColorProperty, new ColorAnimation(c, parameters.Duration ) );
                    }
                    else if ( parameters.From is GradientBrush ) {

                    }
                }
                else if (parameters.To is SolidColorBrush to)
                {
                    parameters.From.BeginAnimation(
                        SolidColorBrush.ColorProperty, 
                        new ColorAnimation( ((SolidColorBrush)parameters.To).Color, parameters.Duration));
                }
                else if ( parameters.To is GradientBrush from ) {

                }

            }

            #endregion


            #region RenderPressedProperty

            /// <summary>
            /// DependencyProperty for <see cref="RenderPressed" /> property.
            /// </summary>
            public static readonly DependencyProperty RenderPressedProperty =
                     DependencyProperty.Register("RenderPressed",
                             typeof(bool),
                             typeof(ButtonChameleon),
                             new FrameworkPropertyMetadata(
                                    false,
                                    new PropertyChangedCallback(OnRenderPressedChanged)));

            /// <summary>
            /// When true the chrome renders with a pressed look.
            /// </summary>
            public bool RenderPressed
            {
                get => (bool)GetValue(RenderPressedProperty);
                set => SetValue(RenderPressedProperty, value);
            }

            private static void OnRenderPressedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                ButtonChameleon chameleon = ((ButtonChameleon)o);

                if (Animates)
                {
                    if ((bool)e.NewValue)
                    {
                        if (chameleon._localResouces == null)
                        {
                            chameleon._localResouces = new LocalResouces();
                            // ставит в очередь перерисовку
                            chameleon.InvalidateVisual();
                        }

                        // TODO animations setup:

                        var parameters = chameleon.GetPressedAnimationParameters( _pressedDuration );
                        AnimateBrushColor( parameters );

                        if ( !chameleon.RenderMouseOver ) {
                            AnimateBrushOpacity( parameters.From, _pressedDuration );
                        }
                    }
                    // Mouse is not hovered
                    else if (chameleon._localResouces == null)
                    {
                        chameleon.InvalidateVisual();
                    }
                    else
                    {
                        if ( chameleon.RenderMouseOver ) {
                            var parameters = chameleon.GetHoverAnimationParameters( _pressedDuration );
                            AnimateBrushColor(parameters);
                        }
                        else {
                            AnimateBackgroundToNormal( chameleon, _pressedDuration );
                        }
                    }
                }
                else
                {
                    chameleon._localResouces = null;
                    chameleon.InvalidateVisual();
                }

                chameleon.Foreground = chameleon.ForegroundOverlay;
            }

            #endregion


            #region RenderDisabledProperty

        /// <summary>
        /// DependencyProperty for <see cref="RenderMouseOver" /> property.
        /// </summary>
        public static readonly DependencyProperty RenderDisabledProperty =
                 DependencyProperty.Register("RenderDisabled",
                         typeof(bool),
                         typeof(ButtonChameleon),
                         new FrameworkPropertyMetadata(
                                false,
                                new PropertyChangedCallback(OnRenderDisabledChanged)));

        /// <summary>
        /// When true the chrome renders with a mouse over look.
        /// </summary>
        public bool RenderDisabled
        {
            get => (bool)GetValue(RenderDisabledProperty);
            set => SetValue(RenderDisabledProperty, value);
        }

        private static void OnRenderDisabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonChameleon chameleon = ((ButtonChameleon)o);


        }

        #endregion


            #region ForegroundProperty

            public static readonly DependencyProperty ForegroundProperty =
                        TextElement.ForegroundProperty.AddOwner(
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                        null,
                                        FrameworkPropertyMetadataOptions.AffectsRender |
                                        FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender |
                                        FrameworkPropertyMetadataOptions.Inherits,
                                        OnForegroundChanged));

            private static void OnForegroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
            {
                var chameleon = ( ButtonChameleon )d;

                if ( e.NewValue != null ) {
                    if ( !chameleon.RenderMouseOver 
                         && !chameleon.RenderPressed 
                         && !chameleon.RenderDisabled
                         && !object.ReferenceEquals( chameleon.NormalForeground, e.NewValue ))  
                    {
                        var newBrush = (( Brush )e.NewValue).Clone();
                        if ( newBrush.CanFreeze ) {
                            newBrush.Freeze();
                        }

                        chameleon.NormalForeground = newBrush;
                    }
                }

            }

            /// <summary>
            /// The Background property defines the brush used to fill the background of the button.
            /// </summary>
            public Brush Foreground
            {
                get => (Brush)GetValue(ForegroundProperty);
                set => SetValue(ForegroundProperty, value);
            }




        #endregion


            #region HoveredForegroundProperty

            public static readonly DependencyProperty HoveredForegroundProperty =
                        DependencyProperty.RegisterAttached(
                                "HoveredForeground",
                                typeof(Brush),
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                    null,
                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits ));

            public Brush HoveredForeground
            {
                get => (Brush)GetValue(HoveredForegroundProperty);
                set => SetValue(HoveredForegroundProperty, value);
            }

            public static void SetHoveredForeground(DependencyObject element, Brush value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                element.SetValue(HoveredForegroundProperty, value);
            }

            public static Brush GetHoveredForeground(DependencyObject element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                return (Brush)element.GetValue(HoveredForegroundProperty);
            }

            #endregion


            #region NormalForegroundProperty

            public static readonly DependencyProperty NormalForegroundProperty =
                        DependencyProperty.RegisterAttached(
                                "NormalForeground",
                                typeof(Brush),
                                typeof(ButtonChameleon),
                                new FrameworkPropertyMetadata(
                                    null,
                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | 
                                    FrameworkPropertyMetadataOptions.Inherits, 
                                    NormalForegroundChanged // because attached to parent template
                                    ));

            private static void NormalForegroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
            {
                if ( !(d is ButtonChameleon chameleon)) return;

                if ( chameleon.HoveredForeground != null || e.NewValue == null ) return;

                if ( e.NewValue is SolidColorBrush brush ) {
                    if ( brush.Color.GetColorHeight() == ColorHeights.Higher ) {
                        chameleon.HoveredForeground = new SolidColorBrush( brush.Color.ToBrighten( 100 ));
                    }
                    else {
                        chameleon.HoveredForeground = new SolidColorBrush( brush.Color.ToDarken( 100 ));
                    }

                    if ( chameleon.HoveredForeground.CanFreeze ) {
                        chameleon.HoveredForeground.Freeze();
                    }
                }

            }

            public Brush NormalForeground
        {
                get => (Brush)GetValue(NormalForegroundProperty);
                set => SetValue(NormalForegroundProperty, value);
            }

            public static void SetNormalForeground(DependencyObject element, Brush value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                element.SetValue(NormalForegroundProperty, value);
            }

            public static Brush GetNormalForeground(DependencyObject element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                return (Brush)element.GetValue(NormalForegroundProperty);
            }

            #endregion

        #endregion


        #region CLR Properties

        private static readonly object _lock = new object();

        private static bool Animates =>
            SystemParameters.PowerLineStatus == PowerLineStatus.Online &&
            SystemParameters.ClientAreaAnimation &&
            RenderCapability.Tier > 0;

        private BrushAnimationPatameters GetHoverAnimationParameters( Duration duration  )
            => new BrushAnimationPatameters() {
                From = BackgroundOverlay,
                To = HoveredBackground,
                Default = NormalBackgroundBrush,
                Duration = duration,
                DefaultPercent = HOVER_PERCENT,
                ColorHeights = _colorHeights
            };

        private BrushAnimationPatameters GetPressedAnimationParameters(Duration duration)
            => new BrushAnimationPatameters()
            {
                From = BackgroundOverlay,
                To = PressedBackground,
                Default = NormalBackgroundBrush,
                Duration = duration,
                DefaultPercent = PRESSED_PERCENT,
                ColorHeights = _colorHeights
            };

        private static Brush _commonDisabledBackgroundBrush;
        private static Brush CommonDisabledBackgroundBrush
        {
            get {
                if ( _commonDisabledBackgroundBrush == null ) {
                    bool lockTaken = false;
                    Monitor.Enter( _lock, ref lockTaken );

                    if ( _commonDisabledBackgroundBrush == null ) {
                        _commonDisabledBackgroundBrush = new SolidColorBrush();

                        if ( _commonDisabledBackgroundBrush.CanFreeze ) {
                            _commonDisabledBackgroundBrush.Freeze();
                        }
                    }

                    if ( lockTaken ) Monitor.Exit( _lock );
                }

                return _commonDisabledBackgroundBrush;
            }
        }

        private Brush _disabledBackgroundBrush;

        private Brush DisabledBackgroundBrush
        {
            get {
                if ( Background == null ) {
                    return CommonDisabledBackgroundBrush;
                }

                return _disabledBackgroundBrush;
            }
        }



        private static Brush _commonMouseHoverBackgroundBrush;

        private static Brush CommonMouseHoverBackgroundBrush
        {
            get {
                if (_commonMouseHoverBackgroundBrush == null)
                {
                    bool lockTaken = false;
                    Monitor.Enter(_lock, ref lockTaken);

                    if (_commonMouseHoverBackgroundBrush == null)
                    {
                        _commonMouseHoverBackgroundBrush = new SolidColorBrush();

                        if (_commonMouseHoverBackgroundBrush.CanFreeze)
                        {
                            _commonMouseHoverBackgroundBrush.Freeze();
                        }
                    }

                    if (lockTaken) Monitor.Exit(_lock);
                }

                return _commonMouseHoverBackgroundBrush;
            }
        }

        private Brush _mouseHoverBackgroundBrush;

        private Brush MouseHoverBackgroundBrush
        {
            get {
                if (Background == null)
                {
                    return CommonMouseHoverBackgroundBrush;
                }

                return _mouseHoverBackgroundBrush;
            }
        }



        private static Brush _commonPressedBackgroundBrush;

        private static Brush CommonPressedBackgroundBrush
        {
            get {
                if (_commonPressedBackgroundBrush == null)
                {
                    bool lockTaken = false;
                    Monitor.Enter(_lock, ref lockTaken);

                    if (_commonPressedBackgroundBrush == null)
                    {
                        _commonPressedBackgroundBrush = new SolidColorBrush();

                        if (_commonPressedBackgroundBrush.CanFreeze)
                        {
                            _commonPressedBackgroundBrush.Freeze();
                        }
                    }

                    if (lockTaken) Monitor.Exit(_lock);
                }

                return _commonPressedBackgroundBrush;
            }
        }

        private Brush _pressedBackgroundBrush;

        private Brush PressedBackgroundBrush
        {
            get {
                if (Background == null)
                {
                    return CommonPressedBackgroundBrush;
                }

                return _pressedBackgroundBrush;
            }
        }



        private static SolidColorBrush _commonNormalBackgroundBrush;

        private static SolidColorBrush CommonNormalBackgroundBrush
        {
            get {
                if (_commonNormalBackgroundBrush == null)
                {
                    bool lockTaken = false;
                    Monitor.Enter(_lock, ref lockTaken);

                    if (_commonNormalBackgroundBrush == null)
                    {
                        _commonNormalBackgroundBrush = new SolidColorBrush( new Color() { A = 0, R = 0, G = 0, B = 0 } );

                        if (_commonNormalBackgroundBrush.CanFreeze)
                        {
                            _commonNormalBackgroundBrush.Freeze();
                        }
                    }

                    if (lockTaken) Monitor.Exit(_lock);
                }

                return _commonNormalBackgroundBrush;
            }
        }

        private Brush _normalBackgroundBrush;

        private Brush NormalBackgroundBrush
        {
            get {
                Brush brush;

                if ( Background == null 
                     || ( Background != null
                          && Background.GetType() != typeof( SolidColorBrush )
                          && Background.GetType() != typeof( GradientBrush ) ) )
                {
                    brush = CommonNormalBackgroundBrush;
                    _colorHeights = new [] { ((SolidColorBrush)brush).Color.GetColorHeight() };

                    return CommonNormalBackgroundBrush;
                }


                brush = Background;
 
                _colorHeights = brush is SolidColorBrush 
                                    ? new[] { (( SolidColorBrush )brush).Color.GetColorHeight() } 
                                    : GetColorHeights( ( GradientBrush )brush );
                return brush;
            }
        }

        private ColorHeights[] GetColorHeights( GradientBrush brush )
        {
            if ( brush == null ) return new ColorHeights[0];
            var gs = brush.GradientStops;
            var colorHeights = new ColorHeights[ gs.Count ];

            for ( int i = 0; i < gs.Count; ++i ) {
                colorHeights[ i ] = gs[ i ].Color.GetColorHeight();
            }

            return colorHeights;
        }


        private Brush BackgroundOverlay
        {
            get {
                if (!Animates)
                {
                    if ( RenderDisabled ) {
                        return DisabledBackgroundBrush;
                    }

                    if ( RenderMouseOver ) {
                        return MouseHoverBackgroundBrush;
                    }

                    if ( RenderPressed ) {
                        return PressedBackgroundBrush;
                    }

                    return null;
                }

                if ( _localResouces != null ) {

                    if ( _localResouces.BackgroundOverlay == null ) {

                        _localResouces.BackgroundOverlay = NormalBackgroundBrush.Clone();
                        _localResouces.BackgroundOverlay.Opacity = 0;

                    }

                    return _localResouces.BackgroundOverlay;
                }

                return null;
            }
        }


        private Brush ForegroundOverlay
        {
            get {

                if ( (RenderMouseOver || RenderPressed) && HoveredForeground != null  ) {
                    return HoveredForeground;
                }

                return NormalForeground;
            }
        }

        #endregion


        #region Protected Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desired;
            UIElement child = Child;

            if (child != null)
            {
                Size childConstraint = new Size();
                bool isWidthTooSmall = (availableSize.Width < 2.0);
                bool isHeightTooSmall = (availableSize.Height < 2.0);
 
                if (!isWidthTooSmall)
                {
                    childConstraint.Width = availableSize.Width - 2.0;
                }
                if (!isHeightTooSmall)
                {
                    childConstraint.Height = availableSize.Height - 2.0;
                }

                child.Measure(childConstraint);
                desired = child.DesiredSize;

                if (!isWidthTooSmall)
                {
                    desired.Width += 2;
                }

                if (!isHeightTooSmall)
                {
                    desired.Height += 2;
                }
            }
            else
            {
                desired = new Size(
                   width: Math.Min(2, availableSize.Width),
                   height: Math.Min(2, availableSize.Height));
            }

            return desired;
        }

        /// <summary>
        /// Computes the position of its single child
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement child = Child;
            if (child == null) return finalSize;


            Rect childArrangeRect = new Rect
            {
                Width = Math.Max(0d, finalSize.Width - 2),
                Height = Math.Max(0d, finalSize.Height - 2)
            };
            childArrangeRect.X = (finalSize.Width - childArrangeRect.Width) * 0.5;
            childArrangeRect.Y = (finalSize.Height - childArrangeRect.Height) * 0.5;

            child.Arrange(childArrangeRect);

            return finalSize;
        }

        /// <summary>
        /// Render callback.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Trace.WriteLine($"OnRender thread id: {Thread.CurrentThread.ManagedThreadId}");

            Rect bounds = new Rect(0, 0, ActualWidth, ActualHeight);

            // Draw Background (if we don't  the system draws white rectangle)
            DrawBackground(drawingContext, ref bounds);

            // Draw Border dropshadows
            //DrawDropShadows( drawingContext, ref bounds );

            // Draw outer border
            //DrawBorder( drawingContext, ref bounds );
        }

        private void DrawBackground(DrawingContext dc, ref Rect bounds)
        {
            if (!IsEnabled)
                return;

            Brush fill = Background;

            if ((bounds.Width > 2) && (bounds.Height > 2))
            {
                Rect backgroundRect = new Rect(bounds.Left + 1,
                                    bounds.Top + 1,
                                    bounds.Width - 2,
                                    bounds.Height - 2);

                // Draw Background
                if (fill != null)
                    dc.DrawRectangle(fill, null, backgroundRect);

                // Draw BackgroundOverlay
                fill = BackgroundOverlay;
                if (fill != null)
                    dc.DrawRectangle(fill, null, backgroundRect);
            }
        }


        private void DrawBorder(DrawingContext dc, ref Rect bounds)
        {

        }

        #endregion

        private LocalResouces _localResouces;

        private class LocalResouces
        {
            public Brush BackgroundOverlay;
        }

        private struct BrushAnimationPatameters
        {
            public Brush From { get; set; }
            public Brush To { get; set; }
            public Brush Default { get; set; }
            public Duration Duration { get; set; }
            public byte DefaultPercent { get; set; }
            public ColorHeights[] ColorHeights { get; set; }
        }
    }
}
