using UnityEngine;

public class SimpleTransformMove : IMove
{
    private readonly Transform _bodyTransform; // qué transform movemos
    private readonly PlayerModel _model;
    private readonly Transform _visual;        // para rotar solo el mesh/animator si querés

    public SimpleTransformMove(Transform bodyTransform, PlayerModel model, Transform visualRoot = null)
    {
        _bodyTransform = bodyTransform;
        _model = model;
        _visual = visualRoot != null ? visualRoot : bodyTransform;
    }

    public void Move(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            _visual.rotation = Quaternion.Slerp(_visual.rotation, target, _model.rotationLerp * Time.deltaTime);
        }

        _bodyTransform.position += direction * speed * Time.deltaTime;
    }
}