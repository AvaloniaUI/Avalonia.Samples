using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using MsExtensionsHostingSample.ViewModels;

namespace MsExtensionsHostingSample;

public static class ViewLocatorHelpers
{
    public static IServiceCollection AddView<TViewModel, TView>(this IServiceCollection services)
        where TView : Control, new()
        where TViewModel : ViewModelBase
    {
        services.AddSingleton(new ViewLocator.ViewLocationDescriptor(typeof(TViewModel), () => new TView()));
        return services;
    }
} 

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _dic;

    public ViewLocator(IEnumerable<ViewLocationDescriptor> descriptors)
    {
        _dic = descriptors.ToDictionary(x => x.ViewModel, x => x.Factory);
    }
        
    public Control Build(object? param) => _dic[param!.GetType()]();

    public bool Match(object? param) => param is not null && _dic.ContainsKey(param.GetType());
    
    public record ViewLocationDescriptor(Type ViewModel, Func<Control> Factory);
}