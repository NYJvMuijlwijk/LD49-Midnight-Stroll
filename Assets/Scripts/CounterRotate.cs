using UnityEngine;

public class CounterRotate : MonoBehaviour
{
    [SerializeField] private Transform counterTarget;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        if (counterTarget == null) return;

        Quaternion rotation = counterTarget.rotation;
        Quaternion targetRotation = new Quaternion(rotation.x, rotation.y, rotation.z, 1);
        Quaternion inverse = Quaternion.Inverse(targetRotation);
        targetRotation *= inverse;

        // if (targetRotation != _transform.rotation) 
        _transform.rotation = targetRotation;
    }
}