#if NET462
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using SQLitePCL;
using Windows.Win32;

namespace NeoScavHelperTool.Framework
{
    // Needed so that SQLite works with Costura and Fody in Framework Targets
    // This whole approach was based in https://stackoverflow.com/a/63887710/3561396
    public class ModuleGetFunctionPointer : IGetFunctionPointer
    {
        private readonly ProcessModule _module;

        public static ProcessModule GetModule(string moduleName)
        {
            var modules = Process
                .GetCurrentProcess()
                .Modules.Cast<ProcessModule>()
                .Where(e => Path.GetFileNameWithoutExtension(e.ModuleName) == moduleName)
                .ToList();
            if (modules.Count == 0)
            {
                throw new ArgumentException(
                    $"Found no modules named '{moduleName}' in the current process.",
                    nameof(moduleName)
                );
            }
            if (modules.Count > 1)
            {
                throw new ArgumentException(
                    $"Found several modules named '{moduleName}' in the current process.",
                    nameof(moduleName)
                );
            }
            return modules[0];
        }

        public ModuleGetFunctionPointer(string moduleName)
            : this(GetModule(moduleName)) { }

        public ModuleGetFunctionPointer(ProcessModule module)
        {
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public IntPtr GetFunctionPointer(string name)
        {
            using (var handle = new SafeProcessHandle(_module.BaseAddress, ownsHandle: false))
            {
                return PInvoke.GetProcAddress(handle, name);
            }
        }
    }
}
#endif
