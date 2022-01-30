namespace SpellParser.Core
{
    public class Change
    {
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override string ToString()
        {
            return $" {{ Name = {Name}, OldValue = {OldValue}, NewValue = {NewValue} }} ";
        }
    }
}