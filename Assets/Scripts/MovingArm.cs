using UnityEngine;

public class MovingArm : MonoBehaviour
{
    [SerializeField] Transform arm;
    [SerializeField] bool soloYawPadre = true;

    void LateUpdate()
    {
        if (Camera.main == null) return;
        Vector3 dirCam = Camera.main.transform.forward;
        if (soloYawPadre)
        {
            Vector3 plano = dirCam; plano.y = 0f;
            if (plano.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(plano);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(dirCam);
        }
        if (arm != null)
        {
            arm.rotation = Quaternion.LookRotation(dirCam);
        }
    }
}
