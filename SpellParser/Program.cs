using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpellParser.Commmands;
using SpellParser.Infrastructure.Data;
using SpellParser.Infrastructure.Reporters;

namespace SpellParser
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            //using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options => options.SingleLine = true));
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Hello Everquest!");

            // https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
            IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            var settings = config.GetRequiredSection("Settings").Get<Settings>();

            using var reporter = new MarkdownReporter(settings.Export);

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqCasterSpells = eqCasterSpellsRepository.GetAll(settings.Import, settings.Expansion);

            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll(settings.Import);

            var commands = new ICommand[] {
                new CheckDuplicateSPDATSpellsCommand(eqCasterSpells, reporter, logger)
                , new CheckMissingSpellNamesInPEQCommand(eqCasterSpells, peqSpells, reporter, logger)
                , new CheckDoublicateSpellNameMatchesWithPEQCommand(eqCasterSpells, peqSpells, reporter, logger)
                , new CreateAutomaticUpdateScript(eqCasterSpells, peqSpells, reporter, settings.Export, logger)
                , new CreateManualUpdateLogCommand(eqCasterSpells, peqSpells, reporter, logger)
            };

            foreach (var command in commands)
            {
                command.Execute();
            }
        }
    }
}