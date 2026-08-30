using System.Diagnostics;
using System.IO.Compression;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public static class ClientPackaging
{
    /// <summary>
    /// Be advised this can be called from server packaging during a HybridACZ build.
    /// </summary>
    public static async Task PackageClient(bool skipBuild, string configuration, IPackageLogger logger)
    {
        logger.Info("Building client...");

        if (!skipBuild)
        {
            await ProcessHelpers.RunCheck(new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList =
                {
                    "build",
                    Path.Combine("Content.Client", "Content.Client.csproj"),
                    "-c", configuration,
                    "--nologo",
                    "/v:m",
                    "/t:Rebuild",
                    "/p:FullRelease=true",
                    "/m"
                }
            });
        }

        logger.Info("Verificando seguridad y sandbox de tipos para el cliente...");
        var clientDll = Path.Combine("bin", "Content.Client", "Content.Client.dll");
        var sharedDll = Path.Combine("bin", "Content.Client", "Content.Shared.dll");
        var clientBinDir = Path.Combine("bin", "Content.Client");

        if (!File.Exists(clientDll))
            clientDll = Path.Combine("Content.Client", "bin", configuration, "net10.0", "Content.Client.dll");
        if (!File.Exists(sharedDll))
            sharedDll = Path.Combine("Content.Client", "bin", configuration, "net10.0", "Content.Shared.dll");
        if (!Directory.Exists(clientBinDir))
            clientBinDir = Path.GetDirectoryName(clientDll)!;

        var searchDirs = new[] { clientBinDir, Path.Combine("bin", "Client") };

        if (!RobustSandboxVerifier.VerifyAssembly(sharedDll, logger, searchDirs)
            || !RobustSandboxVerifier.VerifyAssembly(clientDll, logger, searchDirs))
        {
            throw new Exception("¡FALLÓ LA VERIFICACIÓN DE SEGURIDAD DEL CLIENTE! Hay tipos o llamadas prohibidas por el sandbox.");
        }

        logger.Info("Packaging client...");

        var sw = RStopwatch.StartNew();
        {
            await using var zipFile =
                File.Open(Path.Combine("release", "SS14.Client.zip"), FileMode.Create, FileAccess.ReadWrite);
            using var zip = new ZipArchive(zipFile, ZipArchiveMode.Update);
            var writer = new AssetPassZipWriter(zip);

            await WriteResources("", writer, logger, default);
            await writer.FinishedTask;
        }

        logger.Info($"Finished packaging client in {sw.Elapsed}");
    }

    public static async Task WriteResources(
        string contentDir,
        AssetPass pass,
        IPackageLogger logger,
        CancellationToken cancel)
    {
        var graph = new RobustClientAssetGraph();
        pass.Dependencies.Add(new AssetPassDependency(graph.Output.Name));

        var dropSvgPass = new AssetPassFilterDrop(f => f.Path.EndsWith(".svg"))
        {
            Name = "DropSvgPass",
        };
        dropSvgPass.AddDependency(graph.Input).AddBefore(graph.PresetPasses);

        AssetGraph.CalculateGraph([pass, dropSvgPass, ..graph.AllPasses], logger);

        var inputPass = graph.Input;

        await RobustSharedPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Content.Client",
            new[] { "Content.Client", "Content.Shared", "Content.Shared.Database" },
            cancel: cancel);

        await RobustClientPackaging.WriteClientResources(
            contentDir,
            inputPass,
            SharedPackaging.AdditionalIgnoredResources,
            cancel);

        inputPass.InjectFinished();
    }
}
