namespace Bodoconsult.Inventory.Helper;

/// <summary>
/// Handling passwords
/// </summary>
public class PasswordHelper
{
    /// <summary>
    /// Encrypts the password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string EncryptPassword(string password)
    {
        var result = Console.PasswordHandler.Encrypt(password);
        // ReSharper disable once RedundantAssignment
        password = "";
        return result;
    }

    /// <summary>
    /// decrypts password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string DecryptPassword(string password)
    {
        var result = Console.PasswordHandler.Decrypt(password);
        // ReSharper disable once RedundantAssignment
        password = "";
        return result;
    }
}