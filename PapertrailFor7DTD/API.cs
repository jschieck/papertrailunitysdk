using PapertrailFor7DTD.SDK;
using UnityEngine;

namespace PapertrailFor7DTD {
    internal class API : IModApi {

        public void InitMod(Mod _modInstance) {
            ModEvents.GameAwake.RegisterHandler(PapertrailLogger.Initialize);
            ModEvents.GameStartDone.RegisterHandler(() => Log.LogCallbacks += LogToPapertrail);
        }

        private static void LogToPapertrail(string _msg, string _trace, LogType _type) {
            if (!PapertrailLogger.IsEnabled) {
                return;
            }
            switch (_type) {
                case LogType.Error:
                case LogType.Exception:
                    PapertrailLogger.Log(Severity.Error, string.IsNullOrEmpty(_trace) ? _msg : $"{_msg}\n{_trace}");
                    break;
                case LogType.Log:
                    PapertrailLogger.Log(Severity.Informational, string.IsNullOrEmpty(_trace) ? _msg : $"{_msg}\n{_trace}");
                    break;
                case LogType.Warning:
                    PapertrailLogger.Log(Severity.Warning, string.IsNullOrEmpty(_trace) ? _msg : $"{_msg}\n{_trace}");
                    break;
            }
        }
    }
}
