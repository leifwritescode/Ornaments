﻿using Microsoft.Extensions.DependencyInjection;
using Snowstorm.CommandLine;
using System.CommandLine;

namespace Snowstorm;

public class Snowstorm
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RootCommand _rootCommand;

    public Snowstorm()
    {
        _serviceProvider = InitialiseServices();
        _rootCommand = InitialiseCommands();
    }

    public async Task Run(string[] args)
    {
        await _rootCommand.InvokeAsync(args);
    }

    private async void HandleSolveCommandAsync(SolveCommandOptions solveCommandOptions)
    {
        throw new NotImplementedException();
    }

    private async void HandleListCommandAsync(ListCommandOptions listCommandOptions)
    {
        throw new NotImplementedException();
    }

    private async void HandleBenchmarkCommandAsync(BenchmarkCommandOptions benchmarkCommandOptions)
    {
        throw new NotImplementedException();
    }

    private IServiceProvider InitialiseServices()
    {
        var serviceCollection = new ServiceCollection();
        return serviceCollection.BuildServiceProvider();
    }

    private RootCommand InitialiseCommands()
    {
        var year = new Option<int>(new[] { "-y", "--year" }, () => 2022, "Advent of Code event year.");
        var day = new Option<int>(new[] { "-d", "--day" }, () => 1, "Advent of Code event date.");
        var dryRun = new Option<bool>("--dry-run", () => false, "Disable automatic submission of answers.");
        var timeout = new Option<int>(new[] { "-t", "--timeout" }, () => 15, "Maximum execution time in seconds. Note: All challenges have a solution taking at most 15s to run.");
        var years = new Option<IEnumerable<int>>(new[] { "-y", "--year" }, () => Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1), "Advent of Code event year.");
        var runs = new Option<int>(new[] { "-r", "--runs" }, () => 1000, "Number of runs to average execution time over.");

        year.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            var range = Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1);
            if (!range.Contains(value))
            {
                result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
            }
        });
        day.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            var range = Enumerable.Range(1, 25);
            if (!range.Contains(value))
            {
                result.ErrorMessage = "Value must be between 1 and 25 (inclusive.)";
            }
        });
        timeout.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            var range = Enumerable.Range(15, 300);
            if (!range.Contains(value))
            {
                result.ErrorMessage = "Value must be between 15 and 300 (inclusive.)";
            }
        });
        years.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<IEnumerable<int>>() ?? Enumerable.Empty<int>();
            var range = Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1);
            if (value.Any(x => !range.Contains(x)))
            {
                result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
            }
        });
        runs.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            var range = Enumerable.Range(1, 1000000);
            if (!range.Contains(value))
            {
                result.ErrorMessage = $"Value must be between 10 and 1000 (inclusive.)";
            }
        });

        var solveCommand = new Command("solve", "Run Advent of Code event solutions.") { year, day, dryRun, timeout };
        var solveCommandArgumentsBinder = new SolveCommandOptionsBinder(year, day, dryRun, timeout);
        solveCommand.SetHandler(HandleSolveCommandAsync, solveCommandArgumentsBinder);

        var listCommand = new Command("list", "List available Advent of Code event solutions.") { years };
        var listCommandArgumentsBinder = new ListCommandOptionsBinder(years);
        listCommand.SetHandler(HandleListCommandAsync, listCommandArgumentsBinder);

        var benchmarkCommand = new Command("benchmark", "Benchmark a solution.") { year, day, runs };
        var benchmarkCommandArgumentsBinder = new BenchmarkCommandOptionsBinder(year, day, runs);
        benchmarkCommand.SetHandler(HandleBenchmarkCommandAsync, benchmarkCommandArgumentsBinder);

        var rootCommand = new RootCommand("Snowstorm — An Advent of Code SDK.");
        rootCommand.AddCommand(solveCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(benchmarkCommand);
        return rootCommand;
    }
}