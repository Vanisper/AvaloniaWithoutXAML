using System;
using Avalonia;
using Avalonia.Controls;
using PlaySound.Common;
using PlaySound.Config;

class Project
{
    public static void Main(string[] args)
    {
        AppBuilder.Configure<Application>()
                  .UsePlatformDetect()
                  .Start(AppMain, args);
    }

    public static void AppMain(Application app, string[] args)
    {
        string? errorMessage = null;

        try
        {
            // 初始化音频管理器
            using var audioStream = ResourceManager.GetResourceStream(AppResources.WHOOP.Path);
            AudioManager.Instance.Initialize(audioStream); // 单例

            app.Styles.Add(new Avalonia.Themes.Fluent.FluentTheme());
            app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;

            var win = new Window
            {
                Title = "My Sound App",
                Height = 140,
                Width = 180,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            // 使用 UIHelper 创建播放按钮
            var playButton = UIHelper.CreateStandardButton("Play");
            playButton.Margin = new Thickness(0, 5, 0, 5);

            playButton.Click += (s, e) =>
            {
                try
                {
                    if (AudioManager.Instance.IsInitialized)
                    {
                        AudioManager.Instance.Play();
                    }
                }
                catch (AudioException ex)
                {
                    ShowErrorDialog(app, $"播放失败: {ex.Message}");
                }
                catch (Exception ex)
                {
                    ShowErrorDialog(app, $"未知错误: {ex.Message}");
                }
            };

            // 使用 UIHelper 创建模拟错误按钮
            var errorButton = UIHelper.CreateStandardButton("模拟错误");
            errorButton.Margin = new Thickness(0, 5, 0, 5);

            errorButton.Click += (s, e) =>
            {
                try
                {
                    // 故意触发一个错误来演示错误处理
                    throw new AudioException("这是一个模拟的音频播放错误，用于展示错误处理功能。");
                }
                catch (AudioException ex)
                {
                    ShowErrorDialog(app, $"错误演示: {ex.Message}");
                }
            };

            // 使用 UIHelper 创建垂直布局容器
            var stackPanel = UIHelper.CreateVerticalStackPanel(5);

            stackPanel.Children.Add(playButton);
            stackPanel.Children.Add(errorButton);

            win.Content = stackPanel;
            win.Show();
            app.Run(win);
        }
        catch (AudioException ex)
        {
            errorMessage = $"音频系统错误: {ex.Message}";
            ShowErrorWindow(app, errorMessage);
        }
        catch (Exception ex)
        {
            errorMessage = $"应用程序初始化失败: {ex.Message}";
            ShowErrorWindow(app, errorMessage);
        }
    }

    // 显示错误对话框的辅助方法
    private static void ShowErrorDialog(Application app, string message)
    {
        var errorWin = new Window
        {
            Title = "错误",
            Height = 150,
            Width = 400,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(10)
        };

        var okButton = new Button
        {
            Content = "确定",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 10)
        };

        okButton.Click += (s, e) => errorWin.Close();

        var stackPanel = new Avalonia.Controls.StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Vertical
        };

        stackPanel.Children.Add(textBlock);
        stackPanel.Children.Add(okButton);

        errorWin.Content = stackPanel;
        errorWin.Show();
    }

    // 显示错误窗口的辅助方法
    private static void ShowErrorWindow(Application app, string message)
    {
        app.Styles.Add(new Avalonia.Themes.Fluent.FluentTheme());
        app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;

        var win = new Window
        {
            Title = "My Sound App - 错误",
            Height = 120,
            Width = 300,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(10)
        };

        win.Content = textBlock;
        win.Show();
        app.Run(win);
    }
}