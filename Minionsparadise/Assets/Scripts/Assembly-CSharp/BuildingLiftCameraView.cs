using UnityEngine;

public class BuildingLiftCameraView : MonoBehaviour
{
    public Camera m_camera;
    public Camera m_gameCamera;
    public Transform m_gameCameraTransform;

    private void OnPreRender()
    {
        if (m_camera.fieldOfView != m_gameCamera.fieldOfView)
        {
            m_camera.fieldOfView = m_gameCamera.fieldOfView;
            //Debug.Log("[BuildingLiftCameraView] FOV changed: " + m_camera.fieldOfView); removing logs pollution of that, since we know it works
        }

        base.gameObject.transform.localPosition = m_gameCameraTransform.localPosition;
        base.gameObject.transform.localRotation = m_gameCameraTransform.localRotation;
        base.gameObject.transform.localScale = m_gameCameraTransform.localScale;
    }
}
