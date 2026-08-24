using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace QrCodeSample.Services;

/// <summary>
/// The dialog manager helps to register an <see cref="IDialogParticipant"/> to a certain <see cref="Visual"/>
/// </summary>
public class DialogManager
{
    // this dictionary stores the mapping
    private static readonly Dictionary<IDialogParticipant, Visual> RegistrationMapper =
        new Dictionary<IDialogParticipant, Visual>();

    // Keep track of per-Visual subscription so we can properly (un)subscribe
    private static readonly AttachedProperty<RegistrationSubscription?> SubscriptionProperty =
        AvaloniaProperty.RegisterAttached<DialogManager, Visual, RegistrationSubscription?>(
            "Subscription");

    static DialogManager()
    {
        // add a listener to changes of the attached register property
        RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
    }

    private static void RegisterChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is null)
        {
            throw new InvalidOperationException("The DialogManager can only be registered on a Visual");
        }

        // Unregister any old registered context
        if (e.GetOldValue<IDialogParticipant>() is { } oldValue)
        {
            // Remove only if the current mapping still points to this sender
            if (RegistrationMapper.TryGetValue(oldValue, out var currentSender) && currentSender == sender)
            {
                RegistrationMapper.Remove(oldValue);
            }

            // Dispose any existing subscription bound to this sender for the old value
            if (sender.GetValue(SubscriptionProperty) is { } oldSub && ReferenceEquals(oldSub.Participant, oldValue))
            {
                oldSub.Dispose();
                sender.SetValue(SubscriptionProperty, null);
            }
        }

        // Register any new context (and ensure it re-registers on re-attach)
        if (e.GetNewValue<IDialogParticipant>() is { } newValue)
        {
            // Clean up previous subscription if any (defensive)
            if (sender.GetValue(SubscriptionProperty) is { } existing)
            {
                existing.Dispose();
                sender.SetValue(SubscriptionProperty, null);
            }

            var sub = new RegistrationSubscription(sender, newValue);
            sender.SetValue(SubscriptionProperty, sub);
            sub.ApplyCurrentAttachmentState();
        }
    }

    /// <summary>
    /// This property handles the registration of Views and ViewModel
    /// </summary>
    public static readonly AttachedProperty<IDialogParticipant> RegisterProperty =
        AvaloniaProperty.RegisterAttached<DialogManager, Visual, IDialogParticipant>(
            "Register");

    /// <summary>
    /// Accessor for attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static void SetRegister(AvaloniaObject element, IDialogParticipant value)
    {
        element.SetValue(RegisterProperty, value);
    }

    /// <summary>
    /// Accessor for attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static IDialogParticipant GetRegister(AvaloniaObject element)
    {
        return element.GetValue(RegisterProperty);
    }

    /// <summary>
    /// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered Visual for the context or null if none was found</returns>
    public static Visual? GetVisualForContext(IDialogParticipant context)
    {
        return RegistrationMapper.GetValueOrDefault(context);
    }

    /// <summary>
    /// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered TopLevel for the context or null if none was found</returns>
    public static TopLevel? GetTopLevelForContext(IDialogParticipant context)
    {
        return TopLevel.GetTopLevel(GetVisualForContext(context));
    }

    private sealed class RegistrationSubscription : IDisposable
    {
        private readonly Visual _sender;
        public IDialogParticipant Participant { get; }

        private readonly EventHandler<VisualTreeAttachmentEventArgs> _onAttached;
        private readonly EventHandler<VisualTreeAttachmentEventArgs> _onDetached;
        private bool _disposed;

        public RegistrationSubscription(Visual sender, IDialogParticipant participant)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            Participant = participant ?? throw new ArgumentNullException(nameof(participant));

            _onAttached = (_, _) => RegistrationMapper[Participant] = _sender;
            _onDetached = (_, _) => RegistrationMapper.Remove(Participant);

            _sender.AttachedToVisualTree += _onAttached;
            _sender.DetachedFromVisualTree += _onDetached;
        }

        public void ApplyCurrentAttachmentState()
        {
            // If already attached, ensure we are registered now
            if (_sender.IsAttachedToVisualTree())
            {
                RegistrationMapper[Participant] = _sender;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _sender.AttachedToVisualTree -= _onAttached;
            _sender.DetachedFromVisualTree -= _onDetached;
            // On dispose, also make sure the mapping is cleared for this participant → sender
            if (RegistrationMapper.TryGetValue(Participant, out var currentSender) && ReferenceEquals(currentSender, _sender))
            {
                RegistrationMapper.Remove(Participant);
            }
        }
    }
}