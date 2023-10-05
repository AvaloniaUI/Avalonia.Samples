using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmDialogSample.Core
{
    /// <summary>
    /// Simple implementation of Interaction pattern from ReactiveUI framework.
    /// https://www.reactiveui.net/docs/handbook/interactions/
    /// </summary>
    public sealed class Interaction<TInput, TOutput> : IDisposable, ICommand
    {
        // this is a reference to the registered interaction handler. 
        private Func<TInput, Task<TOutput>>? _handler;

        /// <summary>
        /// Performs the requested interaction <see langword="async"/>. Returns the result provided by the View
        /// </summary>
        /// <param name="input">The input parameter</param>
        /// <returns>The result of the interaction</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task<TOutput> HandleAsync(TInput input)
        {
            if (_handler is null)
            {
                throw new InvalidOperationException("Handler wasn't registered");
            }

            return _handler(input);
        }

        /// <summary>
        /// Registers a handler to our Interaction
        /// </summary>
        /// <param name="handler">the handler to register</param>
        /// <returns>a disposable object to clean up memory if not in use anymore/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IDisposable RegisterHandler(Func<TInput, Task<TOutput>> handler)
        {
            if (_handler is not null)
            {
                throw new InvalidOperationException("Handler was already registered");
            }

            _handler = handler;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            return this;
        }

        public void Dispose()
        {
            _handler = null;
        }

        public bool CanExecute(object? parameter) => _handler is not null;

        public void Execute(object? parameter) => HandleAsync((TInput?)parameter!);

        public event EventHandler? CanExecuteChanged;
    }
}
