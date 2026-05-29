public class PauseSoundCommand : global::strange.extensions.command.impl.Command
{
	private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("PauseSoundCommand") as global::Kampai.Util.IKampaiLogger;

	[Inject]
	public bool isPaused { get; set; }

	public override void Execute()
	{
		Pause("bus:/Non-Diegetic/u_Music", isPaused);
		Pause("bus:/Diagetic/u_SFX", isPaused);
	}

	private void Pause(string busName, bool isPaused)
	{
		FMOD_StudioSystem instance = FMOD_StudioSystem.instance;
		global::FMOD.Studio.System system = instance.System;
		if (!system.isValid())
		{
			return;
		}
		global::FMOD.Studio.Bus bus;
		system.getBus(busName, out bus);
		if (bus.isValid())
		{
			bus.setPaused(isPaused);
		}
		else
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Could not find bus: " + busName);
		}
	}
}
