namespace CaptainPinkTurd.DataPersistence
{
    public interface IDataPersistence
    {
        string Name { get; } 
        object SaveData();
        void LoadData(object data);
    }
}