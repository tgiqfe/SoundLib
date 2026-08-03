namespace SoundLib.Pwsh.Lib
{
    internal class SoundVolume
    {
        public int Level { get; private set; }
        public bool Mute { get; private set; }

        public SoundVolume(int level, bool mute)
        {
            this.Level = level;
            this.Mute = mute;
        }
    }
}
