using UnityEngine;

public class Wood : DragablePiece {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "CampFire")
        {
            HeatManager.instance.AddLog();
            Destroy(gameObject);
        }
    }
}
