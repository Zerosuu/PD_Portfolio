using UnityEngine;

// Handles logic of when player is on the ground, used as a component on a child object to the player
public class CheckGrounded : MonoBehaviour
{
    private int _enteredGroundColliders = 0; // Increases upon entering ground colliders, decreases when exiting

    public bool IsGrounded { get { return _enteredGroundColliders > 0; } }
    public int EnteredGroundColliders { get { return _enteredGroundColliders; } set { _enteredGroundColliders = value; } }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ground"))
        { 
            _enteredGroundColliders++;
            Debug.Log("Ground entered");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            _enteredGroundColliders--;
            Debug.Log("Ground exited");
        }
    }

    // Function handling recounting the amount of groud colliders the player is currently standing on
    public void RepopulateColliderCount()
    {
        _enteredGroundColliders = 0;

        foreach (Collider col in Physics.OverlapBox(transform.position, transform.localScale, transform.rotation))
            if (col.CompareTag("Ground"))
                _enteredGroundColliders++;

        Debug.Log("Updated collider count to: " + _enteredGroundColliders);
    }
}
