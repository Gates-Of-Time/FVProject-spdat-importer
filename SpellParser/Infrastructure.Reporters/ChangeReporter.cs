using SpellParser.Core;

namespace SpellParser.Infrastructure.Reporters
{
    public class ChangeReporter
    {
        public static string ToMarkdown(Change change)
        {
            return $"{{ Column = {change.Name}, OldValue = {change.OldValue}, NewValue = {change.NewValue} }}";
        }
    }
}