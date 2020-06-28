using SmartCore.ConfigCenter.Apollo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.ConfigCenter.Apollo.Internals
{
    public abstract class AbstractConfigRepository : IConfigRepository
    {
        
        private readonly List<IRepositoryChangeListener> _listeners = new List<IRepositoryChangeListener>();
        public string Namespace { get; }

        protected AbstractConfigRepository(string @namespace) => Namespace = @namespace;

        public abstract Properties GetConfig();

        public abstract Task Initialize();

        public void AddChangeListener(IRepositoryChangeListener listener)
        {
            lock (_listeners)
                if (!_listeners.Contains(listener))
                {
                    _listeners.Add(listener);
                }
        }

        public void RemoveChangeListener(IRepositoryChangeListener listener)
        {
            lock (_listeners)
                _listeners.Remove(listener);
        }

        protected void FireRepositoryChange(string namespaceName, Properties newProperties)
        {
            lock (_listeners)
                foreach (var listener in _listeners)
                {
                    try
                    {
                        listener.OnRepositoryChange(namespaceName, newProperties);
                    }
                    catch (Exception ex)
                    {
                        //Logger().Error($"Failed to invoke repository change listener {listener.GetType()}", ex);
                    }
                }
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~AbstractConfigRepository()
        {
            Dispose(false);
        }
        #endregion
    }
}
