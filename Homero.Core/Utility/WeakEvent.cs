﻿// MODIFIED VERSION OF https://github.com/thomaslevesque/WeakEvent
// TODO: ALL LICENSING INFO (PAY RESPECT TO THE MAN!)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Homero.Core.EventArgs;

namespace Homero.Core.Utility
{
    public class WeakEventSource<TEventArgs>
    {
        private readonly List<WeakDelegate> _handlers;
        public event EventHandler<EventFailedEventArgs> AsyncRaiseFailed;
        public event EventHandler InvokeStarted;
        public event EventHandler InvokeEnded;

        public WeakEventSource()
        {
            _handlers = new List<WeakDelegate>();
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            throw new NotImplementedException();
        }

        public void Raise(object sender, TEventArgs e)
        {
            InvokeStarted?.Invoke(this, null);
            lock (_handlers)
            {
                _handlers.RemoveAll(h => !h.Invoke(sender, e));
            }
            InvokeEnded?.Invoke(this, null);
        }

        public void RaiseAsync(object sender, TEventArgs e)
        {
            RaiseAsync(sender, e, null);
        }

        public void RaiseAsync(object sender, TEventArgs e, Predicate<object> filter)
        {
            InvokeStarted?.Invoke(sender, e as System.EventArgs);
            var asyncRaise = new Task<List<WeakDelegate>>(() =>
            {
                var result = new List<WeakDelegate>();
                var handlerCopy = new List<WeakDelegate>(_handlers); // Local copy to play around with.
                foreach (var handler in handlerCopy)
                {
                    if (filter != null)
                    {
                        if (!filter(handler.GetTargetInstance()))
                        {
                            continue;
                        }
                    }
                    try
                    {
                        if (!handler.Invoke(sender, e))
                        {
                            result.Add(handler);
                        }
                    }
                    catch (Exception ex)
                    {
                        AsyncRaiseFailed?.Invoke(sender, new EventFailedEventArgs(e as System.EventArgs, ex));
                    }
                }
                return result;
            });

            asyncRaise.ContinueWith((delegate (Task<List<WeakDelegate>> task)
            {
                lock (_handlers)
                {
                    _handlers.RemoveAll(x => task.Result.Contains(x));
                }
                InvokeEnded?.Invoke(sender, e as System.EventArgs);
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
                var index = _handlers.FindIndex(h => h.IsMatch(handler));
                if (index >= 0)
                    _handlers.RemoveAt(index);
            }
        }

        private class WeakDelegate
        {
            private readonly MethodInfo _method;
            private readonly OpenEventHandler _openHandler;

            private readonly WeakReference _weakTarget;

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
                else if (method.GetCustomAttributes(typeof(AsyncStateMachineAttribute)) != null)
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            Expression.Convert(target, method.DeclaringType),
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

            #endregion Open handler generation and cache
        }
    }
}