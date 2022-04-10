using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;
using Agbm.Wpf.CustomControls.Infrastructure;
using p1eXu5.Wpf.CustomControls.Infrastructure;
using static p1eXu5.Wpf.CustomControls.Helpers.WindowHelpers;

namespace p1eXu5.Wpf.CustomControls
{
    [TemplatePart(Name = "PART_WindowBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_HeaderBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MaximizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_RestoreButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class TetrisWindow : Window
    {
        private double _lastWidth, _lastHeight;

        static TetrisWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TetrisWindow), new FrameworkPropertyMetadata(typeof(TetrisWindow)));
            SetUpCommands();
        }

        public TetrisWindow()
        {
            SizeChanged += OnSizeChanged;
            StateChanged += OnStateChanged;
            Loaded += OnLoaded;
        }


        #region Properties

        private IntPtr Hwnd => new WindowInteropHelper( this ).Handle;

        private Button MinimizeButton { get; set; }
        private Button MaximizeButton { get; set; }
        private Button RestoreButton { get; set; }
        private Button CloseButton { get; set; }
        private Border WindowBorder { get; set; }

        #endregion


        #region Commands

        private static void Maximize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            e.CanExecute = wnd.WindowState == WindowState.Normal;
        }

        private static void Maximize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            wnd.WindowState = WindowState.Maximized;
        }

        private static void Restore_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            e.CanExecute = wnd.WindowState == WindowState.Maximized;
        }

        private static void Restore_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            wnd.WindowState = WindowState.Normal;
            wnd.RestoreButton.Visibility = Visibility.Collapsed;
            wnd.MaximizeButton.Visibility = Visibility.Visible;
        }

        private static void Minimize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            e.CanExecute = wnd.WindowState != WindowState.Minimized;
        }

        private static void Minimize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            wnd.WindowState = WindowState.Minimized;
        }

        private static void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = (TetrisWindow)sender;
            wnd.Close();
        }

        #endregion


        #region protected

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MinimizeButton = GetTemplateChild( "PART_MinimizeButton" ) as Button;
            if ( MinimizeButton != null ) {
                MinimizeButton.Command = WindowCommands.Minimize;
            }

            MaximizeButton = GetTemplateChild("PART_MaximizeButton") as Button;
            if (MaximizeButton != null){
                MaximizeButton.Command = WindowCommands.Maximize;
            }

            RestoreButton = GetTemplateChild("PART_RestoreButton") as Button;
            if (RestoreButton != null){
                RestoreButton.Command = WindowCommands.Restore;
            }

            CloseButton = GetTemplateChild("PART_CloseButton") as Button;
            if (CloseButton != null){
                CloseButton.Command = WindowCommands.Close;
            }

            if ( GetTemplateChild("PART_HeaderBorder") is Border header ) {
                header.MouseDown += Header_MouseDown;
                header.MouseLeftButtonDown += Header_MouseLeftButtonDown;

            }

            WindowBorder = GetTemplateChild("PART_WindowBorder") as Border;


        }

        #endregion


        #region Handlers

        private void OnLoaded( object sender, RoutedEventArgs args )
        {
            SwitchMaximazeRestoreButtons();
            _lastHeight = Height;
            _lastWidth = Width;
        }
        private void OnStateChanged( object sender, EventArgs e )
        {
            CommandManager.InvalidateRequerySuggested();
            SwitchMaximazeRestoreButtons();

            if ( WindowState == WindowState.Maximized ) {
                _lastHeight = ActualHeight;
                _lastWidth = ActualWidth;
            }
            else if ( WindowState == WindowState.Normal ) {
                Width = _lastWidth;
                Height = _lastHeight;
            }
        }
        private void OnSizeChanged( object sender, SizeChangedEventArgs e )
        {
            if ( WindowState == WindowState.Maximized ) 
            {
                var hMonitor = MonitorFromWindow( Hwnd, MonitorDwFlags.MONITOR_DEFAULTTONULL );

                MONITORINFO mi;

                unsafe {
                    mi = new MONITORINFO() {
                        cbSize = ( uint )sizeof( MONITORINFO )
                    };
                }

                if ( GetMonitorInfoA( hMonitor, ref mi ) ) {

                    Thickness t = new Thickness(0);
                    if ( ActualWidth > mi.rcMonitor.left ) {
                        t.Left = t.Right = (ActualWidth - (mi.rcMonitor.right - mi.rcMonitor.left) ) / 2;
                    }

                    if ( ActualHeight > mi.rcMonitor.bottom ) {
                        t.Top = t.Bottom = (ActualHeight - (mi.rcMonitor.bottom - mi.rcMonitor.top) ) / 2;
                    }

                    WindowBorder.Padding = t;
                }
            }
            else {
                WindowBorder.Padding = WindowChrome.GetWindowChrome( this ).ResizeBorderThickness;
            }
        }
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ( WindowCommands.Maximize.CanExecute( null, this ) ) {
                    WindowCommands.Maximize.Execute( null, this );
                }
                else if ( WindowCommands.Restore.CanExecute( null, this ) ) {
                    WindowCommands.Restore.Execute( null, this );
                }
            }
        }

        #endregion


        #region Privates

        private static void SetUpCommands()
        {
            CommandManager.RegisterClassCommandBinding( typeof(TetrisWindow),
                new CommandBinding( WindowCommands.Maximize, Maximize_Executed, Maximize_CanExecute ));

            CommandManager.RegisterClassCommandBinding(typeof(TetrisWindow),
                new CommandBinding(WindowCommands.Restore, Restore_Executed, Restore_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(TetrisWindow),
                new CommandBinding(WindowCommands.Minimize, Minimize_Executed, Minimize_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(TetrisWindow),
                new CommandBinding(WindowCommands.Close, Close_Executed ));
        }
        private void SwitchMaximazeRestoreButtons()
        {
            if ( MaximizeButton == null || RestoreButton == null ) return;

            if ( WindowState == WindowState.Maximized ) 
            {
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else if ( WindowState == WindowState.Normal ) 
            {
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}
