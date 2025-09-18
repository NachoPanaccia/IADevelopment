using UnityEngine;

public class RigidbodyMove : IMove
{
    readonly Rigidbody rb;
    readonly PlayerModel model;
    readonly Transform visual;

    public RigidbodyMove(Rigidbody rb, PlayerModel model, Transform visualRoot = null)
    {
        this.rb = rb;
        this.model = model;
        this.visual = visualRoot ? visualRoot : rb.transform;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Move(Vector3 dir, float speed)
    {
        if (dir.sqrMagnitude > 0.0001f)
        {
            var target = Quaternion.LookRotation(dir, Vector3.up);
            visual.rotation = Quaternion.Slerp(visual.rotation, target, model.rotationLerp * Time.deltaTime);
        }

        var targetPos = rb.position + dir * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        if (dir.sqrMagnitude < 0.0001f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.angularVelocity = Vector3.zero;
        }
    }
}