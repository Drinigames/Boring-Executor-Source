using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boring_Executor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            QuorumModule.AutoUpdate();

            // ======= Premium configuration =======
            //QuorumModule.SetInfo("ExecutorName", "v1.4.8");
            //QuorumModule.UseDiscordRPC(false);


            //QuorumModule.SetAttachNotification("Boring Executor", "Successfully Attached!");
            // =====================================
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QuorumModule.ExecuteScriptAll(richTextBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            QuorumModule.AttachAll();
        }
    }

    public static class QuorumModule
    {
        private const string Dll = "Quorum-External-Module.dll";
        private const CallingConvention Cc = CallingConvention.Cdecl;

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "AttachAll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool AttachAll();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "Attach")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Attach(uint pid);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "IsAttached")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsAttached();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "IsAttachedByPid")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsAttachedByPid(uint pid);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "Detach")]
        public static extern void Detach();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "DetachByPid")]
        public static extern void DetachByPid(uint pid);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "GetClientsList")]
        private static extern IntPtr GetClientsListRaw();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "FreeString")]
        private static extern void FreeString(IntPtr p);

        public static List<(uint Pid, bool Attached)> GetClients()
        {
            var result = new List<(uint, bool)>();
            IntPtr p = GetClientsListRaw();
            if (p == IntPtr.Zero) return result;

            try
            {
                string s = Marshal.PtrToStringAnsi(p) ?? string.Empty;
                foreach (var line in s.Split('\n'))
                {
                    if (line.Length == 0) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;
                    if (uint.TryParse(parts[0], out uint pid))
                        result.Add((pid, parts[1] == "1"));
                }
            }
            finally
            {
                FreeString(p);
            }
            return result;
        }

        public static void ReleaseString(IntPtr p) => FreeString(p);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "ExecuteScriptAll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ExecuteScriptAll([MarshalAs(UnmanagedType.LPStr)] string script);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "Execute")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Execute(uint pid, [MarshalAs(UnmanagedType.LPStr)] string script);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "AllowMultiInstance")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool AllowMultiInstance([MarshalAs(UnmanagedType.I1)] bool allow);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "OpenRoblox")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool OpenRoblox();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "IsMultiInstanceAllowed")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsMultiInstanceAllowed();

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "AutoUpdate")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool AutoUpdate();

        // ======= Premium Only ========

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "SetInfo", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetInfo(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string version);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "UseDiscordRPC")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool UseDiscordRPC([MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "SetAttachNotification")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetAttachNotification(
            [MarshalAs(UnmanagedType.LPStr)] string title,
            [MarshalAs(UnmanagedType.LPStr)] string text);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "SetMainFolder")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetMainFolder([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "SetWorkspace")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetWorkspace([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(Dll, CallingConvention = Cc, EntryPoint = "SetAutoexecute")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetAutoexecute([MarshalAs(UnmanagedType.LPStr)] string path);
    }
}
