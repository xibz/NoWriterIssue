using System.Reflection;
using UnityEngine;

namespace TestUtils {
    public static class PrivateUtils {
        public static T GetValue<T>(object o, string member) {
            FieldInfo info = o.GetType().GetField(member, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)info.GetValue(o);
        }

        public static void SetValue<T>(object o, string member, T value) {
            object[] param = new object[]{
                value
            };
            o.GetType().GetProperty(member).GetSetMethod(true).Invoke(o, param);
        }
    }
}
