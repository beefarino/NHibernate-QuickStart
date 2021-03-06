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


using CodeOwls.PowerShell.Paths.Extensions;

namespace CodeOwls.PowerShell.Provider.PathNodes
{
    public class PathNode : IPathNode
	{
		public PathNode( object item, string name, bool isContainer )
		{
			Item = item;
			Name = name;
			IsCollection = isContainer;
		}
		public object Item
		{
			get;
			private set;
		}

        private string _name;

        public string Name
        {
            get { return _name.MakeSafeForPath(); }
            private set { _name = value; }
        }

        public bool IsCollection
		{
			get; 
			private set;
		}
	}
}
