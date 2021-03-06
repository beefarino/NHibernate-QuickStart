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


using System.Collections.Generic;
using System.Threading;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Provider.PathNodeProcessors
{
    public class CachingPathNodeProcessor : PathNodeProcessorDecorator
    {
        static private Dictionary<string, INodeFactory> _cache;

        public CachingPathNodeProcessor(IPathNodeProcessor basePathNodeProcessor) : base(basePathNodeProcessor)
        {
            var cache = _cache;
            if( null == cache )
            {
                cache = new Dictionary<string, INodeFactory>();
                Interlocked.CompareExchange(ref _cache, cache, null);                
            }
        }

        public override INodeFactory ResolvePath(string path)
        {
            var pathLowered = path.ToLowerInvariant();
            if( _cache.ContainsKey( pathLowered ) )
            {
                return _cache[pathLowered];
            }
            
            var factory = base.ResolvePath(path);
            _cache[pathLowered] = factory;
            
            return factory;
        }
    }
}
