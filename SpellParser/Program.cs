using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpellParser.Commmands;
using SpellParser.Infrastructure.Data;
using SpellParser.Infrastructure.Reporters;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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

            Deserialize(settings.Import.EQCasterExportFilePath);


            using var reporter = new MarkdownReporter(settings.Export);
            using var sqlReport = new SQLReporter(settings.Export);

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqCasterSpells = eqCasterSpellsRepository.GetAll(settings.Import, settings.Expansion);

            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll(settings.Import, settings.Expansion);

            var commands = new ICommand[] {
                new CheckDuplicateSPDATSpellsCommand(eqCasterSpells, reporter, logger)
                , new CheckMissingSpellNamesInPEQCommand(eqCasterSpells, peqSpells, reporter, logger)
                , new CheckDuplicateSpellNameMatchesCommand(eqCasterSpells, peqSpells, reporter, settings.Import, logger)
                , new CreateAutomaticUpdateScript(eqCasterSpells, peqSpells, reporter, sqlReport, settings.Import, logger)
                , new CreateManualUpdateLogCommand(eqCasterSpells, peqSpells, reporter, settings.Import, logger)
            };

            foreach (var command in commands)
            {
                command.Execute();
            }
        }

        static void Deserialize(string fileName)
        {

            //if (File.Exists(fileName))
            //{
            //    using (var stream = File.Open(fileName, FileMode.Open))
            //    {
            //        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            //        {
            //            var chars = reader.ReadChars(689);
            //            var charsStr = new string(chars);
            //        }
            //    }
            //}

            if (File.Exists(@"C:\Development\gates-of-time\FVProject-spdat-importer\DataFiles\spdat.2001.08.22.eff"))
            {
                using (var stream = File.Open(@"C:\Development\gates-of-time\FVProject-spdat-importer\DataFiles\spdat.2001.08.22.eff", FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        var id = reader.ReadInt32();
                        var chars = reader.ReadChars(689);
                        var charsStr = new string(chars);
                    }
                }
            }

            var spells = Deserializestring(@"C:\Development\gates-of-time\FVProject-spdat-importer\DataFiles\spdat.2001.08.22.eff");
            // To prove that the table deserialized correctly,
            // display the key/value pairs.
            foreach (DictionaryEntry de in spells)
            {
                Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
            }
        }

        static Hashtable Deserializestring(string fileName)
        {
            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(fileName, FileMode.Open);
            try
            {
                var formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and
                // assign the reference to the local variable.
                return (Hashtable)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            }
            finally
            {
                fs.Close();
            }

            return new Hashtable();
        }
    }
}