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
using System.Management.Automation.Runspaces;

namespace CodeOwls.PowerShell.Provider.PathNodes
{
    public abstract class NodeFactoryBase : INodeFactory
    {
        public virtual INodeFactory Resolve(string nodeName)
        {
            var children = GetNodeChildren();
            foreach (var child in children)
            {
                var childName = child.Name;
                if (StringComparer.InvariantCultureIgnoreCase.Equals(nodeName, childName))
                {
                    return child;
                }
            }
            return null;
        }

        public abstract IPathNode GetNodeValue();
        public virtual IEnumerable<INodeFactory> GetNodeChildren()
        {
            return null;
        }

        static readonly Dictionary<Type, string> ItemModeCache = new Dictionary<Type, string>();
        protected string EncodedItemMode
        {
            get
            {
                // "dnrgslcmri"           
                // "d+~<>0cmri"

                bool canCopy = null != this as ICopyItem;
                bool canRemove = null != this as IRemoveItem;
                bool canMove = canCopy && canRemove;
                var d = " ";
                var containerEncoded = GetNodeValue().IsCollection ? "d" : d;
                var newEncoded = null != this as INewItem ? "+" : d;
                var removeEncoded = null != this as IRemoveItem ? "~" : d;

                var getEncoded = null != GetNodeValue() ? "<" : d;
                var setEncoded = null != this as ISetItem ? ">" : d;
                var clearEncoded = null != this as IClearItem ? "0" : d;

                var copyEncoded = canCopy ? "c" : d;
                var moveEncoded = canMove ? "m" : d; ;
                var renameEncoded = null != this as IRenameItem ? "r" : d;
                var invokeEncoded = null != this as IInvokeItem ? "i" : d;
                return containerEncoded + newEncoded + removeEncoded + getEncoded + setEncoded +
                                      clearEncoded +
                                      copyEncoded + moveEncoded + renameEncoded + invokeEncoded;
            }
        }
        public virtual string ItemMode
        {
            get
            {
                var type = GetType();

                if (!ItemModeCache.ContainsKey(type))
                {
                    ItemModeCache[type] = EncodedItemMode;
                }

                return ItemModeCache[type];
            }
        }

        public abstract string Name
        {
            get;
        }
    }
}
