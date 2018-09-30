namespace ToastifyAPI.Core.Auth
{
    public interface IToken
    {
        #region Public Properties

        string AccessToken { get; }
        string TokenType { get; }
        int ExpiresIn { get; }
        string RefreshToken { get; }

        #endregion

        bool IsExpired();
    }
}