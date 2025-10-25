using PlaySound.Core;

namespace PlaySound.Config
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourcesType { Audio }

    public sealed class AppResources : SmartEnum<AppResources, string>
    {
        public static readonly AppResources WHOOP = new(nameof(WHOOP))
        { Path = "Whoop.wav", Type = ResourcesType.Audio };

        #region Instance Members and Properties
        private AppResources(string key) : base(key) { }
        public ResourcesType? Type { get; init; }
        public required string Path { get; init; }
        #endregion
    }
}
