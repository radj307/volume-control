using System;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class SpotifyTrack : ISpotifyTrack, IEquatable<SpotifyTrack>
    {
        #region Public Properties

        public SpotifyTrackType Type { get; }
        public string Title { get; }
        public int Length { get; }

        #endregion

        public SpotifyTrack(SpotifyTrackType type) : this(type, null, 0)
        {
        }

        public SpotifyTrack(SpotifyTrackType type, string title) : this(type, title, 0)
        {
        }

        public SpotifyTrack(SpotifyTrackType type, string title, int length)
        {
            this.Type = type;
            this.Title = title;
            this.Length = length;
        }

        public virtual bool IsValid()
        {
            switch (this.Type)
            {
                case SpotifyTrackType.Unknown:
                    return false;

                case SpotifyTrackType.Song:
                    return !string.IsNullOrWhiteSpace(this.Title) && this.Length >= 0;

                case SpotifyTrackType.Episode:
                    return this.Length >= 0;

                case SpotifyTrackType.Ad:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual string GetClipboardText(string template)
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return this.Title ?? string.Empty;
        }

        #region Equals / GetHashCode

        public bool Equals(ISpotifyTrack other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return this.Type == other.Type &&
                   string.Equals(this.Title, other.Title, StringComparison.InvariantCulture) &&
                   this.Length == other.Length;
        }

        public bool Equals(SpotifyTrack other)
        {
            return this.Equals((ISpotifyTrack)other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((SpotifyTrack)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this.Type;
                hashCode = (hashCode * 397) ^ (this.Title != null ? this.Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Length;
                return hashCode;
            }
        }

        #endregion

        #region Static Members

        public static bool Equal(ISpotifyTrack s1, ISpotifyTrack s2)
        {
            if (ReferenceEquals(s1, s2))
                return true;
            if (s1 == null || s2 == null)
                return false;

            return s1.Equals((object)s2);
        }

        #endregion
    }
}