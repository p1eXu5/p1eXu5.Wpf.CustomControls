
### TetrisWindow

  ![TetrisWindow](images/tetris-window.png)

  * Setup:

  ```xml
  <!-- App.xaml -->
  ...
  <ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/p1eXu5.Wpf.CustomControls;component/Themes/ChameleonButtonStyle.xaml" />
  </ResourceDictionary.MergedDictionaries>
  ...
  ```

  ```xml
  <!-- MainWindow.xaml -->
  <p1eXu5:TetrisWindow x:Class="p1eXu5.PxUml.WpfClient.MainWindow"
        xmlns:p1eXu5="clr-namespace:p1eXu5.Wpf.CustomControls;assembly=p1eXu5.Wpf.CustomControls"
    ...
  ```

  ```csharp
  // MainWindow.xaml.cs
  public partial class MainWindow : TetrisWindow
  {
    ...
  }
  ```
