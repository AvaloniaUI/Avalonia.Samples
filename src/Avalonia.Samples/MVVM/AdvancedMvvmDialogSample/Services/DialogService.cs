using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMvvmDialogSample.Services
{
	public class DialogService : AvaloniaObject
	{
	    internal static readonly Dictionary<object, Control> RegistrationMapper =
			new Dictionary<object, Control>();

		static DialogService()
		{
			RegisterProperty.Changed.Subscribe(RegisterChanged);
		}

		private static void RegisterChanged(AvaloniaPropertyChangedEventArgs<object?> e)
		{
			if (e.Sender is not Control sender)
			{
				throw new InvalidOperationException("The DialogService can only be registered on a Control");
			}

			// Unregister any old registered context
			if (e.OldValue.Value != null)
			{
				RegistrationMapper.Remove(e.OldValue.Value);
			}

			// Register any new context
			if (e.NewValue.Value != null)
			{
				RegistrationMapper.Add(e.NewValue.Value, sender);
			}
		}

		/// <summary>
		/// This property handles the registration of Views and ViewModel
		/// </summary>
		public static readonly AttachedProperty<object?> RegisterProperty = AvaloniaProperty.RegisterAttached<DialogService, Control, object?>(
			"Register", null);


		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static void SetRegister(AvaloniaObject element, object value)
		{
			element.SetValue(RegisterProperty, value);
		}

		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static object GetRegister(AvaloniaObject element)
		{
			return element.GetValue(RegisterProperty);
		}
	}

	public static class DialogHelper 
	{ 
		public static async Task<IEnumerable<string>?> OpenFileDialogAsync(this object? context, string? title = null, bool selectMany = true)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if(DialogService.RegistrationMapper.TryGetValue(context, out var control))
			{
				var topLevel = TopLevel.GetTopLevel(control);

                var storageFile = await topLevel!.StorageProvider.OpenFilePickerAsync(
                                new FilePickerOpenOptions()
                                {
                                    AllowMultiple = selectMany,
                                    Title = title ?? "Select any file(s)"
                                });

                return storageFile?.Select(s => s.Name);
            }
			return null;
		}

	}
}
