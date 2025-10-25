using System;
using System.IO;
using System.Linq;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace PlaySound.Common
{
    /// <summary>
    /// 音频管理器，负责音频资源的生命周期管理和播放控制
    /// </summary>
    public class AudioManager : IDisposable
    {
        private MiniAudioEngine? _engine;
        private AudioPlaybackDevice? _device;
        private SoundPlayer? _player;
        private Stream? _resourceStream;
        private bool _isInitialized = false;
        private bool _disposed = false;

        /// <summary>
        /// 获取音频管理器的单例实例
        /// </summary>
        public static AudioManager Instance { get; } = new AudioManager();

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        private AudioManager() { }

        /// <summary>
        /// 初始化音频管理器
        /// </summary>
        /// <param name="audioStream">音频数据流</param>
        /// <returns>初始化是否成功</returns>
        public bool Initialize(Stream audioStream)
        {
            if (_isInitialized)
                return true;

            ArgumentNullException.ThrowIfNull(audioStream);

            try
            {
                // 1. Initialize the Audio Engine context.
                // 初始化音频引擎上下文
                _engine = new MiniAudioEngine();

                // 2. Define the desired audio format.
                // 定义目标音频格式
                var format = AudioFormat.Dvd; // 48kHz, 16-bit Stereo

                // 3. Initialize a specific playback device. Passing `null` will use the system default too.
                // 初始化特定的播放设备。传递 `null` 将使用系统默认设置。
                var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);
                _device = _engine.InitializePlaybackDevice(defaultDevice, format);

                // 4. Create a SoundPlayer with the provided stream
                // 使用提供的流创建 SoundPlayer
                _resourceStream = audioStream;
                _player = new SoundPlayer(_engine, _device.Format,
                    new StreamDataProvider(_engine, _device.Format, _resourceStream));

                // 5. Add the player to the device's MasterMixer.
                // 将播放器添加到音频设备的 MasterMixer。
                _device.MasterMixer.AddComponent(_player);

                // 6. Start the device to begin audio processing.
                // 启动设备以开始音频处理。
                _device.Start();

                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Cleanup();
                throw new AudioException($"音频管理器初始化失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 使用指定的音频数据流播放音频
        /// </summary>
        /// <param name="audioStream">音频数据流</param>
        public void PlayAudio(Stream audioStream)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioManager), "音频管理器已被释放，无法使用");

            if (audioStream == null)
                throw new ArgumentNullException(nameof(audioStream));

            // 清理当前资源
            Cleanup();

            // 使用新的流重新初始化
            Initialize(audioStream);
            Play();
        }

        /// <summary>
        /// 使用指定的音频资源名称播放音频（便利方法）
        /// </summary>
        /// <param name="audioResourceName">音频资源名称</param>
        public void PlayAudio(string audioResourceName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioManager), "音频管理器已被释放，无法使用");

            if (!ResourceManager.ResourceExists(audioResourceName))
                throw new ArgumentException($"音频资源不存在: {audioResourceName}", nameof(audioResourceName));

            using var audioStream = ResourceManager.GetResourceStream(audioResourceName);
            PlayAudio(audioStream);
        }

        /// <summary>
        /// 播放当前加载的音频
        /// </summary>
        public void Play()
        {
            EnsureInitialized();

            try
            {
                _player?.Stop();
                _player?.Play();
            }
            catch (Exception ex)
            {
                throw new AudioException($"音频播放失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 停止音频播放
        /// </summary>
        public void Stop()
        {
            if (_isInitialized && _player != null)
            {
                try
                {
                    _player.Stop();
                }
                catch (Exception ex)
                {
                    throw new AudioException($"音频停止失败: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// 检查音频管理器是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 确保音频管理器已初始化
        /// </summary>
        private void EnsureInitialized()
        {
            if (!_isInitialized || _player == null)
            {
                throw new InvalidOperationException("音频管理器未初始化，请先调用 Initialize() 方法");
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        private void Cleanup()
        {
            try
            {
                _player?.Stop();
                _device?.Stop();
                _player?.Dispose();
                _device?.Dispose();
                _engine?.Dispose();
                _resourceStream?.Dispose();
            }
            catch
            {
                // 忽略清理过程中的异常
            }
            finally
            {
                _player = null;
                _device = null;
                _engine = null;
                _resourceStream = null;
                _isInitialized = false;
            }
        }

        /// <summary>
        /// 实现 Dispose 模式
        /// </summary>
        /// <param name="disposing">是否正在释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    Cleanup();
                }

                // TODO: 释放非托管资源（如果有的话）
                _disposed = true;
            }
        }

        /// <summary>
        /// 实现 IDisposable 接口
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 终结器，作为安全网确保非托管资源被释放
        /// </summary>
        ~AudioManager()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// 音频相关的异常类
    /// </summary>
    public class AudioException : Exception
    {
        public AudioException(string message) : base(message) { }
        public AudioException(string message, Exception innerException) : base(message, innerException) { }
    }
}
