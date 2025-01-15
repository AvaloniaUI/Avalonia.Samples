// ***********************************************************************
// Assembly         : SimpleToDoListTests
// Author           : Joe Care
// Created          : 01-12-2025
//
// Last Modified By : Joe Care
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="TestAppBuilder.cs" company="SimpleToDoListTests">
//     Copyright (c) .... All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Headless;
using SimpleToDoList;

/// <summary>
/// Class TestAppBuilder.
/// </summary>
[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public class TestAppBuilder
{
    /// <summary>
    /// Builds the avalonia application.
    /// </summary>
    /// <returns>AppBuilder.</returns>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}