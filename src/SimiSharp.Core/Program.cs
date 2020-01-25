using System;
using System.IO;
using System.Threading.Tasks;
using PowerArgs;

namespace SimiSharp.Core
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class SimiSharpArguments
    {
        [ArgDescription("Path to reference source file.")]
        [ArgShortcut("-r")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo ReferenceFilePath { get; set; }

        [ArgDescription("Path to analyzed source file.")]
        [ArgShortcut("-a")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo AnalyzedFilePath { get; set; }

        public void Main()
        {
            Console.WriteLine(value: "hello");
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
