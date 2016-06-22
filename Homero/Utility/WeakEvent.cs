﻿// MODIFIED VERSION OF https://github.com/thomaslevesque/WeakEvent
// TODO: ALL LICENSING INFO (PAY RESPECT TO THE MAN!)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Homero.Utility
{
    public class WeakEventSource<TEventArgs>
    {
        private readonly List<WeakDelegate> _handlers;

        public WeakEventSource()
        {
            _handlers = new List<WeakDelegate>();
        }

        public void Raise(object sender, TEventArgs e)
        {
            lock (_handlers)
            {
                _handlers.RemoveAll(h => !h.Invoke(sender, e));
            }
        }

        public void RaiseAsync(object sender, TEventArgs e)
        {
            RaiseAsync(sender, e, null);
        }

        public void RaiseAsync(object sender, TEventArgs e, Predicate<object> filter)
        {
            Task<List<WeakDelegate>> asyncRaise = new Task<List<WeakDelegate>>(() =>
            {
                List<WeakDelegate> result = new List<WeakDelegate>();
                List<WeakDelegate> handlerCopy = new List<WeakDelegate>(_handlers); // Local copy to play around with.
                foreach (var handler in handlerCopy)
                {
                    if (filter != null)
                    {
                        if (!filter(handler.GetTargetInstance()))
                        {
                            continue;
                        }
                    }
                    if (!handler.Invoke(sender, e))
                    {
                        result.Add(handler);
                    }
                }
                return result;
            });

            asyncRaise.ContinueWith((delegate(Task<List<WeakDelegate>> task)
            {
                lock (_handlers)
                {
                    _handlers.RemoveAll(x => task.Result.Contains(x));
                }
            }));

            asyncRaise.Start();
        }

        public void Subscribe(EventHandler<TEventArgs> handler)
        {
            var weakHandlers = handler
                .GetInvocationList()
                .Select(d => new WeakDelegate(d))
                .ToList();

            lock (_handlers)
            {
                _handlers.AddRange(weakHandlers);
            }
        }

        public void Unsubscribe(EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
            {
                int index = _handlers.FindIndex(h => h.IsMatch(handler));
                if (index >= 0)
                    _handlers.RemoveAt(index);
            }
        }

        class WeakDelegate
        {
            #region Open handler generation and cache

            private delegate void OpenEventHandler(object target, object sender, TEventArgs e);

            // ReSharper disable once StaticMemberInGenericType (by design)
            private static readonly ConcurrentDictionary<MethodInfo, OpenEventHandler> _openHandlerCache =
                new ConcurrentDictionary<MethodInfo, OpenEventHandler>();

            private static OpenEventHandler CreateOpenHandler(MethodInfo method)
            {
                var target = Expression.Parameter(typeof(object), "target");
                var sender = Expression.Parameter(typeof(object), "sender");
                var e = Expression.Parameter(typeof(TEventArgs), "e");

                if (method.IsStatic)
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
                else
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            Expression.Convert(target, method.DeclaringType),
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
            }

            #endregion

            private readonly WeakReference _weakTarget;
            private readonly MethodInfo _method;
            private readonly OpenEventHandler _openHandler;
            public WeakDelegate(Delegate handler)
            {
                _weakTarget = handler.Target != null ? new WeakReference(handler.Target) : null;
                _method = handler.GetMethodInfo();
                _openHandler = _openHandlerCache.GetOrAdd(_method, CreateOpenHandler);
            }

            public bool Invoke(object sender, TEventArgs e)
            {
                object target = null;
                if (_weakTarget != null)
                {
                    target = _weakTarget.Target;
                    if (target == null)
                        return false;
                }
                _openHandler(target, sender, e);
                return true;
            }

            public object GetTargetInstance() // TODO: Getter
            {
                return _weakTarget?.Target;
            }

            public bool IsMatch(EventHandler<TEventArgs> handler)
            {
                return ReferenceEquals(handler.Target, _weakTarget?.Target)
                    && handler.GetMethodInfo().Equals(_method);
            }
        }
    }
}