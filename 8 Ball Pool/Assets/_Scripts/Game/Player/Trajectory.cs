using System;
using Unity.VisualScripting;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] private Transform whiteBall;
    [SerializeField] private Transform cue;
    [SerializeField] private LineRenderer baseLine;
    [SerializeField] private LineRenderer circleLine;
    [SerializeField] private int circleSegmentsCount = 100;
    [SerializeField] private LineRenderer directionLine;

    private float currentHitDistance;
    private float whiteBallRadius;
    private Vector3 whiteBallCenterPosition;
    private Vector3 directionWhiteVector;
    private Vector3 directionVector;
    private Transform targetBall;


    private void OnEnable()
    {
        if (whiteBall == null)
        {
            whiteBall = GameObject.FindWithTag("WhiteBall").transform;
        }

        whiteBallRadius = whiteBall.GetComponent<SphereCollider>().radius * whiteBall.transform.lossyScale.x;
        circleLine.positionCount = circleSegmentsCount + 1;
    }

    private void Update()
    {
        var currentWhiteBallPosition = whiteBall.position;
        var direction = cue.transform.right;

        if (Physics.SphereCast(currentWhiteBallPosition, whiteBallRadius, direction, out RaycastHit hit))
        {
            
            
            currentHitDistance = hit.distance;
            targetBall = hit.transform;
            directionVector = hit.normal;

            var targetBallPosition = targetBall.position;

            if (targetBall == null) return;

            baseLine.SetPosition(0, whiteBall.position);
            baseLine.SetPosition(1, currentWhiteBallPosition + direction * currentHitDistance);

            float deltaTheta = (2f * Mathf.PI) / circleSegmentsCount;
            float theta = 0f;

            for (int i = 0; i < circleSegmentsCount + 1; i++)
            {
                float x = whiteBallRadius * Mathf.Cos(theta);
                float z = whiteBallRadius * Mathf.Sin(theta);

                Vector3 point = new Vector3(x, 0f, z) + currentWhiteBallPosition +
                                direction * currentHitDistance;

                circleLine.SetPosition(i, point);

                theta += deltaTheta;
            }

            directionLine.SetPosition(0, targetBallPosition);
            directionLine.SetPosition(1, targetBallPosition + directionVector * -0.5f);
        }
    }

    public void SetAlpha(bool isOwner)
    {
        var newColor = baseLine.startColor;
        newColor.a = isOwner ? 1 : 0.5f;

        baseLine.startColor = newColor;
        baseLine.endColor = newColor;

        circleLine.startColor = newColor;
        circleLine.endColor = newColor;

        directionLine.startColor = newColor;
        directionLine.endColor = newColor;
    }
}