using UnityEngine;

public class CreatureImage : MonoBehaviour
{
    private GameObject head = null;
    private GameObject eye = null;
    private GameObject leftArm = null;
    private GameObject rightArm = null;
    private GameObject leftLeg = null;
    private GameObject rightLeg = null;
    private GameObject body = null;
    private GameObject tail = null;
    private GameObject tailObject = null;
    private GameObject chestObject = null;

    public void SetUp( string headId, string eyeId, string armId, string legId, string bodyId, string tailObjectId, string chestObjectId )
    {
        Transform _transform = this.transform;

        head = _transform.Find( "head/" + headId + "_head" ).gameObject;
        head.SetActive( true );

        eye = _transform.Find( "eye/" + eyeId + "_eye" ).gameObject;
        eye.SetActive( true );

        leftArm = _transform.Find( "left_arm/" + armId + "_left_arm_A" ).gameObject;
        leftArm.SetActive( true );

        rightArm = _transform.Find( "right_arm/" + armId + "_right_arm_A" ).gameObject;
        rightArm.SetActive( true );

        leftLeg = _transform.Find( "left_leg/" + legId + "_left_leg_A" ).gameObject;
        leftLeg.SetActive( true );

        rightLeg = _transform.Find( "right_leg/" + legId + "_right_leg_A" ).gameObject;
        rightLeg.SetActive( true );

        body = _transform.Find( "body/" + bodyId + "_body_A" ).gameObject;
        body.SetActive( true );

        tail = _transform.Find( "tail/" + bodyId + "_tail_A" ).gameObject;
        tail.SetActive( true );

        tailObject = _transform.Find( "tail_object/" + tailObjectId + "_tail_object" ).gameObject;
        tailObject.SetActive( true );

        chestObject = _transform.Find( "chest_object/" + chestObjectId + "_chest_object" ).gameObject;
        chestObject.SetActive( true );
    }

    public void Reset()
    {
        if (head != null)
        {
            head.SetActive( false );
        }

        if (eye != null)
        {
            eye.SetActive( false );
        }

        if (leftArm != null)
        {
            leftArm.SetActive( false );
        }

        if (rightArm != null)
        {
            rightArm.SetActive( false );
        }

        if (leftLeg != null)
        {
            leftLeg.SetActive( false );
        }

        if (rightLeg != null)
        {
            rightLeg.SetActive( false );
        }

        if (body != null)
        {
            body.SetActive( false );
        }

        if (tail != null)
        {
            tail.SetActive( false );
        }

        if (tailObject != null)
        {
            tailObject.SetActive( false );
        }

        if (chestObject != null)
        {
            chestObject.SetActive( false );
        }
    }
}
