using PapertrailFor7DTD.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapertrailFor7DTD {
    internal class API : IModApi {
        public void InitMod(Mod _modInstance) {
            ModEvents.GameAwake.RegisterHandler(PapertrailLogger.Initialize);
        }
    }
}
