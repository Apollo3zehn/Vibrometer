using System;
using System.Reflection;
using System.Resources;

namespace Vibrometer.BaseTypes
{
    public static class Utilities
    {
        public static string GetEnumLocalization(Enum enumValue, Type dictionaryType)
        {
            ResourceManager resourceManager;

            resourceManager = (ResourceManager)dictionaryType.GetTypeInfo().GetDeclaredProperty("ResourceManager").GetValue(null, null);

            return resourceManager.GetString(enumValue.GetType().Name + "_" + enumValue.ToString());
        }
    }
}
