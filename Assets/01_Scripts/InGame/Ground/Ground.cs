using UnityEngine;
using MyGame.Utils;
using UnityEngine.Diagnostics;

public class Ground : MonoBehaviour
{
    public Transform[] sections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPoint = Utility.GetMousePosition(Camera.main, LayerMask.GetMask("Ground")).Value;

            if (GetAdjustedPoint(0, sections[0].position, targetPoint, out Vector3 adjustedPoint))
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = adjustedPoint;
            }
        }
    }

    public int GetSectionNumber(Vector3 targetPoint)
    {
        targetPoint.y = transform.position.y;

        for (int i = 0; i < sections.Length; i++)
        {
            float xDistance = Mathf.Abs(sections[i].position.x - targetPoint.x);
            float zDistance = Mathf.Abs(sections[i].position.z - targetPoint.z);

            if (xDistance <= sections[i].lossyScale.x * 0.5f && zDistance <= sections[i].lossyScale.z * 0.5f)
                return i;
        }
        return -1;
    }

    public bool GetAdjustedPoint(int sectionNum, Vector3 startPoint, Vector3 targetPoint, out Vector3 adjustedPoint)
    {
        adjustedPoint = Vector3.zero;
        if (sectionNum == GetSectionNumber(targetPoint))
        {
            adjustedPoint = targetPoint;
            return true;
        }

        Vector3 targetPath = targetPoint - startPoint;
        Ray ray = new Ray(startPoint, targetPath);

        if (RayIntersectsOBB_XZ(ray, sections[sectionNum].position, sections[sectionNum].rotation, sections[sectionNum].lossyScale * 0.5f, out Vector3 hit))
        {
            adjustedPoint = hit;
            return true;
        }
        else
            return false;
    }

    bool RayIntersectsOBB_XZ(Ray ray, Vector3 obbCenter, Quaternion obbRotation, Vector2 halfExtents, out Vector3 hit)
    {
        hit = Vector3.zero;

        // 평면 기준 변환 행렬 (XZ만 고려)
        Matrix4x4 worldToLocal = Matrix4x4.TRS(obbCenter, obbRotation, Vector3.one).inverse;

        Vector3 localOrigin = worldToLocal.MultiplyPoint(ray.origin);
        Vector3 localDirection = worldToLocal.MultiplyVector(ray.direction).normalized;

        Vector2 min = -halfExtents;
        Vector2 max = halfExtents;

        float tmin = (min.x - localOrigin.x) / localDirection.x;
        float tmax = (max.x - localOrigin.x) / localDirection.x;
        if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

        float tzmin = (min.y - localOrigin.z) / localDirection.z;
        float tzmax = (max.y - localOrigin.z) / localDirection.z;
        if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

        if (tmin > tzmax || tzmin > tmax) return false;

        tmin = Mathf.Max(tmin, tzmin);

        Vector3 localHit = localOrigin + localDirection * tmin;
        hit = Matrix4x4.TRS(obbCenter, obbRotation, Vector3.one).MultiplyPoint(localHit);
        return true;
    }

    Color[] sectionColors;

    private void OnDrawGizmos()
    {
        if (sections.Length == 0) return;

        sectionColors = new Color[]
        {
            Color.yellow,
            Color.red,
            Color.blue,
            Color.magenta
        };

        for (int i = 0; i < sections.Length; i++)
        {
            Gizmos.color = sectionColors[i];
            Gizmos.DrawWireCube(sections[i].position, sections[i].lossyScale);
        }
    }
}
