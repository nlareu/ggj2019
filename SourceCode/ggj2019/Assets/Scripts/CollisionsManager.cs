using System.Collections.Generic;
using UnityEngine;

public class CollisionsManager {

    private static List<IGameCollision> Collisions;

    static CollisionsManager() {
        Collisions = new List<IGameCollision>
        {
            new AvatarGroundCollision(),
            //new AvatarMortalObjectCollision(Tags.MORTAL_WALL_01),
        };
    }

    public static void ResolveCollision(GameObject collider1, GameObject collider2, Collision2D collision)
    {
        IGameCollision gameColl = Collisions.Find(item => (collider1.CompareTag(item.Collider1Tag)) && (collider2.CompareTag(item.Collider2Tag)));

        if (gameColl != null)
            gameColl.Resolve(collider1, collider2, collision);
    }
}
