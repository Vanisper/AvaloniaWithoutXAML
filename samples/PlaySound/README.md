# PlaySound

这是一个展示如何在跨平台环境中播放音频的示例。请注意，Avalonia 本身不支持音频功能。
本示例使用 SoundFlow 库，这是一个专门为跨平台音频播放设计的库，可在 Windows、macOS 和 Linux 上运行。
代码已经更新，用现代的跨平台音频解决方案替代了仅适用于 Windows 的 System.Windows.Extensions 包。

## 功能特点

- 跨平台音频播放（Windows、macOS、Linux）
- 使用 SoundFlow 库和 MiniAudio 后端
- 嵌入式音频资源（Whoop.wav）
- 简单的用户界面，包含播放按钮
- 音频初始化失败的错误处理

## 实现方式

应用程序初始化 SoundFlow 音频引擎，创建播放设备，并加载嵌入的 WAV 文件。
当用户点击"播放"按钮时，声音将通过默认音频设备播放。

## 解决的问题

原始项目使用了 Windows 专用的 System.Windows.Extensions 包，在 macOS 和 Linux 上无法运行。
现在使用 SoundFlow 库，实现了真正的跨平台音频播放功能。

![Window with a "Play" button.](ScreenCap.png "Window with a 'Play' button.")