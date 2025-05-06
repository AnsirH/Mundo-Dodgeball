using UnityEngine;
using MyGame.Utils;
using System.Collections;

public class Ground : MonoBehaviour
{
    public Transform[] sections;

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
        int targetPointSectionNum = GetSectionNumber(targetPoint);
        if (sectionNum == targetPointSectionNum)
        {
            adjustedPoint = targetPoint;
            return true;
        }

        Vector3 targetVector = (targetPoint - startPoint).normalized;
        Ray ray = new Ray(startPoint, targetVector);

        if (RayIntersectsOBB_XZ(
            ray, 
            sections[targetPointSectionNum].position, sections[targetPointSectionNum].rotation, 
            new Vector2(sections[targetPointSectionNum].lossyScale.x, sections[targetPointSectionNum].lossyScale.z) * 0.5f, 
            out Vector3 hit
            ))
        {
            if (GetSectionNumber(hit) != sectionNum)
            {
                ray = new Ray(hit, (startPoint - hit).normalized);
                RayIntersectsOBB_XZ(
                    ray,
                    sections[sectionNum].position,
                    sections[sectionNum].rotation,
                    new Vector2(sections[sectionNum].lossyScale.x,
                    sections[sectionNum].lossyScale.z) * 0.5f,
                    out hit);
            }

            if (Vector3.Distance(startPoint, hit) <= 0.1f)
            {
                hit = GetShortestDistancePoint(sectionNum, startPoint, targetPoint);
            }
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
        if (tmin < 0) tmin = -tmin;
        Vector3 localHit = localOrigin + localDirection * tmin;
        hit = Matrix4x4.TRS(obbCenter, obbRotation, Vector3.one).MultiplyPoint(localHit);


        StartCoroutine(SpawnStartEndAdjustedPoint(ray.origin, ray.GetPoint(Vector3.Distance(ray.origin, hit) * 2.0f), hit));

        return true;
    }

    private Vector3 GetShortestDistancePoint(int sectionNum, Vector3 startPoint, Vector3 targetPoint)
    {
        int targetPointSectionNum = GetSectionNumber(targetPoint);
        if (sectionNum == targetPointSectionNum)
            return targetPoint;

        Transform section = sections[sectionNum];

        Vector3 sectionDirection = targetPoint - section.position;
        Vector3 sectionMax = section.lossyScale * 0.5f;
        Vector3 sectionMin = -sectionMax;
        if (sectionDirection.x > sectionMax.x && sectionDirection.z > sectionMax.z)
            return section.position + sectionMax;
        else if (sectionDirection.x < sectionMin.x && sectionDirection.z > sectionMax.z)
            return section.position + new Vector3(sectionMin.x, 0, sectionMax.z);
        else if (sectionDirection.x < sectionMin.x && sectionDirection.z < sectionMin.z)
            return section.position + sectionMin;
        else if (sectionDirection.x > sectionMax.x && sectionDirection.z < sectionMin.z)
            return section.position + new Vector3(sectionMax.x, 0, sectionMin.z);

        Vector3 targetVector = Vector3.zero;
        if (sectionDirection.x > sectionMax.x)
            targetVector = Vector3.left;
        else if (sectionDirection.x < sectionMin.x)
            targetVector = Vector3.right;
        else if (sectionDirection.z > sectionMax.z)
            targetVector = Vector3.back;
        else if (sectionDirection.z < sectionMin.z)
            targetVector = Vector3.forward;

        Ray ray = new Ray(targetVector, targetVector);

        RayIntersectsOBB_XZ(
            ray,
            section.position, section.rotation,
            sectionMax,
            out Vector3 hit
            );

        StartCoroutine(SpawnStartEndAdjustedPoint(ray.origin, ray.GetPoint(Vector3.Distance(ray.origin, hit) * 2.0f), hit));

        return hit;
    }

    #region Test Functions
    private IEnumerator SpawnStartEndAdjustedPoint(Vector3 start, Vector3 end, Vector3 adjusted)
    {
        GameObject[] points = new GameObject[3];

        points[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        points[0].GetComponent<MeshRenderer>().material.color = Color.red;
        points[0].transform.position = start;

        points[1] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        points[1].GetComponent<MeshRenderer>().material.color = Color.black;
        points[1].transform.position = end;

        points[2] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        points[2].GetComponent<MeshRenderer>().material.color = Color.yellow;
        points[2].transform.position = adjusted;

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < points.Length; i++) 
        {
            Destroy(points[i]);
        }
    }

    Color[] sectionColors;

    private void OnDrawGizmos()
    {
        if (sections == null) return;

        sectionColors = new Color[]
        {
            Color.yellow,
            Color.red,
            Color.blue,
            Color.magenta,
            Color.cyan,
            Color.white,
            Color.black,
            Color.green,
            Color.gray
        };

        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] == null) continue;
            Gizmos.color = sectionColors[i];
            Gizmos.DrawWireCube(sections[i].position, sections[i].lossyScale);
        }
    }

    #endregion
}
