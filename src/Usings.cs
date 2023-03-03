global using System.CommandLine;
global using System.CommandLine.Binding;
global using System.Diagnostics.CodeAnalysis;

global using Azure.Identity;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Graph;
global using Microsoft.Graph.Models;

global using Newtonsoft.Json;

global using Onspring.API.SDK;
global using Onspring.API.SDK.Models;

global using OnspringAzureADSyncer.Extensions;
global using OnspringAzureADSyncer.Interfaces;
global using OnspringAzureADSyncer.Models;
global using OnspringAzureADSyncer.Services;

global using Serilog;
global using Serilog.Events;
global using Serilog.Formatting.Compact;
global using Serilog.Sinks.SystemConsole.Themes;

global using ShellProgressBar;
