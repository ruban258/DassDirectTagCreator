using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using System.Windows.Forms;

namespace AGTIAADDIN.Utility
{
    public static class Util
    {
        public static bool ExportBlock(PlcBlock block, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                block.Export(new FileInfo(filePath), ExportOptions.WithReadOnly);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception during export:" + Environment.NewLine + ex);
                return false;
            }
            return true;
        }
        public static Form GetForegroundWindow()
        {
            // Workaround for Add-In Windows to be shown in foreground of TIA Portal
            var form = new Form { Opacity = 0, ShowIcon = false };
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }
    }
}