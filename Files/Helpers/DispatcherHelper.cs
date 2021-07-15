using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using System;
using System.Runtime.CompilerServices;


namespace Files.Helpers
{
    /// <summary>
    /// This class provides static methods helper for executing code in UI thread of the main window.
    /// </summary>
    internal static class DispatcherHelper
    { /// <summary>
      /// This struct represents an awaitable dispatcher.
      /// </summary>
        public struct DispatcherPriorityAwaitable
        {
            private readonly DispatcherQueue dispatcher;
            private readonly DispatcherQueuePriority priority;

            internal DispatcherPriorityAwaitable(DispatcherQueue dispatcher, DispatcherQueuePriority priority)
            {
                this.dispatcher = dispatcher;
                this.priority = priority;
            }

            /// <summary>
            /// Get awaiter of DispatcherPriorityAwaiter
            /// </summary>
            /// <returns>Awaiter of DispatcherPriorityAwaiter</returns>
            public DispatcherPriorityAwaiter GetAwaiter()
            {
                return new DispatcherPriorityAwaiter(this.dispatcher, this.priority);
            }
        }

        /// <summary>
        /// This struct represents the awaiter of a dispatcher.
        /// </summary>
        public struct DispatcherPriorityAwaiter : INotifyCompletion
        {
            private readonly DispatcherQueue dispatcher;
            private readonly DispatcherQueuePriority priority;

            /// <summary>
            /// Gets a value indicating whether task has completed
            /// </summary>
            public bool IsCompleted => false;

            internal DispatcherPriorityAwaiter(DispatcherQueue dispatcher, DispatcherQueuePriority priority)
            {
                this.dispatcher = dispatcher;
                this.priority = priority;
            }

            /// <summary>
            /// Get result for this awaiter
            /// </summary>
            public void GetResult()
            {
            }

            /// <summary>
            /// Fired once task has completed for notify completion
            /// </summary>
            /// <param name="continuation">Continuation action</param>
            public async void OnCompleted(Action continuation)
            {
                await this.dispatcher.EnqueueAsync(continuation, this.priority);
            }
        }

        /// <summary>
        /// Yield and allow UI update during tasks.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of a thread to yield</param>
        /// <param name="priority">Dispatcher execution priority, default is low</param>
        /// <returns>Awaitable dispatcher task</returns>
        public static DispatcherPriorityAwaitable YieldAsync(this DispatcherQueue dispatcher, DispatcherQueuePriority priority = DispatcherQueuePriority.Low)
        {
            return new DispatcherPriorityAwaitable(dispatcher, priority);
        }
    }
}