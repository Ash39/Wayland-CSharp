using System;

namespace Wayland
{
    public static class DebugLog
    {
        private static readonly bool HasEnvVar;
        static DebugLog()
        {
            HasEnvVar = Environment.GetEnvironmentVariable("WAYLAND_DEBUG") == "1";
        }

        public static void WriteLine(DebugType type,string @interface, uint id, string op)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1>(DebugType type,string @interface, uint id, string op,T1 arg1)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };
            
            @string += $"({arg1})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };
            
            @string += $"({arg1}, {arg2})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };
            
            @string += $"({arg1}, {arg2}, {arg3})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };
            
            @string += $"({arg1}, {arg2}, {arg3}, {arg4})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6})";
            
            Console.WriteLine(@string);
        }
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11}, {arg12})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11}, {arg12}, {arg13})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11}, {arg12}, {arg13}, {arg14})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11}, {arg12}, {arg13}, {arg14}, {arg15})";
            
            Console.WriteLine(@string);
        }
        
        public static void WriteLine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(DebugType type,string @interface, uint id, string op,T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            if (!HasEnvVar) return;

            string @string = type switch
            {
                DebugType.Event => $"{@interface}@{id}.{op}",
                DebugType.Request => $"-->{@interface}@{id}.{op}",
                _ => null
            };

            @string += $"({arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}, {arg7}, {arg8}, {arg9}, {arg10}, {arg11}, {arg12}, {arg13}, {arg14}, {arg15}, {arg16})";
            
            Console.WriteLine(@string);
        }
    }
    
    public enum DebugType
    {
        Event,
        Request
    }
}
