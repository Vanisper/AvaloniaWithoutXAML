using Avalonia.Controls;
using Avalonia.Media;

namespace PlaySound.Common
{
    /// <summary>
    /// UI 辅助类，提供通用的 UI 操作方法
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// 创建一个标准的按钮
        /// </summary>
        /// <param name="content">按钮内容</param>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns>配置好的按钮</returns>
        public static Button CreateStandardButton(string content, double width = 120, double height = 30)
        {
            return new Button
            {
                Content = content,
                Width = width,
                Height = height,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Background = new SolidColorBrush(Color.FromRgb(70, 130, 180)),
                Foreground = new SolidColorBrush(Colors.White)
            };
        }

        /// <summary>
        /// 创建一个标准的文本块
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="centered">是否居中</param>
        /// <returns>配置好的文本块</returns>
        public static TextBlock CreateStandardTextBlock(string text, double fontSize = 14, bool centered = true)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(10)
            };

            if (centered)
            {
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            }

            return textBlock;
        }

        /// <summary>
        /// 创建一个垂直布局容器
        /// </summary>
        /// <param name="spacing">子元素间距</param>
        /// <returns>配置好的 StackPanel</returns>
        public static StackPanel CreateVerticalStackPanel(double spacing = 10)
        {
            return new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Spacing = spacing,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
}