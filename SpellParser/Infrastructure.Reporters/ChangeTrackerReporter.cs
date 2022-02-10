using SpellParser.Core;

namespace SpellParser.Infrastructure.Reporters
{
    public class ChangeTrackerReporter
    {
        public static string ToMarkdown(ChangeTracker changeTracker)
        {
            var changes = changeTracker.Changes.Select(x => ChangeReporter.ToMarkdown(x));
            return $@"```
Id = {changeTracker.Id}
Spell = {changeTracker.Name}
Changes = [
  {string.Join("\n  ", changes)}
]
```";
        }
    }
}