namespace BodoInventory;

internal interface IBuffer
{
    void InsertChar(int index, char c);
    void DeleteChar(int index);
    void Clear();
}