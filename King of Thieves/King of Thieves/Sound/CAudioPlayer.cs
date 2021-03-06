﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace King_of_Thieves.Sound
{
    class CAudioPlayer
    {
        private Thread _audioThread;
        private CSound _song;
        private BlockingCollection<CSound> _effects;
        public Dictionary<string, CSound> soundBank = new Dictionary<string, CSound>();

        public CAudioPlayer()
        {

            _init();
            _effects = new BlockingCollection<CSound>();
            System.Threading.ThreadStart threadStarter = _checkForThingsToPlay;
            _audioThread = new Thread(threadStarter);
            _audioThread.Start();


        }

        ~CAudioPlayer()
        {
            _audioThread.Abort();
            _effects.Dispose();
            _effects = null;
            _song = null;
        }

        private void _init()
        {
            //load sound files here
            //USE NAMESPACE FORMAT
            soundBank.Add("Player:Attack1", new CSound(CMasterControl.glblContent.Load<SoundEffect>("sounds/linkAttack1")));
            soundBank.Add("Player:Attack2", new CSound(CMasterControl.glblContent.Load<SoundEffect>("sounds/linkAttack2")));
            soundBank.Add("Player:Attack3", new CSound(CMasterControl.glblContent.Load<SoundEffect>("sounds/linkAttack3")));
            soundBank.Add("Player:Attack4", new CSound(CMasterControl.glblContent.Load<SoundEffect>("sounds/linkAttack4")));
            soundBank.Add("Player:SwordSlash", new CSound(CMasterControl.glblContent.Load<SoundEffect>("sounds/linkSwordSlash")));
        }

        public void stop()
        {
            _audioThread.Abort();
        }

        public void pause()
        {
            MediaPlayer.Pause();
        }

        public void resume()
        {
            MediaPlayer.Resume();
        }

        public CSound song
        {
            get
            {
                return _song;
            }
            set
            {
                _song = value;
            }
        }

        public void addSfx(CSound sfx)
        {
            _effects.Add(sfx);
        }

        //this function name is an abomination to my programming abilities. Luckily only the thread is going to use this.
        private void _checkForThingsToPlay()
        {
            while (true)
            {
                _play(_effects.Take());
            }
        }

        private void _play(CSound file)
        {
            if (file != null)
            {
                if (file.sfx != null)
                    file.sfx.Play();
                else if (file.sfxInstance != null)
                {
                    if ((file.track != false) && (file.sfxInstance.State != SoundState.Playing))
                        file.sfxInstance.Play();
                    else //we don't care if it's playing or not, just dooooo it!
                        file.sfxInstance.Play();
                }
                else if (file.song != null)
                {
                    _song = file;
                    MediaPlayer.IsRepeating = file.loop;
                    MediaPlayer.Play(_song.song);
                }
            }
            else
                throw new FormatException("The CSound passed did not contain any valid audio information.");
        }
    }
}