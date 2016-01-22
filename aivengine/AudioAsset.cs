/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Collections.Generic;
using Aiv.Fast2D;
using Aiv.Vorbis;

namespace Aiv.Engine
{
    public class AudioAsset : Asset
    {
        public AudioAsset(string fileName, bool stream = false) : base(fileName)
        {
            if (!stream)
                Clip = new AudioClip(FileName);
        }

        public AudioClip Clip { get; set; }

        public AudioAsset Clone()
        {
            var go = new AudioAsset(BaseFileName);
            return go;
        }
    }
}