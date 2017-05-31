namespace Toastify.Core
{
    public class Song
    {
        public string Artist { get; set; }
        public string Track { get; set; }
        public string Album { get; set; }

        public string CoverArtUrl { get; set; }

        public Song(string artist, string title, string album = null)
        {
            this.Artist = artist;
            this.Track = title;
            this.Album = album;
        }

        public override string ToString()
        {
            return this.Artist == null ? this.Track : $"{this.Artist} – {this.Track}";
        }

        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Artist) || !string.IsNullOrEmpty(this.Track);
        }

        public override bool Equals(object obj)
        {
            var target = obj as Song;
            if (target == null)
                return false;

            return target.Artist == this.Artist && target.Track == this.Track;
        }

        // overriding GetHashCode is "required" when overriding Equals
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}