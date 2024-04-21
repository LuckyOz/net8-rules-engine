namespace net8_rules_engine.Configs
{
    public static class EngineExpressionItemHelper
    {
        public static bool CheckContains(string check, string valList)
        {
            if (String.IsNullOrEmpty(check) || String.IsNullOrEmpty(valList))
                return false;

            var list = valList.Split(',').ToList();

            return list.Contains(check);
        }
    }
}
