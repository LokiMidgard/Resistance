using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

using Microsoft.Xna.Framework;

namespace Resistance.Musicplayer
{
    public static class MusicManager
    {

        static Song actualSong;

        static bool gameHasControl;

        public static void Init()
        {
            Game1.instance.Activated += new EventHandler<EventArgs>(game_Activated);
        }

        private static void game_Activated(object sender, EventArgs e)
        {
            gameHasControl = MediaPlayer.GameHasControl;

            if (gameHasControl && actualSong != null && MediaPlayer.State != MediaState.Playing)
                Play();
        }

        public static void PlaySong(Song s)
        {
            gameHasControl = MediaPlayer.GameHasControl;

            actualSong = s;

            if (gameHasControl)
                Play();
        }

        public static void Play()
        {
            if (actualSong == null)
                return;

            try
            {
                MediaPlayer.Play(actualSong);
            }
            catch (InvalidOperationException)
            {
                actualSong = null;
            }
        }

        public static void Stop()
        {
            actualSong = null;

            if (gameHasControl)
                MediaPlayer.Stop();
        }

        public static void Update(GameTime gameTime)
        {
            gameHasControl = MediaPlayer.GameHasControl;
            MediaState state = MediaPlayer.State;

            if (gameHasControl )
            {
                if (actualSong != null)
                {
                    if (state != MediaState.Playing)
                    {
                        if (state == MediaState.Paused)
                        {
                            Resume();
                        }

                        else
                        {
                            Play();
                        }

                    }
                }
                else
                {
                    if (state != MediaState.Stopped)
                        MediaPlayer.Stop();
                }
            }

            IsGameMusicPlaying = (state == MediaState.Playing) && gameHasControl;
        }

        public static  void Resume()
        {
            try
            {
                MediaPlayer.Resume();
            }
            catch (InvalidOperationException)
            {
                actualSong = null;
            }
        }

        public static bool IsGameMusicPlaying { get; private set; }
    }
}
