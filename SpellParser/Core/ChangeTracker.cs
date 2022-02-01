using System.Collections.Generic;
using System.Linq;

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
        public IReadOnlyCollection<Change> Changes { get { return InternalChanges.AsReadOnly(); } }
        private List<Change> InternalChanges { get; } = new List<Change>();

        public void AddChange(Change change)
        {
            InternalChanges.Add(change);
        }
        public void AddChanges(IEnumerable<Change> changes)
        {
            InternalChanges.AddRange(changes);
        }

        public string SQL {
            get {
                var sql = $@"-- {Name}
UPDATE spells_new SET
{string.Join("\n,", Changes.Select(x => $"{x.Name} = {GetSqlValue(x.Name, x.NewValue)}"))}
WHERE id = {Id};";                
                return sql;
            }
        }

        public string UndoSQL
        {
            get
            {
                var sql = $@"-- {Name}
UPDATE spells_new SET
{string.Join("\n,", Changes.Select(x => $"{x.Name} = {GetSqlValue(x.Name, x.OldValue)}"))}
WHERE id = {Id};";
                return sql;
            }
        }

        private string GetSqlValue(string columnName, string value) {
            if (columnName == nameof(PEQSpell.name)) {
                return $"'{value}'";
            }

            return value;
        }

        public override string ToString()
        {
            return $" Id = {Id}, Spell = {Name}, Changes = [{string.Join(',', Changes)}] ";
        }
    }
}