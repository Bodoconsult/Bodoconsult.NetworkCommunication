

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Factory interface for creating an instance of <see cref="IDataMessageSplitter"/> based on the current firmware version
/// </summary>
public interface IDataMessageSplitterFactory
{

    /// <summary>
    /// Create an instance of <see cref="IDataMessageSplitter"/> based on the current firmware version
    /// </summary>
    /// <param name="firmware"></param>
    /// <returns></returns>
    IDataMessageSplitter CreateInstance(uint firmware);

}