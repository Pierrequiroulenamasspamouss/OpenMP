[global::UnityEngine.ExecuteInEditMode]
public class WaterslideVFXParticleRotation : global::UnityEngine.MonoBehaviour
{
	public enum FollowAxis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public global::UnityEngine.ParticleSystem particleSys;

	public WaterslideVFXParticleRotation.FollowAxis followAxis = WaterslideVFXParticleRotation.FollowAxis.Y;

	private void Start()
	{
		if (particleSys == null)
		{
			particleSys = base.gameObject.GetComponent<global::UnityEngine.ParticleSystem>();
		}
		if (particleSys == null)
		{
			throw new global::System.ArgumentNullException("particleSys", "Please assign particleSys on gameObject " + base.gameObject.name);
		}
	}

	private void Update()
	{
		var main = particleSys.main;
		switch (followAxis)
		{
		case WaterslideVFXParticleRotation.FollowAxis.X:
			main.startRotation = base.transform.rotation.eulerAngles.x * ((float)global::System.Math.PI / 180f);
			break;
		case WaterslideVFXParticleRotation.FollowAxis.Y:
			main.startRotation = base.transform.rotation.eulerAngles.y * ((float)global::System.Math.PI / 180f);
			break;
		case WaterslideVFXParticleRotation.FollowAxis.Z:
			main.startRotation = base.transform.rotation.eulerAngles.z * ((float)global::System.Math.PI / 180f);
			break;
		}
	}
}
