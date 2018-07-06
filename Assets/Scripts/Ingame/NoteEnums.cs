namespace TempestWave.Ingame
{
    public enum NoteInfo
    {
        NormalNote, LongNoteStart, LongNoteEnd, SlideNoteStart, SlideNoteCheckpoint, SlideNoteEnd, DamageNote, HiddenNote,
        SystemNoteStarter = 10, SystemNoteEnder, SystemNoteSpeeder, SystemNoteScroller, SystemNoteXLStarter, SystemNoteXLEnder, SystemNoteSlideDummy,
    }

    public enum FlickMode
    {
        None, Left, Right, Up, Down
    }

    public enum NoteSize
    {
        Small, Large, ExtraLarge
    }

    public enum GameMode
    {
        Starlight, Theater, Theater4, Theater2P, Theater2L, Platinum
    }
}
