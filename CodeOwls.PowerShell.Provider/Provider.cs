/*
   Copyright (c) 2011 Code Owls LLC, All Rights Reserved.

   Licensed under the Microsoft Reciprocal License (Ms-RL) (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.opensource.org/licenses/ms-rl

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using CodeOwls.PowerShell.Paths.Exceptions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.Attributes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Provider
{
    //[CmdletProvider("YourProviderName", ProviderCapabilities.ShouldProcess)]
    [LogProviderToSession]
    public abstract class Provider : NavigationCmdletProvider, 
        IPropertyCmdletProvider, ICmdletProviderSupportsHelp
    {
        private IPathNodeProcessor _pathNodeProcessor;

        internal Drive Drive
        {
            get
            {
                var drive = PSDriveInfo as Drive;

                if (null == drive)
                {
                    drive = ProviderInfo.Drives.FirstOrDefault() as Drive;
                }

                return drive;
            }
        }

 
        protected abstract IPathNodeProcessor PathNodeProcessor { get; }
        
        INodeFactory ResolvePath( string path )
        {
            path = Drive.EnsurePathIsRooted(path);
            return PathNodeProcessor.ResolvePath(path);
        }

        protected virtual Context Context
        {
            get
            {                
                return new Context( this, PathNodeProcessor, DynamicParameters );
            }
        }

        #region Implementation of IPropertyCmdletProvider

        public void GetProperty(string path, Collection<string> providerSpecificPickList)
        {
            LogVerbose("GetProperty( {0} )", path);
            
            var factory = GetNodeFactoryFromPath(path);
            var node = factory.GetNodeValue();
            PSObject values = null;

            if (null == providerSpecificPickList || 0 == providerSpecificPickList.Count)
            {
                values = PSObject.AsPSObject(node.Item);
            }
            else
            {
                values = new PSObject();
                var value = node.Item;
                var propDescs = TypeDescriptor.GetProperties(value);
                var props = (from PropertyDescriptor prop in propDescs
                             where (providerSpecificPickList.Contains(prop.Name,
                                                                      StringComparer.InvariantCultureIgnoreCase))
                             select prop);

                props.ToList().ForEach(p =>
                                           {
                                               var iv = p.GetValue(value);
                                               if (null != iv)
                                               {
                                                   values.Properties.Add(new PSNoteProperty(p.Name, p.GetValue(value)));
                                               }
                                           });
            }
            WritePropertyObject(values, path);
        }

        public object GetPropertyDynamicParameters(string path, Collection<string> providerSpecificPickList)
        {
            return null;
        }

        public void SetProperty(string path, PSObject propertyValue)
        {
            var factory = GetNodeFactoryFromPath(path);
            var node = factory.GetNodeValue();
            var value = node.Item;
            var propDescs = TypeDescriptor.GetProperties(value);
            var psoDesc = propertyValue.Properties;
            var props = (from PropertyDescriptor prop in propDescs
                         let psod = (from pso in psoDesc
                                     where StringComparer.InvariantCultureIgnoreCase.Equals(pso.Name, prop.Name)
                                     select pso).FirstOrDefault()
                         where null != psod
                         select new {PSProperty = psod, Property = prop});


            props.ToList().ForEach(p => p.Property.SetValue(value, p.PSProperty.Value));
        }

        public object SetPropertyDynamicParameters(string path, PSObject propertyValue)
        {
            return null;
        }

        public void ClearProperty(string path, Collection<string> propertyToClear)
        {
            WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.ClearItemProperty, ClearItemPropertyNotsupportedErrorID);
        }

        public object ClearPropertyDynamicParameters(string path, Collection<string> propertyToClear)
        {
            return null;
        }

        #endregion


        #region Implementation of ICmdletProviderSupportsHelp

        public string GetHelpMaml(string helpItemName, string path)
        {
            LogVerbose("GetHelpMaml( {0}, {1} )", helpItemName, path);

            if (String.IsNullOrEmpty(helpItemName) || String.IsNullOrEmpty(path))
            {
                return String.Empty;
            }

            var parts = helpItemName.Split(new[] {'-'});
            if (2 != parts.Length)
            {
                return String.Empty;
            }

            var nodeFactory = ResolvePath(path);
            if (null == nodeFactory)
            {
                return String.Empty;
            }

            XmlDocument document = new XmlDocument();

            string filename = GetExistingHelpDocumentFilename();

            if (String.IsNullOrEmpty(filename) ||
                !File.Exists(filename))
            {
                return String.Empty;
            }

            try
            {
                document.Load(filename);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                               new MamlHelpDocumentExistsButCannotBeLoadedException(filename, e),
                               GetHelpCustomMamlErrorID,
                               ErrorCategory.ParserError,
                               filename
                               ));

                return string.Empty;
            }

            List<string> keys = GetCmdletHelpKeysForNodeFactory(nodeFactory);

            string verb = parts[0];
            string noun = parts[1];
            string maml = (from key in keys
                           let m = GetHelpMaml(document, key, verb, noun)
                           where !String.IsNullOrEmpty(m)
                           select m).FirstOrDefault();

            if (String.IsNullOrEmpty(maml))
            {
                maml = GetHelpMaml(document, NotSupportedCmdletHelpID, verb, noun);
            }
            return maml ?? String.Empty;
        }

        private List<string> GetCmdletHelpKeysForNodeFactory(INodeFactory nodeFactory)
        {
            var nodeFactoryType = nodeFactory.GetType();
            var idsFromAttributes =
                from CmdletHelpPathIDAttribute attr in
                    nodeFactoryType.GetCustomAttributes(typeof (CmdletHelpPathIDAttribute), true)
                select attr.ID;

            List<string> keys = new List<string>(idsFromAttributes);
            keys.AddRange(new[]
                              {
                                  nodeFactoryType.FullName,
                                  nodeFactoryType.Name,
                                  nodeFactoryType.Name.Replace("NodeFactory", ""),
                              });
            return keys;
        }

        private string GetExistingHelpDocumentFilename()
        {
            CultureInfo currentUICulture = Host.CurrentUICulture;
            string moduleLocation = this.ProviderInfo.Module.ModuleBase; 
            string filename = null;
            while (currentUICulture != null && currentUICulture != currentUICulture.Parent)
            {
                string helpFilePath = GetHelpPathForCultureUI(currentUICulture.Name, moduleLocation);

                if (File.Exists(helpFilePath))
                {
                    filename = helpFilePath;
                    break;
                }
                currentUICulture = currentUICulture.Parent;
            }

            if (String.IsNullOrEmpty(filename))
            {
                string helpFilePath = GetHelpPathForCultureUI("en-US", moduleLocation);

                if (File.Exists(helpFilePath))
                {
                    filename = helpFilePath;
                }
            }

            LogVerbose( "Existing help document filename: {0}", filename);
            return filename;
        }

        private string GetHelpPathForCultureUI(string cultureName, string moduleLocation)
        {
            string documentationDirectory = Path.Combine(moduleLocation, cultureName);
            var path = Path.Combine(documentationDirectory, ProviderInfo.HelpFile);

            LogVerbose("GetHelpPathForCultureUI( {0}, {1} ) => {2}", cultureName, moduleLocation, path);
            return path;
        }

        private string GetHelpMaml(XmlDocument document, string key, string verb, string noun)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("cmd", "http://schemas.microsoft.com/maml/dev/command/2004/10");

            string xpath = String.Format(
                "/helpItems/providerHelp/CmdletHelpPaths/CmdletHelpPath[@ID='{0}']/cmd:command[ ./cmd:details[ (cmd:verb='{1}') and (cmd:noun='{2}') ] ]",
                key,
                verb,
                noun);

            XmlNode node = null;
            try
            {
                node = document.SelectSingleNode(xpath, nsmgr);
            }
            catch (XPathException)
            {
                return string.Empty;
            }

            if (node == null)
            {
                return String.Empty;
            }

            return node.OuterXml;
        }

        #endregion

        private void WriteCmdletNotSupportedAtNodeError(string path, string cmdlet, string errorId)
        {
            var exception = new NodeDoesNotSupportCmdletException(path, cmdlet);
            var error = new ErrorRecord(exception, errorId, ErrorCategory.NotImplemented, path);
            WriteError(error);
        }

        private void WriteGeneralCmdletError(Exception exception, string errorId, string path)
        {
            WriteError(
                new ErrorRecord(
                    exception,
                    errorId,
                    ErrorCategory.NotSpecified,
                    path
                    ));
        }

        protected override bool IsItemContainer(string path)
        {
            LogVerbose("IsItemContainer( {0} )", path);

            if (IsRootPath(path))
            {
                return true;
            }

            var node = ResolvePath(path);
            if (null == node)
            {
                return false;
            }

            return node.GetNodeValue().IsCollection;
        }

        protected override object MoveItemDynamicParameters(string path, string destination)
        {
            LogVerbose("MoveItemDynamicParameters( {0}, {1} )", path, destination);

            return null;
        }

        protected override void MoveItem(string path, string destination)
        {
            LogVerbose("MoveItem( {0}, {1} )", path, destination);
            var sourceNode = GetNodeFactoryFromPath(path);
            var copy = GetCopyItem(sourceNode);
            var remove = copy as IRemoveItem;
            if (null == copy || null == remove)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.MoveItem, MoveItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.MoveItem ))
            {
                return;
            }

            try
            {
                DoCopyItem(path, destination, true, copy);
                DoRemoveItem(path, true, remove);
            }
            catch( Exception e )
            {
                WriteGeneralCmdletError( e, MoveItemInvokeErrorID, path);
            }
        }

        protected override string MakePath(string parent, string child)
        {
            LogVerbose("MakePath( {0}, {1} )", parent, child);
            
            var newPath = base.MakePath(parent, child);
            return newPath;
        }

        protected override string GetParentPath(string path, string root)
        {
            path = base.GetParentPath(path, root);
            return path;
        }
        
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            LogVerbose("NormalizeRelativePath( {0}, {1} )", path, basePath);
            return base.NormalizeRelativePath(path, basePath);
        }

        protected override string GetChildName(string path)
        {
            LogVerbose("GetChildName( {0} )", path);
            path = MakePath(Drive.Name + ":/" + Drive.CurrentLocation, path).TrimEnd('/');
            path = path.Replace('\\', '/');
            return path.Split('/').Last();
        }

        protected override void GetItem(string path)
        {
            LogVerbose("GetItem( {0} )", path);
            var factory = ResolvePath(path);
            if (null == factory)
            {
                return;
            }

            try
            {
                WritePathNode(path, factory);
            }
            catch( Exception e)
            {
                WriteGeneralCmdletError( e, GetItemInvokeErrorID, path );
            }
        }

        protected override void SetItem(string path, object value)
        {
            LogVerbose("SetItem( {0} )", path);

            var factory = GetNodeFactoryFromPath(path);
            var @set = factory as ISetItem;
            if (null == factory || null == @set)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.SetItem, SetItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            path = SessionState.Path.ParseChildName(path);

            if (!ShouldProcess(fullPath, ProviderCmdlet.SetItem))
            {
                return;
            }

            try
            {
                var result = @set.SetItem(Context, path, value);
                if (null != result)
                {
                    WritePathNode(path, result);
                }
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError( e, SetItemInvokeErrorID, fullPath );
            }
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {
            LogVerbose("SetItemDynamicParameters( {0} )", path);

            var factory = GetNodeFactoryFromPath(path);
            var @set = factory as ISetItem;
            if (null == factory || null == @set)
            {
                return null;
            }

            return @set.SetItemParameters;
        }

        protected override object ClearItemDynamicParameters(string path)
        {
            LogVerbose("ClearItemDynamicParameters( {0} )", path);

            var factory = GetNodeFactoryFromPath(path);
            var clear = factory as IClearItem;
            if (null == factory || null == clear)
            {
                return null;
            }

            return clear.ClearItemDynamicParamters;
        }

        protected override void ClearItem(string path)
        {
            LogVerbose("ClearItem( {0} )", path);

            var factory = GetNodeFactoryFromPath(path);
            var clear = factory as IClearItem;
            if (null == factory || null == clear)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.ClearItem, ClearItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            path = SessionState.Path.ParseChildName(path);

            if (!ShouldProcess(fullPath, ProviderCmdlet.ClearItem))
            {
                return;
            }

            try
            {
                clear.ClearItem(Context, path);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, ClearItemInvokeErrorID, fullPath);
            }
        }

        private bool ForceOrShouldContinue(INodeFactory factory, string fullPath, string op)
        {
            return ForceOrShouldContinue(factory.Name, fullPath, op);
        }

        private bool ForceOrShouldContinue(string itemName, string fullPath, string op)
        {
            if (Force || !ShouldContinue(ShouldContinuePrompt, String.Format("{2} {0} ({1})", itemName, fullPath, op)))
            {
                return false;
            }
            return true;
        }

        protected override object InvokeDefaultActionDynamicParameters(string path)
        {
            LogVerbose("InvokeItemDynamicParameters( {0} )", path);
            var factory = ResolvePath(path);
            var invoke = factory as IInvokeItem;
            if (null == factory || null == invoke)
            {
                return null;
            }

            return invoke.InvokeItemParameters;
        }

        protected override void InvokeDefaultAction(string path)
        {
            LogVerbose("InvokeItem( {0} )", path);
            var factory = ResolvePath(path);
            var invoke = factory as IInvokeItem;
            if (null == factory || null == invoke)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.InvokeItem, InvokeItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;

            if (!ShouldProcess(fullPath, ProviderCmdlet.InvokeItem))
            {
                return;
            }

            path = SessionState.Path.ParseChildName(path);
            try
            {
                var results = invoke.InvokeItem(Context, path);
                if (null == results)
                {
                    return;
                }

                results.ToList().ForEach(r => WriteItemObject(r, path, false));
            }
            catch( Exception e )
            {
                WriteGeneralCmdletError(e, InvokeItemInvokeErrorID, fullPath);
            }
        }

        protected override bool ItemExists(string path)
        {
            LogVerbose("ItemExists( {0} )", path);
            if (IsRootPath(path))
            {
                return true;
            }

            if (null != Drive)
            {
                return null != ResolvePath(path);
            }

            return false;
        }

        protected override bool IsValidPath(string path)
        {
            LogVerbose("IsValidPath( {0} )", path);
            return true;
        }


        protected override void GetChildItems(string path, bool recurse)
        {
            LogVerbose("GetChildItems( {0}, {1} )", path, recurse);
            INodeFactory nodeFactory = ResolvePath(path);
            if (null == nodeFactory)
            {
                return;
            }

            var children = nodeFactory.GetNodeChildren();
            WriteChildItem(path, recurse, children);
        }

        private void WriteChildItem(string path, bool recurse, IEnumerable<INodeFactory> children)
        {
            if (null == children || 0 == children.Count())
            {
                return;
            }

            children.ToList().ForEach(
                f =>
                    {
                        try
                        {
                            var i = f.GetNodeValue();
                            if (null == i)
                            {
                                return;
                            }
                            var childPath = path + "/" + i.Name;
                            WritePathNode(childPath, f);
                            if (recurse)
                            {
                                var kids = f.GetNodeChildren();
                                WriteChildItem(path + "/" + i.Name, recurse, kids);
                            }
                        }
                        catch 
                        {
                        }
                    });
        }


        private bool IsRootPath(string path)
        {
            path = Regex.Replace(path.ToLower(), @"[a-z0-9_]+:[/\\]?", "");
            return String.IsNullOrEmpty(path);
        }

        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            return null;
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            LogVerbose("GetChildNames( {0}, {1} )", path, returnContainers);
            INodeFactory nodeFactory = ResolvePath(path);
            if (null == nodeFactory)
            {
                return;
            }

            nodeFactory.GetNodeChildren().ToList().ForEach(
                f =>
                    {
                        var i = f.GetNodeValue();
                        if (null == i)
                        {
                            return;
                        }
                        WriteItemObject(i.Name, path, i.IsCollection);
                    });
        }

        protected override object GetChildNamesDynamicParameters(string path)
        {
            return null;
        }

        protected override void RenameItem(string path, string newName)
        {
            LogVerbose("RenameItem( {0}, {1} )", path, newName);
            var factory = GetNodeFactoryFromPath(path);
            var rename = factory as IRenameItem;
            if (null == factory || null == rename)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.RenameItem, RenameItemNotsupportedErrorID);
                return;
            }

            var fullPath = path;

            if (!ShouldProcess(fullPath, ProviderCmdlet.RenameItem))
            {
                return;
            }

            var child = SessionState.Path.ParseChildName(path);
            try
            {
                rename.RenameItem(Context, child, newName);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, RenameItemInvokeErrorID, fullPath);
            }
        }

        protected override object RenameItemDynamicParameters(string path, string newName)
        {
            LogVerbose("RenameItemDynamicParameters( {0}, {1} )", path, newName);
            var factory = ResolvePath(path);
            var rename = factory as IRenameItem;
            if (null == factory || null == rename)
            {
                return null;
            }
            return rename.RenameItemParameters;
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            LogVerbose("NewItem( {0}, {1}, {2} )", path, itemTypeName, newItemValue);
            var factory = GetNodeFactoryFromPathOrParent(path);
            var @new = factory as INewItem;
            if (null == factory || null == @new)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.NewItem, NewItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            var child = SessionState.Path.ParseChildName(path);

            if (!ShouldProcess(fullPath, ProviderCmdlet.NewItem))
            {
                return;
            }
            
            try
            {
                var item = @new.NewItem(Context, child, itemTypeName, newItemValue);
                PathNode node = item as PathNode;
                WritePathNode(path, node);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, NewItemInvokeErrorID, fullPath);
            }
        }

        private void WritePathNode(string path, INodeFactory factory)
        {
            var value = factory.GetNodeValue();
            if (null == value)
            {
                return;
            }

            PSObject pso = PSObject.AsPSObject(value.Item);
            pso.Properties.Add(new PSNoteProperty(ItemModePropertyName, factory.ItemMode));
            WriteItemObject(pso, path, value.IsCollection);
        }

        private void WritePathNode(string path, IPathNode node)
        {
            if (null != node)
            {
                WriteItemObject(node.Item, path, node.IsCollection);
            }
        }

        protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
        {
            LogVerbose("NewItemDynamicParameters( {0}, {1}, {2} )", path, itemTypeName, newItemValue);

            var factory = GetNodeFactoryFromPath(path);
            var @new = factory as INewItem;
            if (null == factory || null == @new)
            {
                return null;
            }

            return @new.NewItemParameters;
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            LogVerbose("RemoveItem( {0}, {1} )", path, recurse);
            var factory = GetNodeFactoryFromPath(path);
            var remove = factory as IRemoveItem;
            if (null == factory || null == remove)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.RemoveItem, RemoveItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.RemoveItem))
            {
                return;
            }
            
            try
            {
                DoRemoveItem(path, recurse, remove);                
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, RemoveItemInvokeErrorID, path);
            }
        }

        private void DoRemoveItem(string path, bool recurse, IRemoveItem remove)
        {
            path = SessionState.Path.ParseChildName(path);
            remove.RemoveItem(Context, path, recurse);
        }

        protected override object RemoveItemDynamicParameters(string path, bool recurse)
        {
            LogVerbose("RemoveItemDynamicParameters( {0}, {1} )", path, recurse);
            var factory = GetNodeFactoryFromPath(path);
            var remove = factory as IRemoveItem;
            if (null == factory || null == remove)
            {
                return null;
            }

            return remove.RemoveItemParameters;
        }

        private INodeFactory GetNodeFactoryFromPath(string path)
        {
            INodeFactory factory = null;
            factory = ResolvePath(path);

            return factory;
        }

        private INodeFactory GetNodeFactoryFromPathOrParent(string path)
        {
            bool r;
            return GetNodeFactoryFromPathOrParent(path, out r);
        }

        private INodeFactory GetNodeFactoryFromPathOrParent(string path, out bool isParentOfPath)
        {
            isParentOfPath = false;
            INodeFactory factory = null;
            factory = ResolvePath(path);
            if (null == factory)
            {
                path = SessionState.Path.ParseParent(path, null);
                factory = ResolvePath(path);

                if (null == factory)
                {
                    return null;
                }

                isParentOfPath = true;
            }
            return factory;
        }

        protected override bool HasChildItems(string path)
        {
            LogVerbose("HasChildItems( {0} )", path);
            var factory = ResolvePath(path);
            if (null == factory)
            {
                return false;
            }
            var nodes = factory.GetNodeChildren();
            if (null == nodes)
            {
                return false;
            }
            return nodes.Any();
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            LogVerbose("CopyItem( {0}, {1}, {2} )", path, copyPath, recurse);

            INodeFactory sourceNode = GetNodeFactoryFromPath(path);
            ICopyItem copyItem = GetCopyItem(sourceNode);
            if (null == copyItem)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.CopyItem, CopyItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.CopyItem ))
            {
                return;
            }
            
            try
            {
                IPathNode node = DoCopyItem(path, copyPath, recurse, copyItem);
                WritePathNode(copyPath, node);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, CopyItemInvokeErrorID, path);
            }
        }

        private ICopyItem GetCopyItem(INodeFactory sourceNode)
        {
            var copyItem = sourceNode as ICopyItem;
            if (null == sourceNode || null == copyItem)
            {
                return null;
            }

            return copyItem;
        }

        private IPathNode DoCopyItem(string path, string copyPath, bool recurse, ICopyItem copyItem)
        {
            bool targetNodeIsParentNode;
            var targetNode = GetNodeFactoryFromPathOrParent(copyPath, out targetNodeIsParentNode);

            var sourceName = SessionState.Path.ParseChildName(path);
            var copyName = targetNodeIsParentNode ? SessionState.Path.ParseChildName(copyPath) : null;

            if (null == targetNode)
            {
                WriteError(
                    new ErrorRecord(
                        new CopyOrMoveToNonexistentContainerException(copyPath),
                        CopyItemDestinationContainerDoesNotExistErrorID,
                        ErrorCategory.WriteError,
                        copyPath
                        )
                    );
                return null;
            }

            return copyItem.CopyItem(Context, sourceName, copyName, targetNode.GetNodeValue(), recurse);
        }

        protected override object CopyItemDynamicParameters(string path, string destination, bool recurse)
        {
            LogVerbose("CopyItemDynamicParameters( {0}, {1}, {2} )", path, destination, recurse);
            var factory = GetNodeFactoryFromPath(path);
            var copy = factory as ICopyItem;
            if (null == factory || null == copy)
            {
                return null;
            }

            return copy.CopyItemParameters;
        }


        protected void LogVerbose(string format, params object[] args)
        {
            try
            {
                WriteVerbose(String.Format(format, args));
            }
            catch
            {
            }
        }

        private const string NotSupportedCmdletHelpID = "__NotSupported__";
        private const string RenameItemNotsupportedErrorID = "RenameItem.NotSupported";
        private const string RenameItemInvokeErrorID = "RenameItem.Invoke";
        private const string NewItemNotSupportedErrorID = "NewItem.NotSupported";
        private const string NewItemInvokeErrorID = "NewItem.Invoke";
        private const string ItemModePropertyName = "SSItemMode";
        private const string RemoveItemNotSupportedErrorID = "RemoveItem.NotSupported";
        private const string RemoveItemInvokeErrorID = "RemoveItem.Invoke";
        private const string CopyItemNotSupportedErrorID = "CopyItem.NotSupported";
        private const string CopyItemInvokeErrorID = "CopyItem.Invoke";
        private const string CopyItemDestinationContainerDoesNotExistErrorID = "CopyItem.DestinationContainerDoesNotExist";
        private const string ClearItemPropertyNotsupportedErrorID = "ClearItemProperty.NotSupported";
        private const string GetHelpCustomMamlErrorID = "GetHelp.CustomMaml";
        private const string GetItemInvokeErrorID = "GetItem.Invoke";
        private const string SetItemNotSupportedErrorID = "SetItem.NotSupported";
        private const string SetItemInvokeErrorID = "SetItem.Invoke";
        private const string ClearItemNotSupportedErrorID = "ClearItem.NotSupported";
        private const string ClearItemInvokeErrorID = "ClearItem.Invoke";
        private const string InvokeItemNotSupportedErrorID = "InvokeItem.NotSupported";
        private const string InvokeItemInvokeErrorID = "InvokeItem.Invoke";
        private const string MoveItemNotSupportedErrorID = "MoveItem.NotSupported";
        private const string MoveItemInvokeErrorID = "MoveItem.Invoke";
        private const string ShouldContinuePrompt = "Are you sure?";
    }
}
