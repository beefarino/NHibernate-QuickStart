using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.Attributes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.PowerShell.Provider
{
    interface IProvideNewSession
    {
        IDisposable NewSession();
    }

    [ManageSession]
    public abstract class ProviderWithDisposableSession : Provider, IProvideNewSession
    {
        public abstract IDisposable NewSession();
        
        protected override object GetItemDynamicParameters(string path)
        {
            return base.GetItemDynamicParameters(path);
        }

        protected override object ItemExistsDynamicParameters(string path)
        {
            return base.ItemExistsDynamicParameters(path);
        }

        protected override bool IsItemContainer(string path)
        {
            return base.IsItemContainer(path);
        }

        protected override object MoveItemDynamicParameters(string path, string destination)
        {
            return base.MoveItemDynamicParameters(path, destination);
        }

        protected override void MoveItem(string path, string destination)
        {
            base.MoveItem(path, destination);
        }

        protected override void GetItem(string path)
        {
            base.GetItem(path);
        }

        protected override void SetItem(string path, object value)
        {
            base.SetItem(path, value);
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {
            return base.SetItemDynamicParameters(path, value);
        }

        protected override object ClearItemDynamicParameters(string path)
        {
            return base.ClearItemDynamicParameters(path);
        }

        protected override void ClearItem(string path)
        {
            base.ClearItem(path);
        }

        protected override object InvokeDefaultActionDynamicParameters(string path)
        {
            return base.InvokeDefaultActionDynamicParameters(path);
        }

        protected override void InvokeDefaultAction(string path)
        {
            base.InvokeDefaultAction(path);
        }

        protected override bool ItemExists(string path)
        {
            return base.ItemExists(path);
        }

        protected override bool IsValidPath(string path)
        {
            return base.IsValidPath(path);
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            base.GetChildItems(path, recurse);
        }

        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            return base.GetChildItemsDynamicParameters(path, recurse);
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            base.GetChildNames(path, returnContainers);
        }

        protected override object GetChildNamesDynamicParameters(string path)
        {
            return base.GetChildNamesDynamicParameters(path);
        }

        protected override void RenameItem(string path, string newName)
        {
            base.RenameItem(path, newName);
        }

        protected override object RenameItemDynamicParameters(string path, string newName)
        {
            return base.RenameItemDynamicParameters(path, newName);
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            base.NewItem(path, itemTypeName, newItemValue);
        }

        protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
        {
            return base.NewItemDynamicParameters(path, itemTypeName, newItemValue);
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            base.RemoveItem(path, recurse);
        }

        protected override object RemoveItemDynamicParameters(string path, bool recurse)
        {
            return base.RemoveItemDynamicParameters(path, recurse);
        }

        protected override bool HasChildItems(string path)
        {
            return base.HasChildItems(path);
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }

        protected override object CopyItemDynamicParameters(string path, string destination, bool recurse)
        {
            return base.CopyItemDynamicParameters(path, destination, recurse);
        }
    }
}
