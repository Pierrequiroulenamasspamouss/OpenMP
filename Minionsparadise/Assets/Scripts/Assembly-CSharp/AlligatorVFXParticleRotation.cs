[global::UnityEngine.ExecuteInEditMode]
public class AlligatorVFXParticleRotation : global::UnityEngine.MonoBehaviour
{
	public enum FollowAxis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public global::UnityEngine.ParticleSystem particleSys;

	public AlligatorVFXParticleRotation.FollowAxis followAxis = AlligatorVFXParticleRotation.FollowAxis.Y;

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
		case AlligatorVFXParticleRotation.FollowAxis.X:
			main.startRotation = base.transform.rotation.eulerAngles.x * ((float)global::System.Math.PI / 180f);
			break;
		case AlligatorVFXParticleRotation.FollowAxis.Y:
			main.startRotation = base.transform.rotation.eulerAngles.y * ((float)global::System.Math.PI / 180f);
			break;
		case AlligatorVFXParticleRotation.FollowAxis.Z:
			main.startRotation = base.transform.rotation.eulerAngles.z * ((float)global::System.Math.PI / 180f);
			break;
		}
	}
}
