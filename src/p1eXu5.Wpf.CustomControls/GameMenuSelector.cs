using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using p1eXu5.Wpf.CustomControls.Infrastructure;

namespace p1eXu5.Wpf.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary1"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary1;assembly=WpfCustomControlLibrary1"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:GameMenuSelector/>
    ///
    /// </summary>
    public class GameMenuSelector : Selector
    {
        #region static

        static GameMenuSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameMenuSelector), new FrameworkPropertyMetadata(typeof(GameMenuSelector)));
            SetUpCommands();
        }

        private static void SetUpCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(GameMenuSelector),
                                                       new CommandBinding(SelectorCommands.SelectNext, SelectNext_Executed, SelectorCommands_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(GameMenuSelector),
                                                       new CommandBinding(SelectorCommands.SelectPrev, SelectPrev_Executed, SelectorCommands_CanExecute));
        }

        private static void SelectPrev_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is GameMenuSelector selector)
            {
                selector.Prev();
            }
        }

        private static void SelectNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is GameMenuSelector selector)
            {
                selector.Next();
            }
        }

        private static void SelectorCommands_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is GameMenuSelector selector && selector.HasItems;
        }

        #endregion


        #region fields

        private object[] _items;
        private int _ind;
        private readonly object _lock = new object();

        #endregion


        #region ctor

        public GameMenuSelector()
        {
            MouseEnter += (e, s) =>
            {
                if (!IsKeyboardFocused)
                {
                    Keyboard.Focus(this);
                }
            };

            //LostFocus += OnLostFocus;

            AddHandler(FocusManager.LostFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) { WriteFocusable(sender); }));
            AddHandler(FocusManager.GotFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) { WriteFocusable(sender); }));
            //MouseLeave += ( e, s ) => {
            //    if ( !IsFocused ) return;

            //    var fnd = FocusNavigationDirection.Next;
            //    var request = new TraversalRequest( fnd );
            //    if ( Parent is FrameworkElement fe ) {

            //        fe.MoveFocus( request );
            //    }
            //};
        }

        #endregion


        #region overrides

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {

            if (newValue != null)
            {
                _items = newValue as object[] ?? newValue.Cast<object>().ToArray();
                _ind = -1;
                if (_items.Length > 0)
                {
                    _ind = 0;
                    SelectedItem = _items[_ind];
                }
            }

            base.OnItemsSourceChanged(oldValue, _items);
            CommandManager.InvalidateRequerySuggested();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var prevButton = (Button)GetTemplateChild("PART_PrevButton");
            if (prevButton != null)
            {
                prevButton.Focusable = false;
                prevButton.Command = SelectorCommands.SelectPrev;
            }

            var nextButton = (Button)GetTemplateChild("PART_NextButton");
            if (nextButton != null)
            {
                nextButton.Focusable = false;
                nextButton.Command = SelectorCommands.SelectNext;
            }
        }

        #endregion


        #region privates

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            WriteFocusable(sender);
        }

        private void WriteFocusable(object sender)
        {
            Trace.WriteLine($"{sender.GetType().Name}::{nameof(IsKeyboardFocused)}: {IsKeyboardFocused}");
        }

        private void Next()
        {
            if (_ind < 0)
            {
                SelectedItem = null;
            }

            OffsetIndex(1);
            SelectedItem = _items[_ind];
        }

        private void Prev()
        {
            if (_ind < 0)
            {
                SelectedItem = null;
            }

            OffsetIndex(-1);
            SelectedItem = _items[_ind];
        }

        private void OffsetIndex(int offset)
        {
            bool lockTaken = false;
            Monitor.Enter(_lock, ref lockTaken);
            _ind += offset;
            if (_ind < 0)
            {
                if ( _ind <= -_items.Length ) {
                    _ind %= _items.Length;
                }

                _ind += _items.Length;
            }
            else 
                if (_ind >= _items.Length)
                {
                    _ind %= _items.Length;
                }
            if (lockTaken) Monitor.Exit(_lock);
        }

        #endregion
    }
}
