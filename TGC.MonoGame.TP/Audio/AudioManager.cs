using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace TGC.MonoGame.TP.Audio;

public class AudioManager
{
    public SoundEffect JumpSound { get; private set; }
    public SoundEffect RollingSound { get; private set; }
    public List<SoundEffect> BumpSounds { get; private set; }
    
    public SoundEffect OpenMenuSound { get; private set; }
    public SoundEffect SelectMenuSound { get; private set; }
    public SoundEffect ClickMenuSound { get; private set; }
    
    public Song BackgroundMusic { get; private set; }
    
    public void LoadSounds(ContentManager contentManager)
    {
        JumpSound = LoadAudio<SoundEffect>(contentManager, "jump");
        RollingSound = LoadAudio<SoundEffect>(contentManager, "rolling_hard");
        BumpSounds = LoadAudioList<SoundEffect>(contentManager, 4, i => $"bounce_hard{i}");
        OpenMenuSound = LoadAudio<SoundEffect>(contentManager, "open_menu");
        SelectMenuSound = LoadAudio<SoundEffect>(contentManager, "select_menu");
        ClickMenuSound = LoadAudio<SoundEffect>(contentManager, "click_menu");
        BackgroundMusic = LoadAudio<Song>(contentManager, "classic_vibe");
    }
    
    private static T LoadAudio<T>(ContentManager contentManager, string audioName)
    {
        return contentManager.Load<T>(audioName);
    }
    
    private static List<T> LoadAudioList<T>(ContentManager contentManager, int count, Func<int, string> soundNameFunc)
    {
        var soundList = new List<T>();

        for (var i = 1; i <= count; i++)
        {
            var soundName = soundNameFunc(i);
            var sound = LoadAudio<T>(contentManager, soundName);
            soundList.Add(sound);
        }

        return soundList;
    }
}