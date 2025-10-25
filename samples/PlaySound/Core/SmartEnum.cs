using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace PlaySound.Core
{
    /// <summary>
    /// 智能枚举
    /// </summary>
    /// <typeparam name="TEnum">派生枚举的类型 (CRTP 模式)</typeparam>
    /// <typeparam name="TKey">Key 的类型 (如 string, int)</typeparam>
    /// <remarks>
    /// 基类构造函数
    /// </remarks>
    /// <param name="key">枚举键</param>
    public abstract class SmartEnum<TEnum, TKey>(TKey key)
        where TEnum : SmartEnum<TEnum, TKey>
        where TKey : notnull
    {
        public TKey Key { get; } = key;

        // <summary>
        // 缓存list
        // </summary>
        private static readonly IReadOnlyCollection<TEnum> _list =
        typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == typeof(TEnum))
            .Select(f => (TEnum)f.GetValue(null)!)
            .ToList()
            .AsReadOnly();

        /// <summary>
        /// 获取所有枚举实例
        /// </summary>
        public static IReadOnlyCollection<TEnum> ToArray() => _list;

        /// <summary>
        /// 根据 Key 获取枚举实例
        /// </summary>
        public static TEnum? GetByKey(TKey key)
        {
            // 使用 EqualityComparer.Default 来处理所有 TKey 类型
            // item.Key.Equals(key) 会有空值隐患；且如果 TKey 是值类型，会导致装箱
            return _list.FirstOrDefault(item =>
                EqualityComparer<TKey>.Default.Equals(item.Key, key));
        }

        /// <summary>
        /// 检查 Key 是否相等
        /// </summary>
        public bool EqualsKey(TKey key)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, key);
        }

        public override string ToString() => Key.ToString() ?? string.Empty;
        public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Key);

        public override bool Equals(object? obj)
        {
            if (obj is not SmartEnum<TEnum, TKey> other)
            {
                return false;
            }
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key);
        }

        // 运算符重载 (可以比较 TEnum == TEnum)
        public static bool operator ==(SmartEnum<TEnum, TKey>? left, SmartEnum<TEnum, TKey>? right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(SmartEnum<TEnum, TKey>? left, SmartEnum<TEnum, TKey>? right)
        {
            return !(left == right);
        }

        // 运算符重载 (可以比较 TEnum == TKey)
        public static bool operator ==(SmartEnum<TEnum, TKey>? left, TKey? right)
        {
            if (left is null || right is null)
                return false; // 或根据业务逻辑调整

            return left.EqualsKey(right);
        }

        public static bool operator !=(SmartEnum<TEnum, TKey>? left, TKey? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// (隐式) 从 SmartEnum? 转换为 TKey?
        /// 安全地处理 null 实例
        /// </summary>
        public static implicit operator TKey?(SmartEnum<TEnum, TKey>? status)
        {
            return status is null ? default : status.Key;
        }

        /// <summary>
        /// (显式) 从 TKey? 转换为 SmartEnum?
        /// 安全地处理 null 或无效的 key
        /// </summary>
        /// <remarks>
        /// **返回类型必须是定义此运算符的类型（即 SmartEnum<TEnum, TKey>）。** /// 
        /// 尽管 GetByKey 返回的是具体的派生类型 TEnum?，
        /// 但 TEnum 继承自 SmartEnum，因此这里的返回值是类型安全的。
        /// 
        /// **在实际调用时 (例如 (DerivedEnum?)key)，C# 编译器会查找此转换，
        /// 并在执行后将返回的 SmartEnum 实例 (TEnum) 视为目标 DerivedEnum 类型。**
        /// </remarks>
        public static explicit operator SmartEnum<TEnum, TKey>?(TKey? key)
        {
            if (key is null)
            {
                return null;
            }

            // GetByKey 返回的是 TEnum?，
            return GetByKey(key);
        }
    }
}
