using Spectre.Console;
using Spectre.Console.Cli;
using SoundCloudDownloader.Enums;
using System;
using SoundCloudDownloader.SoundCloud;

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
                case Mode.Test:
                    AnsiConsole.WriteLine(await new SoundCloudClient().TestOAuthAsync());
                    break;
            }
        }

        private static async Task StartSingle()
        {
            AnsiConsole.Write(new Rule("Operation mode [blue]Single[/]"));

            var url = GetValidUrlFromUser();

            try
            {
                await DownloadTrack(url);
                AnsiConsole.MarkupLine($"[green]Finished[/] downloading [green]{url}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        private static async Task DownloadTrack(string trackUrl)
        {
            await AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    var downloadTask = ctx.AddTask(trackUrl, new ProgressTaskSettings
                    {
                        AutoStart = false,
                    });

                    await DownloadTrackWithProgress(downloadTask, trackUrl);
                });
        }

        private static async Task DownloadTrackWithProgress(ProgressTask task, string trackUrl)
        {
            var response = await new SoundCloudClient().StartTrackDownloadAsync(trackUrl);
            task.MaxValue = response.TotalBytes;
            task.StartTask();

            AnsiConsole.MarkupLine($"[blue]Starting[/] download of [green]{trackUrl}[/]");
            var filename = trackUrl.Substring(trackUrl.LastIndexOf('/') + 1) + ".mp3";

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true)) 
            {
                var buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;

                    task.Increment(bytesRead);
                }
            }
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
