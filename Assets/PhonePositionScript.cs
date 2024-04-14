using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhonePositionScript : MonoBehaviour
{
    [Header("Objects")]
    public Transform phone;
    public GameObject screen;

    [Header("Tuning")]
    public float movement_speed = 0.1f;
    public float rotation_speed = 180f;

    public Vector3 raised_position = new Vector3(-0.0146f, 0f, 0.074f);
    public Vector3 raised_orientation = new Vector3(0, 90f, -90f);
    public Vector3 scale = new Vector3(0.03622f, 0.03622f, -0.061263f);

    public Vector3 lowered_orientation = new Vector3(0, 90f, 0f);
    public Vector3 lowered_position = new Vector3(-0.0146f, -0.1f, 0.174f);

    [HideInInspector]
    public bool moving = false;

    public IEnumerator RaisePhone()
    {
        if (moving)
        {
            yield break;
        }
        moving = true;
        while (phone.localPosition != raised_position || phone.localRotation != Quaternion.Euler(raised_orientation))
        {
            phone.localRotation = Quaternion.RotateTowards(phone.localRotation, Quaternion.Euler(raised_orientation), rotation_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localRotation.eulerAngles, raised_orientation) < 0.01f)
            {
                phone.localRotation = Quaternion.Euler(raised_orientation);
            }
            phone.localPosition = Vector3.MoveTowards(phone.localPosition, raised_position, movement_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localPosition, raised_position) < 0.001f)
            {
                phone.localPosition = raised_position;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.2f);
        screen.transform.localPosition = new Vector2(2.02f, 1.57f);
        moving = false;
    }

    public IEnumerator LowerPhone()
    {
        if (moving)
        {
            yield break;
        }
        moving = true;
        screen.transform.localPosition = new Vector2(0,-100000); 
        yield return new WaitForSeconds(0.2f);
        while (phone.localPosition != lowered_position || phone.localRotation != Quaternion.Euler(lowered_orientation))
        {
            phone.localRotation = Quaternion.RotateTowards(phone.localRotation, Quaternion.Euler(lowered_orientation), rotation_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localRotation.eulerAngles, lowered_orientation) < 0.01f)
            {
                phone.localRotation = Quaternion.Euler(lowered_orientation);
            }
            phone.localPosition = Vector3.MoveTowards(phone.localPosition, lowered_position, movement_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localPosition, lowered_position) < 0.001f)
            {
                phone.localPosition = lowered_position;
            }
            yield return new WaitForEndOfFrame();
        }
        moving = false;
    }
}
