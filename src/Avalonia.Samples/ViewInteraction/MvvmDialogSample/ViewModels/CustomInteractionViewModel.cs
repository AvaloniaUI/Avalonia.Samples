using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogSample.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmDialogSample.ViewModels
{
    /// <summary>
    /// This sample demonstrates how to use a custom Interaction, if you have no ReactiveUI installed
    /// </summary>
    /// <remarks>
    /// We make use of the source generators that the CommunityToolkit.Mvvm-package offer. If you want to learn more about it, please visit https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
    /// </remarks>
    public partial class CustomInteractionViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a list of selected files
        /// </summary>
        [ObservableProperty] 
        string[]? _SelectedFiles;

        /// <summary>
        /// Gets an instance of our own Interaction class
        /// </summary>
        public Interaction<string, string[]?> SelectFilesInteraction { get; } = new Interaction<string, string[]?>();

        [RelayCommand]
        private async Task SelectFilesAsync()
        {
            SelectedFiles = await SelectFilesInteraction.HandleAsync("Hello from Avalonia");
        }

        public override string ToString() => "Custom Interaction Sample";
    }
}
