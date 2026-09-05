using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Robust.Packaging;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Packaging;

public static class RobustSandboxVerifier
{
    public static bool VerifyAssembly(string assemblyPath, IPackageLogger logger, string[]? extraSearchDirs = null)
    {
        if (!File.Exists(assemblyPath))
        {
            logger.Error($"[SANDBOX ERROR] No se encontró el archivo de ensamblado: {assemblyPath}");
            return false;
        }

        var logManager = new LogManager();
        logManager.RootSawmill.AddHandler(new ConsoleLogHandler());
        var sawmill = logManager.GetSawmill("res.typecheck");

        var checkerType = typeof(IResourceManager).Assembly.GetType("Robust.Shared.ContentPack.AssemblyTypeChecker");
        if (checkerType == null)
        {
            logger.Error("[SANDBOX ERROR] No se pudo encontrar el tipo AssemblyTypeChecker.");
            return false;
        }

        var checker = Activator.CreateInstance(checkerType, new DummyResourceManager(), sawmill);
        if (checker == null)
        {
            logger.Error("[SANDBOX ERROR] No se pudo instanciar AssemblyTypeChecker.");
            return false;
        }

        var searchDirsList = new List<string>();
        var dir = Path.GetDirectoryName(Path.GetFullPath(assemblyPath));
        if (!string.IsNullOrEmpty(dir))
            searchDirsList.Add(dir);
        if (extraSearchDirs != null)
            searchDirsList.AddRange(extraSearchDirs);
        var allDirs = searchDirsList.Distinct().ToArray();

        checkerType.GetProperty("VerifyIL")?.SetValue(checker, true);
        checkerType.GetProperty("DisableTypeCheck")?.SetValue(checker, false);
        checkerType.GetField("EngineModuleDirectories")?.SetValue(checker, allDirs);
        checkerType.GetProperty("EngineModuleDirectories")?.SetValue(checker, allDirs);

        var checkMethod = checkerType.GetMethod("CheckAssembly", new[] { typeof(Stream) });
        if (checkMethod == null)
        {
            logger.Error("[SANDBOX ERROR] No se pudo encontrar el método CheckAssembly en AssemblyTypeChecker.");
            return false;
        }

        try
        {
            using var stream = File.OpenRead(assemblyPath);
            var result = (bool?)checkMethod.Invoke(checker, new object[] { stream });
            if (result != true)
            {
                logger.Error($"[SANDBOX ERROR] El ensamblado {Path.GetFileName(assemblyPath)} FALLÓ la verificación de seguridad/sandbox.");
                return false;
            }

            logger.Info($"[SANDBOX OK] El ensamblado {Path.GetFileName(assemblyPath)} pasó la verificación de seguridad/sandbox correctamente.");
            return true;
        }
        catch (Exception ex)
        {
            var realEx = ex is TargetInvocationException tie && tie.InnerException != null ? tie.InnerException : ex;
            logger.Error($"[SANDBOX ERROR] Excepción al verificar {Path.GetFileName(assemblyPath)}: {realEx.GetType().FullName}: {realEx.Message}\n{realEx.StackTrace}");
            return false;
        }
    }

    private sealed class DummyResourceManager : IResourceManager
    {
        public IWritableDirProvider UserData => throw new NotImplementedException();
        public void AddRoot(ResPath prefix, IContentRoot loader) => throw new NotImplementedException();
        public Stream ContentFileRead(ResPath path) => throw new FileNotFoundException();
        public Stream ContentFileRead(string path) => throw new FileNotFoundException();
        public bool ContentFileExists(ResPath path) => false;
        public bool ContentFileExists(string path) => false;
        public bool TryContentFileRead(ResPath? path, [NotNullWhen(true)] out Stream? fileStream) { fileStream = null; return false; }
        public bool TryContentFileRead(string path, [NotNullWhen(true)] out Stream? fileStream) { fileStream = null; return false; }
        public IEnumerable<ResPath> ContentFindFiles(ResPath? path) => Array.Empty<ResPath>();
        public IEnumerable<ResPath> ContentFindFiles(string path) => Array.Empty<ResPath>();
        public IEnumerable<string> ContentGetDirectoryEntries(ResPath path) => Array.Empty<string>();
        public IEnumerable<ResPath> GetContentRoots() => Array.Empty<ResPath>();
    }
}

