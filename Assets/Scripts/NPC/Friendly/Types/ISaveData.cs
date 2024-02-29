namespace NPC
{
    internal interface ISaveData<T>
    {
        public void SetData(T data);
        public T GetData();
    }
}