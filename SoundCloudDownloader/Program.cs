using Spectre.Console;
using Spectre.Console.Cli;
using SoundCloudDownloader.Enums;

namespace SoundCloudDownloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("SoundCloud Downloader").Centered().Color(Color.Red));

            var mode = AnsiConsole.Prompt(
                new SelectionPrompt<Mode>()
                .Title("Select [green]mode[/]")
                .PageSize(4)
                .AddChoices(Mode.Single.GetValuesArray()));

            switch (mode)
            {
                case Mode.Single:
                    await StartSingle();
                    break;
                case Mode.Multiple:
                    break;
                case Mode.Playlist:
                    break;
                case Mode.Json:
                    break;
            }
        }

        private static async Task StartSingle()
        {
            AnsiConsole.Write(new Rule("Operation mode [blue]Single[/]"));

            var url = GetValidUrlFromUser();

            AnsiConsole.Status()
                .Start("Downloading [blue]music[/]...", ctx =>
                {
                    AnsiConsole.MarkupLine($"Starting download of [green]{url}[/]");

                });
        }

        private static string GetValidUrlFromUser()
        {
            string validUrl = string.Empty;
            bool valid = false;
            while (!valid)
            {
                validUrl = AnsiConsole.Prompt(
                    new TextPrompt<string>("Provide [red]valid[/] [green]URL[/]:"));

                valid = ValidateUrl(validUrl);
                if (!valid)
                    AnsiConsole.MarkupLine("[red]Provided URL is invalid![/]");
            }

            return validUrl;
        }

        private static bool ValidateUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                && uriResult.Host.Contains("soundcloud.com");
        }
    }
}
