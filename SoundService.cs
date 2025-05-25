using Plugin.Maui.Audio;

public static class SoundService
{
    static Dictionary<string, IAudioPlayer> sounds = new();
    static IAudioManager audioManager;

    public static async Task LoadSounds()
    {
        audioManager = new AudioManager();

        var cannonFireStream = await FileSystem.OpenAppPackageFileAsync("cannon_fire.mp3");
        sounds["cannon_fire"] = audioManager.CreatePlayer(cannonFireStream);

        var targetHitStream = await FileSystem.OpenAppPackageFileAsync("target_hit.mp3");
        sounds["target_hit"] = audioManager.CreatePlayer(targetHitStream);

        var blockerHitStream = await FileSystem.OpenAppPackageFileAsync("blocker_hit.mp3");
        sounds["blocker_hit"] = audioManager.CreatePlayer(blockerHitStream);
    }

    public static void Play(string name)
    {
        if (sounds.TryGetValue(name, out var player))
        {
            // Виконуємо асинхронно, щоб не блокувати основний потік
            Task.Run(() =>
            {
                player.Stop(); // щоб звук не накладався
                player.Play();
            });
        }
    }
}