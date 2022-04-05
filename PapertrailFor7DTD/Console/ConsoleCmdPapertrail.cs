using PapertrailFor7DTD.SDK;
using System;
using System.Collections.Generic;
using System.IO;

namespace PapertrailFor7DTD.Console {
    internal class ConsoleCmdPapertrail : ConsoleCmdAbstract {
        private static readonly string[] commands = new string[] {
                "papertrail",
                "ppt"
            };

        public override string[] GetCommands() {
            return commands;
        }

        public override string GetDescription() {
            return "Setup the voting system on your server";
        }

        public override string GetHelp() {
            return $@"Usage:
  1. {commands[0]}
  2. {commands[0]} set <hostname> <port> [name]
  3. {commands[0]} reset
Description Overview
1. View current configuration
2. Provide hostname and port (which you can get from https://7daystodie-servers.com/servers/manage/), as well as a name you can provide to represent this server
3. Remove existing settings from the server and delete the settings file";
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo) {
            if (_params.Count == 0) {
                SdtdConsole.Instance.Output($@"hostname: {PapertrailLogger.Instance.Settings.hostname}
port: {PapertrailLogger.Instance.Settings.port}
name: {PapertrailLogger.Instance.Settings.systemName}");
            }
            if (_params.Count == 1 && _params[0] == "reset") {
                try {
                    File.Delete(PapertrailSettings.SettingsPath);
                    SdtdConsole.Instance.Output($"Successfully deleted {PapertrailSettings.SettingsPath}");
                } catch (Exception e) {
                    SdtdConsole.Instance.Output($"Failed to deleted {e.Message}");
                }
            }
            if (_params.Count > 1 && _params[0] == "set") {
                var hostname = _params[0];
                if (!int.TryParse(_params[1], out var port)) {
                    SdtdConsole.Instance.Output("Failed to parse port; must be int");
                    return;
                }

                PapertrailLogger.Instance.Settings.hostname = hostname;
                PapertrailLogger.Instance.Settings.port = port;
                if (_params.Count == 3) {
                    PapertrailLogger.Instance.Settings.systemName = _params[2];
                }

                try {
                    PapertrailLogger.Instance.Settings.SaveSettings();
                    SdtdConsole.Instance.Output($"Successfully saved settings to '{PapertrailSettings.SettingsPath}'");
                } catch (Exception e) {
                    SdtdConsole.Instance.Output($"Failed to save settings file to '{PapertrailSettings.SettingsPath}' due to {e.Message}.\n{e.StackTrace}");
                }
            }
        }
    }
}
