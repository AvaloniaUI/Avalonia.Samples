namespace MusicStore.ViewModels

open CommunityToolkit.Mvvm.ComponentModel

[<AbstractClass>]
type ViewModelBase() =
    inherit ObservableObject()