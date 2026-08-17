namespace CaptainPinkTurd.DataPersistence.DataHandlers
{
    /// <summary>
    /// Reads/writes save data as real files on disk. Used on platforms with a
    /// persistent, synchronous filesystem (Standalone, Mobile, Console, Editor).
    /// Not used on WebGL - see WebGLDataHandler instead.
    /// </summary>
    public class FileDataHandler : DataHandler
    {
        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
            : base(dataFileName, useEncryption)
        {
            this.dataDirPath = dataDirPath;
        }
    }
}