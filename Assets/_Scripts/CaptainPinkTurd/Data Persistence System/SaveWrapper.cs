using System;
using System.Collections.Generic;

namespace CaptainPinkTurd.DataPersistence
{
    //Hold all saved objects of the game
    [Serializable] 
    internal class SaveWrapper
    {
        public Dictionary<string, string> saveDataDictionary = new();
    }
}