using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DockerRegistryClient.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                PrintUsage();
                return;
            }

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddCommandLine(args, new Dictionary<string, string>
            {
                {"-X", "ACT"},
                {"-R", "REPO"},
                {"-U", "USER"},
                {"-P", "PWD"}
            });
            var configuration = configurationBuilder.Build();
            var action = configuration["ACT"];
            var defaultColor = Console.ForegroundColor;
            switch (action)
            {
                case "DELETE":
                {
                    var image = configuration["REPO"];
                    if (string.IsNullOrWhiteSpace(image))
                    {
                        PrintUsage();
                        break;
                    }

                    var uri = new Uri(image);
                    var registry = $"{uri.Scheme}://{uri.Authority}";
                    var imageWithTag = uri.AbsolutePath.Substring(1, uri.AbsolutePath.Length - 1);
                    var lastColon = imageWithTag.LastIndexOf(":", StringComparison.Ordinal);
                    var imageName = imageWithTag.Substring(0, lastColon);
                    var tag = imageWithTag.Substring(lastColon + 1, imageWithTag.Length - lastColon - 1);
                    var result = await DockerRegistryClient.DeleteImage(new Uri(registry), imageName, tag,
                        configuration["USER"],
                        configuration["PWD"]);
                    if (result)
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"Delete image {imageName}:{tag} success");
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Delete image {imageName}:{tag} failed");
                        Console.ForegroundColor = defaultColor;
                        Console.WriteLine("");
                    }

                    break;
                }

                case "REPO":
                {
                    var registry = configuration["REPO"];
                    if (string.IsNullOrWhiteSpace(registry))
                    {
                        PrintUsage();
                        break;
                    }
                    var uri = new Uri(registry);
                    var repositories =
                        await DockerRegistryClient.GetRepositories(uri, configuration["USER"], configuration["PWD"]);
                    if (repositories != null)
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"Repositories: {JsonConvert.SerializeObject(repositories)}");
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Fetch repositories failed");
                        Console.ForegroundColor = defaultColor;
                        Console.WriteLine("");
                    }

                    break;
                }

                case "TAG":
                {
                    var image = configuration["REPO"];
                    if (string.IsNullOrWhiteSpace(image))
                    {
                        PrintUsage();
                        break;
                    }
                    var uri = new Uri(image);
                    var registry = $"{uri.Scheme}://{uri.Authority}";
                    var imageName = uri.AbsolutePath.Substring(1, uri.AbsolutePath.Length - 1);
                    var tags =
                        await DockerRegistryClient.GetTags(new Uri(registry), imageName, configuration["USER"],
                            configuration["PWD"]);
                    if (tags != null)
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"Image: {imageName}, Tags: {JsonConvert.SerializeObject(tags)}");
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Fetch image {imageName} tag failed");
                        Console.ForegroundColor = defaultColor;
                        Console.WriteLine("");
                    }

                    break;
                }

                default:
                {
                    PrintUsage();
                    break;
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("");
            Console.WriteLine("Usage: drc [OPTIONS]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -X       DELETE, REPO, TAG");
            Console.WriteLine("  -R       Repository or registry address");
            Console.WriteLine("  -U       User");
            Console.WriteLine("  -P       Password");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("  Delete image:    drc -X DELETE -R http://localhost:5000/test:latest");
            Console.WriteLine("  List repository: drc -X REPO   -R http://localhost:5000");
            Console.WriteLine("  List tag:        drc -X TAG    -R http://localhost:5000/test");
        }
    }
}