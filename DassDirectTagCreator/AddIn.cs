using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AGTIAADDIN.Utility;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Blocks;

namespace AGTIAADDIN
{
    public class AddIn : ContextMenuAddIn
    {
        private readonly TiaPortal _tiaPortal;        
        private readonly string _traceFilePath;

        public AddIn(TiaPortal tiaPortal) : base("AG TIA ADD-IN")
        {
            _tiaPortal = tiaPortal;
            var assemblyName = Assembly.GetCallingAssembly().GetName();
            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", assemblyName.Name, assemblyName.Version.ToString(), "Logs");
            var logDirectory = Directory.CreateDirectory(logDirectoryPath);
            _traceFilePath = Path.Combine(logDirectory.FullName, string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt"));
        }
        
        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
                     
            addInRootSubmenu.Items.AddActionItem<DataBlock>("Create Tag List", AddInClick);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Please select only Global DB", menuSelectionProvider => { }, InfoTextStatus);           
        }
        
        private void AddInClick(MenuSelectionProvider menuSelectionProvider)
        {

            using (var fileStream = new FileStream(_traceFilePath, FileMode.Append))
            {
                Trace.Listeners.Add(new TextWriterTraceListener(fileStream) {TraceOutputOptions = TraceOptions.DateTime});
                Trace.AutoFlush = true;

                var blocks = new List<Block>();

                foreach (PlcBlock block in menuSelectionProvider.GetSelection())
                {
                    blocks.Add(new Block(block));                  
                    
                }
                foreach (var block in blocks)
                {
                    if (block.IsDataBlock)
                    {
                        block.CreateTags();
                    }
                }                
                
                Trace.Close();
            }

            try
            {
                if (new FileInfo(_traceFilePath).Length == 0)
                {
                    File.Delete(_traceFilePath);
                }
            }
            catch
            {
                // Silently ignore file operations
            }
        }

        private static MenuStatus InfoTextStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            var show = false;

            foreach (IEngineeringObject engineeringObject in menuSelectionProvider.GetSelection())
            {
                if (!(engineeringObject.GetType() == menuSelectionProvider.GetSelection().First().GetType() && (engineeringObject is FB || engineeringObject is FC)))
                {
                    show = true;
                    break;
                }
            }

            return show ? MenuStatus.Disabled : MenuStatus.Hidden;
        }

        
    }
}