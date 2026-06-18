using System;
using UnityEngine;
using System.Collections;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Listener")]
    public class StudioListener : MonoBehaviour
    {
        Rigidbody rigidBody;

        void OnEnable()
        {
            RuntimeUtils.EnforceLibraryOrder();
            rigidBody = gameObject.GetComponent<Rigidbody>();
            RuntimeManager.HasListener = true;
            RuntimeManager.SetListenerLocation(gameObject, rigidBody);
            UpdateFMODStudioSystemListener();
        }

        void OnDisable()
        {
            RuntimeManager.HasListener = false;
        }

        void Update()
        {
            RuntimeManager.SetListenerLocation(gameObject, rigidBody);
            UpdateFMODStudioSystemListener();
        }

        void UpdateFMODStudioSystemListener()
        {
            if (global::FMOD_StudioSystem.instance != null && global::FMOD_StudioSystem.instance.System != null && global::FMOD_StudioSystem.instance.System.isValid())
            {
                global::FMOD.ATTRIBUTES_3D attributes = global::FMOD.Studio.UnityUtil.to3DAttributes(gameObject, rigidBody);
                global::FMOD_StudioSystem.instance.System.setListenerAttributes(0, attributes);
            }
        }
    }
}
