using System;
using System.IO;
using System.Media;
//Troelsen, A. and Japikse, P. (2022) Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming. Berkeley, CA: Apress.

namespace CybersecurityChatbot_GUI
{
    public static class AudioPlayer
    {
        public static void PlayGreeting(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️  [WARNING] Greeting audio file not found.");
                Console.WriteLine($"   Looking in: {filePath}");
                Console.ResetColor();
                Console.WriteLine();
                return;
            }

            try
            {
                using var player = new SoundPlayer(filePath);
                player.PlaySync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ [ERROR] Could not play audio: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}