using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Agbm.Wpf.CustomControls.Infrastructure
{
    public class WindowCommands
    {
        static WindowCommands()
        {
            InputGestureCollection input = new InputGestureCollection {
                new KeyGesture( Key.Enter, ModifierKeys.Alt, "Alt+Enter" )
            };
            Maximize = new RoutedUICommand("Maximize window", "Maximize", typeof(WindowCommands), input);

            input = new InputGestureCollection {
                new KeyGesture( Key.Enter, ModifierKeys.Alt, "Alt+Enter" )
            };
            Restore = new RoutedUICommand("Restore window", "Restore", typeof(WindowCommands), input);

            Minimize = new RoutedUICommand("Minimize window", "Minimize", typeof(WindowCommands) );

            input = new InputGestureCollection {
                new KeyGesture( Key.F4, ModifierKeys.Alt, "Alt+F4" )
            };
            Close = new RoutedUICommand("Close window", "Close", typeof(WindowCommands), input);
        }

        public static RoutedUICommand Maximize { get; }
        public static RoutedUICommand Restore { get; }
        public static RoutedUICommand Minimize { get; }
        public static RoutedUICommand Close { get; }
    }
}
