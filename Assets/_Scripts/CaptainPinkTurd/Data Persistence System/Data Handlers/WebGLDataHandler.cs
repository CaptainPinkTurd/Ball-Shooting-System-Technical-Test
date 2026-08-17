using System.IO;
using UnityEngine;

namespace CaptainPinkTurd.DataPersistence.DataHandlers
{
    /// <summary>
    /// Will need to test this out first
    /// Save files are stored inside a custom directory under "/idbfs/ProductName"
    /// instead of Application.persistentDataPath.
    /// 
    /// IMPORTANT:
    /// This ONLY works if the WebGL template enables:
    ///
    ///     autoSyncPersistentDataPath: true
    ///
    /// in the generated index.html store in the build template in this path for example
    /// C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Data\PlaybackEngines\WebGLSupport\BuildTools\WebGLTemplates\Base\Default
    ///
    /// Without autoSyncPersistentDataPath, writes to IDBFS are never synchronized
    /// back to IndexedDB, so saves may appear to work during the current session
    /// but disappear after refreshing or reopening the page.
    ///
    /// If upgrading Unity or switching WebGL templates, ALWAYS verify this option
    /// is still enabled before debugging the save system.
    ///
    /// Reference:
    /// https://www.youtube.com/watch?v=a1f_2kdMbZk
    /// </summary>
    public class WebGLDataHandler : DataHandler
    {
        public WebGLDataHandler(string dataFileName, bool useEncryption)
            : base(dataFileName, useEncryption)
        {
            dataDirPath = Path.Combine("idbfs", Application.productName);
            Directory.CreateDirectory(dataDirPath);
        }
    }
}