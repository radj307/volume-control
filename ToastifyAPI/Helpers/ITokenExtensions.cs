using ToastifyAPI.Core.Auth;

namespace ToastifyAPI.Helpers
{
    // ReSharper disable once InconsistentNaming
    public static class ITokenExtensions
    {
        #region Static Members

        public static string GetExpirationInfo(this IToken token)
        {
            return token != null
                ? $"{{ {nameof(IToken.CreateDate)}: \"{token.CreateDate:yyyy/MM/dd HH:mm:ss.fffK}\", {nameof(IToken.ExpiresIn)}: {token.ExpiresIn} }}"
                : null;
        }

        #endregion
    }
}