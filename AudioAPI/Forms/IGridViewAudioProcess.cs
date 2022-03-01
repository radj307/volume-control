namespace AudioAPI.Forms
{
    public interface IGridViewAudioProcess
    {
        public string ProcessName { get; }
        public float Volume { get; set; }
        public bool Muted { get; set; }
    }
}
