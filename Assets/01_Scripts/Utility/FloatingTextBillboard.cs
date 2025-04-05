using UnityEngine;

public class FloatingTextBillboard : MonoBehaviour
{
    private static Camera cachedCam;

    void Start()
    {
        // 카메라 캐싱
        if (cachedCam == null)
        {
            cachedCam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (cachedCam == null) return;

        // 카메라의 X축 회전만 가져오기 (EulerAngles 기준)
        float pitch = cachedCam.transform.eulerAngles.x;

        // 현재 로컬 회전값 보존
        Vector3 euler = transform.eulerAngles;
        euler.x = pitch; // X만 카메라와 일치
        transform.eulerAngles = euler;
    }
}
