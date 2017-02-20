using System;
using System.Collections.Generic;

namespace ContarPersonas
{
    interface IFrameExtractor
    {
        void ExtractFromTime(string videoFileName, int hours, int minutes, decimal seconds, int numberFrames, string outputBaseName);
        void ExtractFromList(string videoFileName, List<int> list, string outputBaseName, string method = "");
        String[] GetValidListMethods();
    }
}
