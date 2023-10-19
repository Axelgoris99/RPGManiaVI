using UnityEngine;
using UnityEngine.AI;

/*	
	This component is for all objects that the player can
	interact with such as enemies, items etc. It is meant
	to be used as a base class.
*/

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    private Outline outline;
    public Color hoverColor = Color.blue;
    public Color focusColor = Color.yellow;
    
    public float radius = 3f;
    public Transform interactionTransform;

    bool isFocus = false;   // Is this interactable currently being focused?
    public bool IsFocus { get { return isFocus; } }

    bool isHover = false;
    public bool IsHover { get { return isHover; } set { isHover = value; } }

    Transform player;       // Reference to the player transform

    bool hasInteracted = false; // Have we already interacted with the object?

    public virtual void Start()
    {
        if (interactionTransform == null)
        {
            interactionTransform = Player.Instance.transform;
        }

        outline = GetComponent<Outline>();
        outline.OutlineColor = focusColor;
        outline.OutlineWidth = 3f;
        outline.enabled = false;
    }

    void Update()
    {
        if (isFocus)    // If currently being focused
        {
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            // If we haven't already interacted and the player is close enough
            if (!hasInteracted && distance <= radius)
            {
                // Interact with the object
                hasInteracted = true;
                Interact();
            }
        }
    }

    // Called when the object starts being focused
    public void OnFocused(Transform playerTransform)
    {
        outline.OutlineColor = focusColor;
        isFocus = true;
        hasInteracted = false;
        player = playerTransform;
        outline.enabled = true;
    }

    // Called when the object is no longer focused
    public void OnDefocused()
    {
        isFocus = false;
        hasInteracted = false;
        player = null;
        if (isHover) { 
            outline.OutlineColor = hoverColor; 
            return; 
        }
        outline.enabled = false;

    }

    public void OnHover()
    {
        outline.OutlineColor = hoverColor;
        outline.enabled = true;
    }


    // This method is meant to be overwritten
    public virtual void Interact()
    {

    }

}