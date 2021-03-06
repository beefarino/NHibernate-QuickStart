using System;
using System.Linq;
using System.Threading;
using CodeOwls.PowerShell.Host.Executors;

namespace CodeOwls.PowerShell.Host.History
{
    internal class HistoryStackWalker : IHistoryStackWalker
    {
        private readonly Executor _executor;
        private HistoryStack _history;
        private int _index;

        public HistoryStackWalker(Executor executor)
        {
            _executor = executor;
            _index = 0;
        }

        #region IHistoryStackWalker Members

        public string Newest()
        {
            bool isReset = LoadHistory();

            if (!_history.Any())
            {
                return null;
            }

            _index = _history.Count - 1;

            return _history[_index];
        }

        public string Oldest()
        {
            bool isReset = LoadHistory();

            if (!_history.Any())
            {
                return null;
            }

            _index = 0;

            return _history[_index];
        }

        public void Reset()
        {
            _history = null;
            _index = 0;
        }

        public string NextUp()
        {
            bool isReset = LoadHistory();

            if (!_history.Any())
            {
                return null;
            }

            _index -= 1;
            _index = Math.Min(_history.Count - 1, Math.Max(0, _index));

            return _history[_index];
        }

        public string NextDown()
        {
            bool isReset = LoadHistory();

            if (!_history.Any())
            {
                return null;
            }

            _index += 1;
            _index = Math.Max(0, Math.Min(_history.Count - 1, _index));

            return _history[_index];
        }

        #endregion

        private bool LoadHistory()
        {
            if (null != _history)
            {
                return false;
            }

            _history = new HistoryStack();
            Exception error;
            var items = _executor.ExecuteCommand("get-history", out error, ExecutionOptions.None);
            if (null == items)
            {
                return false;
            }

            items.ToList().ForEach(
                item =>
                    {
                        var o = item.BaseObject ?? item;
                        if (null == o)
                        {
                            return;
                        }
                        _history.Add(o.ToString());
                    });
            _index = _history.Count;

            return true;
        }
    }
}