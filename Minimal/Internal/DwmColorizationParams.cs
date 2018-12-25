using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Internal
{
    internal struct DwmColorizationParams
    {
        public uint ColorizationColor,
            ColorizationAfterglow,
            ColorizationColorBalance,
            ColorizationAfterglowBalance,
            ColorizationBlurBalance,
            ColorizationGlassReflectionIntensity,
            ColorizationOpaqueBlend;
    }
}
