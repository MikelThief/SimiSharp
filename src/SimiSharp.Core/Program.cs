using System;
using System.IO;
using System.Threading.Tasks;
using PowerArgs;
using Testura.Code.Compilations;

namespace SimiSharp.Core
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class SimiSharpArguments
    {
        [ArgDescription("Path to reference source file.")]
        [ArgShortcut("-r")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo InputReferenceFilePath { get; set; }

        [ArgDescription("Path to analyzed source file.")]
        [ArgShortcut("-a")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo InputAnalyzedFilePath { get; set; }

        private DirectoryInfo Workspace { get; set; }
        private DirectoryInfo CompiledDirectory { get; set; }
        private DirectoryInfo DecompiledDirectory { get; set; }
        private FileInfo ReferenceBinary { get; set; }
        private FileInfo AnalyzedBinary { get; set; }

        /// <summary>
        /// EntryPoint
        /// </summary>
        public void Main()
        {
            CreateWorkspace();
            CompileFiles();
        }

        private void CompileFiles()
        {
            var compiler = new Compiler();
            var referenceBinaryOutputPath = Path.Combine(path1: CompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(InputReferenceFilePath.FullName)) + ".dll";
            var referenceCompileResult = compiler.CompileFilesAsync(
                outputPath: referenceBinaryOutputPath,
                pathsToCsFiles: InputReferenceFilePath.FullName).ConfigureAwait(false).GetAwaiter().GetResult();
            if (!referenceCompileResult.Success)
            {
                // TODO: Fail
                throw new Exception();
            }
            ReferenceBinary = new FileInfo(fileName: referenceCompileResult.PathToDll);

            var analyzedBinaryOutputPath = Path.Combine(path1: CompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(InputReferenceFilePath.FullName)) + ".dll";
            var analyzedCompileResult = compiler.CompileFilesAsync(
                outputPath: analyzedBinaryOutputPath,
                pathsToCsFiles: InputReferenceFilePath.FullName).ConfigureAwait(false).GetAwaiter().GetResult();
            if (!analyzedCompileResult.Success)
            {
                // TODO: Fail
                throw new Exception();
            }
            AnalyzedBinary = new FileInfo(fileName: analyzedCompileResult.PathToDll);
        }

        private void CreateWorkspace()
        {
            var currentDir = new DirectoryInfo(path: Environment.CurrentDirectory);
            var workspacePath = Path.Combine(path1: currentDir.FullName, path2: "Workspace");

            if(Directory.Exists(path: workspacePath))
            {
                Directory.Delete(path: workspacePath, true);
            }

            Workspace = Directory.CreateDirectory(path: workspacePath);
            var compiledPath = Path.Combine(path1: Workspace.FullName, path2: "Compiled");
            var decompiledPath = Path.Combine(path1: Workspace.FullName, path2: "Decompiled");
            CompiledDirectory = Directory.CreateDirectory(path: compiledPath);
            DecompiledDirectory = Directory.CreateDirectory(path: decompiledPath);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeMain<SimiSharpArguments>(args);
        }
    }
}
