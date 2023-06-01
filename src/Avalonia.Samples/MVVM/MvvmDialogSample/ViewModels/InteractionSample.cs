using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmDialogSample.ViewModels
{
    public class InteractionSample : ViewModelBase
    {
        public InteractionSample()
        {
            confirm = new Interaction<string?, string?>();
            SelectFileCommand = ReactiveCommand.CreateFromTask(AskForSampleText);
        }

        private string? _SampleText = "Click Me";

        /// <summary>
        /// Gets or sets the sample text
        /// </summary>
        public string? SampleText
        {
            get { return _SampleText; }
            set { this.RaiseAndSetIfChanged(ref _SampleText, value); }
        }


        private readonly Interaction<string?, string?> confirm;

        /// <summary>
        /// Gets the confirm interaction
        /// </summary>
        public Interaction<string?, string?> Confirm => this.confirm;


        public ICommand SelectFileCommand { get; }

        private async Task AskForSampleText()
        {
            try
            {
SampleText = await confirm.Handle(SampleText);
            }
            catch 
            { 
                //
                    }
        }
    }
}
