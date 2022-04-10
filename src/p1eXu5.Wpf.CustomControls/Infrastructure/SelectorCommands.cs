using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace p1eXu5.Wpf.CustomControls.Infrastructure
{
    class SelectorCommands
    {
        static SelectorCommands()
        {
            SelectPrev = new RoutedUICommand("Select previous item", 
                                             "SelectPrev", 
                                             typeof(SelectorCommands), 
                                             new InputGestureCollection {
                                                 new KeyGesture( Key.Left, 
                                                                 ModifierKeys.None, 
                                                                 "Left Arrow" )
                                             } );


            SelectNext = new RoutedUICommand("Restore window", 
                                             "Restore", 
                                             typeof(SelectorCommands), 
                                             new InputGestureCollection {
                                                new KeyGesture( Key.Right, 
                                                                ModifierKeys.None, 
                                                                "Right Arrow" )
                                             } );
        }

        public static RoutedUICommand SelectPrev { get; }
        public static RoutedUICommand SelectNext { get; }

    }
}
