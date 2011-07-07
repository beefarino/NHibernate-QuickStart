using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Paths.Processors
{
    public abstract class PathNodeProcessorBase : IPathNodeProcessor
    {
        protected abstract INodeFactory Root { get; }

        public INodeFactory ResolvePath(string path)
        {
            Regex re = new Regex(@"^[-_a-z0-9:]+:/?");
            path = path.ToLowerInvariant().Replace('\\', '/');
            path = re.Replace(path, "");

            var factory = Root;

            var nodeMonikers = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nodeMoniker in nodeMonikers)
            {
                factory = factory.Resolve(nodeMoniker);
                if (null == factory)
                {
                    break;
                }
            }

            return factory;

        }
    }
}
