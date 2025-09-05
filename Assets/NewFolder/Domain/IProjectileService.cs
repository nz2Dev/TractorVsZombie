using System.Collections.Generic;

using UnityEngine;

public struct ProjectileState {
    public int id;
    public Vector3 position;
    public Vector3 velocity;
    public bool isAged;
}

public interface IProjectileService {

    int AddGroup();
    int CreateProjectile(int groupId, Vector3 position, Vector3 velocity, float lifetime);
    void GetGroupProjectiles(int groupId, List<ProjectileState> projectileStates);
    void KillProjectile(int projectileId);

}