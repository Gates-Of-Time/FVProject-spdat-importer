using System.Collections.Generic;

namespace SpellParser.Core
{
    public class ChangeTracker
    {
        private ChangeTracker(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new System.ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Id = id;
            Name = name;
        }

        public static ChangeTracker From(PEQSpell rof2Spell)
        {
            return new ChangeTracker(rof2Spell.id, rof2Spell.name);
        }

        public string Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<Change> Changes
        { get { return InternalChanges.AsReadOnly(); } }
        private List<Change> InternalChanges { get; } = new List<Change>();

        public void AddChange(Change change)
        {
            InternalChanges.Add(change);
        }

        public void AddChanges(IEnumerable<Change> changes)
        {
            InternalChanges.AddRange(changes);
        }
    }
}