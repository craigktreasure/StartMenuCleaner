# Start Menu Cleaner

This is a .NET Core Global Tool that cleans your Windows start menu using a few very simple heuristics.

- [Start Menu Cleaner](#start-menu-cleaner)
  - [Tool management](#tool-management)
    - [Build the tool](#build-the-tool)
    - [Install the tool](#install-the-tool)
    - [Update the tool](#update-the-tool)
    - [Uninstall the tool](#uninstall-the-tool)
  - [Running the tool](#running-the-tool)
  - [Scheduling the tool using Task Scheduler](#scheduling-the-tool-using-task-scheduler)

## Tool management

### Build the tool

To build the tool, run the following command:

``` shell
dotnet build ./StartMenuCleaner/ -c Release
```

The tool will be packed into a `nupkg` file at `./__packages/NuGet/Release/`.

### Install the tool

These instructions assume you have previously [built](#build-the-tool) the tool.

To install the tool, run the following command:

``` shell
dotnet tool install -g StartMenuCleaner --add-source ./__packages/NuGet/Release/ --version <version number>
```

### Update the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

For stable release versions, run the following command:

``` shell
dotnet tool update -g StartMenuCleaner --add-source ./__packages/NuGet/Release/
```

For pre-release versions, there is currently no way to update to a pre-release version. See [here](https://github.com/dotnet/sdk/issues/2551) for updates on this issue. For the time being, you need to [uninstall](#uninstall-the-tool) the previous version of the tool and then [install](#install-the-tool) the tool again.

### Uninstall the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

To uninstall the tool, run the following command:

``` shell
dotnet tool uninstall -g StartMenuCleaner
```

## Running the tool

``` shell
> Clean-StartMenu --help
StartMenuCleaner 1.2.19+8028ea50d1
Copyright (C) 2020 Craig Treasure

  -s, --simulate    Simulate all file operations.

  --silent          Silently run and exit the application.

  -d, --debug       Enable debug information in console.

  --help            Display this help screen.

  --version         Display version information.
  ```

## Scheduling the tool using Task Scheduler

  1. In **Task Scheduler**, select **Create Task...**.
  2. From the **General** tab, give the taks a memorable name and select **Run whether user is logged on or not** to hide the command window.
  3. From the **Triggers** tab, create a new trigger to begin **At log on** delayed for 30 seconds for your specific user account.
  4. From the **Actions** tab, create a new action to run the `Clean-StartMenu` program with the `--silent` argument.
  5. Configure options on the **Conditions** and **Settings** tabs to your liking.
