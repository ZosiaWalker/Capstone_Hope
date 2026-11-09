using System.Collections.Generic;

namespace DialogueSystem
{
    public static class Constants
    {
        public static List<string> SIMPLE_OPERATORS = new List<string>() {
            "==",
            "!=",
        };

        public static List<string> ALL_OPERATORS = new List<string>() {
            "==",
            "!=",
            "<",
            "<=",
            ">",
            ">="
        };

        public static List<string> BOOL_OPTIONS = new List<string>() {
            "True",
            "False"
        };
    }
}
