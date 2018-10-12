using System;
using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISpotifyTrack : IEquatable<ISpotifyTrack>
    {
        #region Public Properties

        SpotifyTrackType Type { get; }
        string Title { get; }
        int Length { get; }

        #endregion

        bool IsValid();
        string GetClipboardText(string template);
    }
}