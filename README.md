# Start Menu Cleaner

This is a .NET Core tool that cleans your Windows start menu using a few very simple heuristics.

## Install the tool

To install the tool, you'll also need to build it using the following instructions.

``` shell
dotnet pack ./StartMenuCleaner/ -c Release --output ./bin/
dotnet tool install -g StartMenuCleaner --add-source ./StartMenuCleaner/bin/ --version <version number>
```

## Updating the tool

There is currently not a way to update to a pre-release version. See [here](https://github.com/dotnet/sdk/issues/2551) for updates on this issue.

For the time being, you need to uninstall the previous version and re-install:

``` shell
dotnet tool uninstall -g StartMenuCleaner
```

## Running the tool

``` shell
> Clean-StartMenu --help
StartMenuCleaner 1.1.4+b3eec6bd01
Copyright (C) 2019 Craig Treasure

  -s, --simulate    Simulate all file operations.

  --silent          Silently run and exit the application.

  -d, --debug       Enable debug information in console.

  --help            Display this help screen.

  --version         Display version information.
  ```

## Running the tool from a schedule task

  1. In **Task Scheduler**, select **Create Task...**.
  2. From the **General** tab, give the taks a memorable name and select **Run whether user is logged on or not** to hide the command window.
  3. From the **Triggers** tab, create a new trigger to begin **At log on** delayed for 30 seconds for your specific user account.
  4. From the **Actions** tab, create a new action to run the `Clean-StartMenu` program with the `--silent` argument.
  5. Configure options on the **Conditions** and **Settings** tabs to your liking.
