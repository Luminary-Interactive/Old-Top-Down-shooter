using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIckUpDropController : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer;

    public float pickupRange;
    public float dropForwardForce, dropUpForce;

    public bool equipped;
    public static bool slotfull;

    // Store the initial constraints
    private RigidbodyConstraints initialConstraints;

    void Start()
    {
        slotfull = false;
        if (!equipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }

        if (equipped)
        {
            initialConstraints = rb.constraints; // Store the initial constraints when the gun is equipped

            rb.isKinematic = true;
            coll.isTrigger = true;
            slotfull = true;
        }
    }

    void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickupRange && Input.GetMouseButtonDown(1) && !slotfull)
        {
            PickUp();
        }
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }


    private void PickUp()
    {
        Player.animator.SetLayerWeight(1, 1.0f);
        equipped = true;
        slotfull = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 3.51f, 0);
        transform.localScale = Vector3.one * 1.5f;

        rb.isKinematic = true;
        coll.isTrigger = true;

        GameManager.ammoText.text = gameObject.GetComponent<Shoot>().ammoCapacity.ToString();
    }

    private void Drop()
    {
        rb.constraints = RigidbodyConstraints.None; // Remove all constraints

        Player.animator.SetLayerWeight(1, 0f);
        equipped = false;
        slotfull = false;

        // set parent to null
        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        // add force
        rb.AddForce(Player.Direction * dropForwardForce, ForceMode.Impulse);
        float random = Random.Range(-5f, 5f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
        GameManager.ammoText.text = "";
}


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().Die();
        }

        if (!other.gameObject.CompareTag("Player"))
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
