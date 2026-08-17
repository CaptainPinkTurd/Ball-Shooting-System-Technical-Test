using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly (string conditionName, object compareValue, EComparisonType comparisonType)[] Conditions;
        public readonly EConditionalLogic Logic;

        // -------------------------
        // 1) Default boolean-style
        // Example:
        // [ShowIf(nameof(flag)]
        // [ShowIf(nameof(a), true, nameof(b), false)]
        // -------------------------
        public ShowIfAttribute(params object[] args)
        {
            Logic = EConditionalLogic.And;
            Conditions = ParseBoolStyle(args);
        }

        // -------------------------
        // 2) Single comparison 
        // [ShowIf(nameof(value), EComparisonType.Greater, 5)]
        // -------------------------
        public ShowIfAttribute(string conditionName, EComparisonType comparison, object compareValue)
        {
            Logic = EConditionalLogic.And;
            Conditions = new[]
            {
                (conditionName, compareValue, comparison)
            };
        }

        // -------------------------
        // 3) Boolean-style OR comparison group
        // [ShowIf(EConditionalLogic.Or, nameof(a), true, nameof(b), false)]
        // OR:
        // [ShowIf(EConditionalLogic.Or,
        //     nameof(x), EComparisonType.Equals, value1,
        //     nameof(y), EComparisonType.Equals, value2)]
        // -------------------------
        public ShowIfAttribute(EConditionalLogic logic, params object[] args)
        {
            Logic = logic;

            Conditions = LooksLikeComparisonTriplets(args)
                ? ParseComparisonStyle(args)
                : ParseBoolStyle(args);
        }
        
        // -------------------------
        // Helper Detection
        // -------------------------
        private static bool LooksLikeComparisonTriplets(object[] args)
        {
            if (args == null || args.Length == 0) return false;
            if (args.Length % 3 != 0) return false;

            for (int i = 1; i < args.Length; i += 3)
                if (args[i] is not EComparisonType) return false;

            for (int i = 0; i < args.Length; i += 3)
                if (args[i] is not string) return false;

            return true;
        }

        private (string, object, EComparisonType)[] ParseComparisonStyle(object[] args)
        {
            int groups = args.Length / 3;
            var result = new (string, object, EComparisonType)[groups];

            for (int i = 0; i < groups; i++)
            {
                int idx = i * 3;

                if (args[idx] is not string name)
                {
                    Debug.LogError("[ShowIf] Expected condition name (string).");
                    continue;
                }

                if (args[idx + 1] is not EComparisonType cmp)
                {
                    Debug.LogError("[ShowIf] Expected EComparisonType.");
                    continue;
                }

                result[i] = (name, args[idx + 2], cmp);
            }

            return result;
        }

        private (string, object, EComparisonType)[] ParseBoolStyle(object[] args)
        {
            if (args == null || args.Length == 0)
                return new (string, object, EComparisonType)[0];

            if (args.Length % 2 != 0)
            {
                var temp = new object[args.Length + 1];
                args.CopyTo(temp, 0);
                temp[args.Length] = true;
                args = temp;
            }

            int count = args.Length / 2;
            var result = new (string, object, EComparisonType)[count];

            for (int i = 0; i < count; i++)
            {
                int idx = i * 2;

                if (args[idx] is not string name)
                {
                    Debug.LogError("[ShowIf] Bool parser expected string for condition name.");
                    continue;
                }

                result[i] = (name, args[idx + 1], EComparisonType.Equals);
            }

            return result;
        }
    }
}