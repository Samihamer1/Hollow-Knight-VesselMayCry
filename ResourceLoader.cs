﻿using System.IO;
using System.Net;
using VesselMayCry.Helpers;

namespace VesselMayCry
{
    internal static class ResourceLoader
    {
        public static Texture2D LoadTexture2D(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            MemoryStream memoryStream = new((int)stream.Length);
            stream.CopyTo(memoryStream);
            stream.Close();
            var bytes = memoryStream.ToArray();
            memoryStream.Close();

            var texture2D = new Texture2D(1, 1);
            _ = texture2D.LoadImage(bytes);
            texture2D.anisoLevel = 0;

            return texture2D;
        }

        public static Sprite LoadSprite(string path)
        {
            Texture2D texture = LoadTexture2D(path);
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.one / 2, 100.0f);
        }

        public static AudioClip LoadAudioClip(string path)
        {
            Stream audioStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (audioStream != null)
            {
                byte[] buffer = new byte[audioStream.Length];
                audioStream.Read(buffer, 0, buffer.Length);
                audioStream.Dispose();
                return WavUtil.ToAudioClip(buffer);
            }
            return null;
        }
    }
}
