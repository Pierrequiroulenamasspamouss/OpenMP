public class UpdateVolumeCommand : global::strange.extensions.command.impl.Command
{
    public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("UpdateVolumeCommand") as global::Kampai.Util.IKampaiLogger;

    [Inject]
    public global::Kampai.Game.IDevicePrefsService prefs { get; set; }

    public override void Execute()
    {
        global::Kampai.Game.DevicePrefs devicePrefs = prefs.GetDevicePrefs();
        FMOD_StudioSystem instance = FMOD_StudioSystem.instance;
        global::FMOD.Studio.System system = instance.System;

        if (!system.isValid())
        {
            logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, "FMOD System is null, skipping volume update.");
            return;
        }

        global::FMOD.Studio.Bus bus;
        system.getBus("bus:/Non-Diegetic/u_Music", out bus);

        if (bus.isValid())
        {
            if (!global::Kampai.Audio.AudioSettingsModel.MusicMuted)
            {
                bool mute = false;
                bus.getMute(out mute);
                if (mute)
                {
                    bus.setMute(false);
                }
            }

            bus.setVolume(devicePrefs.MusicVolume);
        }
        else
        {
            logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Could not find Music bus.");
        }

        global::FMOD.Studio.Bus bus2;
        system.getBus("bus:/Diagetic/u_SFX", out bus2);

        if (bus2.isValid())
        {
            bus2.setVolume(devicePrefs.SFXVolume);
        }
        else
        {
            logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Could not find SFX bus.");
        }

        global::FMOD.Studio.Bus bus3;
        system.getBus("bus:/Non-Diegetic/u_UI", out bus3);

        if (bus3.isValid())
        {
            bus3.setVolume(devicePrefs.SFXVolume);
        }
        else
        {
            logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Could not find UI bus.");
        }
    }
}