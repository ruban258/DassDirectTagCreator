using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Siemens.Engineering;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.Library.Types;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;

namespace AGTIAADDIN.Utility
{
    public class Block
    {
        private PlcBlock _block;
        private ExitState _state;
        public string BlockName { get; }
        
        public bool IsChangeable
        {
            get { return _state == ExitState.IsChangeable; }
        }
        public bool IsDataBlock
        {
            get { return _state == ExitState.IsDataBlock;  }
        }
        public bool ChangeSuccessful
        {
            get { return _state == ExitState.Successful; }
        }
        public Block(PlcBlock plcBlock)
        {
            _block = plcBlock;
            BlockName = plcBlock.Name;           
            SetIsDatablock();
        }
        private void SetIsDatablock()
        {
            if (_block is DataBlock)
            {
                _state = ExitState.IsDataBlock;
            }
        }
        private static string RemoveInvalidFileNameChars(string name)
        {
            Path.GetInvalidFileNameChars().ToList().ForEach(c => name = name.Replace(c.ToString(), "_"));
            return name;
        }
        public void CreateTags()
        {
            var dirPath = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);
            var dir = Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dir.FullName, RemoveInvalidFileNameChars(BlockName) + ".xml");

            if (Util.ExportBlock(_block, filePath) != true)
            {
                _state = ExitState.CouldNotExport;
                return;
            }
            bool changeSuccessful = false;

            changeSuccessful = XmlEdit.CreateCSV(filePath);
            if (!changeSuccessful)
            {
                _state = ExitState.XmlEditingError;
                return;
            }
            else 
            {
                _state = ExitState.Successful;
                return;
            } 
        } 
        public string GetStateText()
        {
            switch (_state)
            {
                case ExitState.BlockIsKnowHowProtected:
                    return "The block is know-how protected.";
                case ExitState.ProgrammingLanguageNotSupported:
                    return "The programming language of the block is not supported.";
                case ExitState.CouldNotCompile:
                    return "The block could not be compiled.";
                case ExitState.CouldNotExport:
                    return "The block could not be exported.";
                case ExitState.CouldNotImport:
                    return "The block could not be imported.";
                case ExitState.IsChangeable:
                    return "The block type is changeable.";
                case ExitState.XmlEditingError:
                    return "Error during editing of SimaticML file";
                case ExitState.IsLibraryType:
                    return "Library types are not supported.";
                case ExitState.Successful:
                    return "The block type was changed successfully.";
                default:
                    return "";
            }
        }

        public string GetActionText()
        {
            switch (_state)
            {
                case ExitState.BlockIsKnowHowProtected:
                    return "Remove the know-how protection.";
                case ExitState.ProgrammingLanguageNotSupported:
                    return "Change the programming language of the block.";
                case ExitState.CouldNotCompile:
                    return "Compile the block without errors.";
                case ExitState.CouldNotExport:
                    return "Please report this issue for further investigation.";
                case ExitState.CouldNotImport:
                    return "Please report this issue for further investigation.";
                case ExitState.IsChangeable:
                    return "No action required.";
                case ExitState.XmlEditingError:
                    return "Please report this issue for further investigation.";
                case ExitState.IsLibraryType:
                    return "Terminate the library type connection.";
                case ExitState.Successful:
                    return "No action required.";
                default:
                    return "";
            }
        }
    }
}