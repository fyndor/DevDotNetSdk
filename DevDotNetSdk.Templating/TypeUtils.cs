namespace DevDotNetSdk.Templating;

internal static class TypeUtils 
{
    public static bool IsSubclassOfRawGeneric(Type genericType, Type typeToCheck)
    {
        while (typeToCheck != null && typeToCheck != typeof(object))
        {
            var cur = typeToCheck.IsGenericType 
                ? typeToCheck.GetGenericTypeDefinition() 
                : typeToCheck;
            if (genericType == cur)
            {
                return true;
            }
            typeToCheck = typeToCheck.BaseType!;
        }
        return false;
    }
}