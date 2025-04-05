using UnityEngine;

public class FloatingTextBillboard : MonoBehaviour
{
    private static Camera cachedCam;

    void Start()
    {
        // ī�޶� ĳ��
        if (cachedCam == null)
        {
            cachedCam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (cachedCam == null) return;

        // ī�޶��� X�� ȸ���� �������� (EulerAngles ����)
        float pitch = cachedCam.transform.eulerAngles.x;

        // ���� ���� ȸ���� ����
        Vector3 euler = transform.eulerAngles;
        euler.x = pitch; // X�� ī�޶�� ��ġ
        transform.eulerAngles = euler;
    }
}
