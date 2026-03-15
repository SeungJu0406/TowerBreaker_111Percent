using UnityEngine;

public class BoundaryGroup : MonoBehaviour
{
    [SerializeField] private Transform LeftBoundary;
    [SerializeField] private Transform RightBoundary;
    [Header("Screen Positions")]
    [SerializeField] private float _leftViewportX = -0.15f;
    [SerializeField] private float _rightViewportX = 1.15f;



    private void Start()
    {
        SetScreenPos();
    }




    private void SetScreenPos()
    {
        Camera cam = Camera.main;
        float depth = Mathf.Abs(cam.transform.position.z);


        float leftW = cam.ViewportToWorldPoint(new Vector3(_leftViewportX, 0f, depth)).x;
        LeftBoundary.position = new Vector3(leftW, LeftBoundary.position.y, 0f);

        float rightW= cam.ViewportToWorldPoint(new Vector3(_rightViewportX, 0f, depth)).x;
        RightBoundary.position = new Vector3(rightW, RightBoundary.position.y, 0f);
    }
}
