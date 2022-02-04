namespace SpellParser.Core
{
    public record SpellEffect
    {
        public string EffectId { get; init; } = default!;
        public string BaseValue { get; init; } = default!;
        public string MaxValue { get; init; } = default!;
        public string LimitValue { get; init; } = default!;
        public string Formula { get; init; } = default!;
    }
}