using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEditor.MPE;

using UnityEngine;

public struct ProjectileState {
    public int id;
    public Vector3 position;
    public Vector3 velocity;
    public bool isAged;
}

public class ProjectileService : MonoBehaviour {

    internal class ProjectileEntity {
    
        public ProjectileEntity(int id, Vector3 position, Vector3 velocity, float spawnTime, float lifetime) {
            Id = id;
            Position = position;
            Velocity = velocity;
            SpawnTime = spawnTime;
            Lifetime = lifetime;
        }

        internal int Id { get; private set; }
        internal Vector3 Position { get; private set; }
        internal Vector3 Velocity { get; private set; }
        internal float SpawnTime { get; private set; }
        internal float Lifetime { get; private set; }
        internal bool IsAged { get; private set; }
        internal bool Killed { get; private set; }

        internal void Move(float deltaTime) {
            Position += Velocity * deltaTime;
        }

        internal void Age(float time) {
            if (!IsAged)
                return;
            
            IsAged = SpawnTime + Lifetime < time;
        }

        internal void Kill() {
            Killed = true;
        }
    }

    private int idCounter = 1;
    private int groupIdCounter = 1;
    private Dictionary<int, ProjectileEntity> projectilesRegistry = new ();
    private Dictionary<int, List<ProjectileEntity>> groupRegistry = new ();
    private List<int> projectileRemovalBuffer = new List<int>(32);

    private void Update() {
        foreach (var projectile in projectilesRegistry.Values) {
            projectile.Age(Time.time);
            
            if (projectile.IsAged) {
                projectileRemovalBuffer.Add(projectile.Id);
                continue;
            }

            projectile.Move(Time.deltaTime);
        }
    }

    private void LateUpdate() {
        if (projectileRemovalBuffer.Count == 0) 
            return;

        foreach (var removalProjectileId in projectileRemovalBuffer) {
            projectilesRegistry.Remove(removalProjectileId);
        }

        projectileRemovalBuffer.Clear();
    }

    public int AddGroup() {
        var nextGroupId = groupIdCounter++;
        groupRegistry[nextGroupId] = new List<ProjectileEntity>(64);
        return nextGroupId;
    }

    public int CreateProjectile(int groupId, Vector3 position, Vector3 velocity, float lifetime) {
        var nextProjectileId = idCounter++;
        
        var nextProjectile = new ProjectileEntity(nextProjectileId, position, velocity, Time.time, lifetime);
        projectilesRegistry[nextProjectileId] = nextProjectile;

        if (groupRegistry.TryGetValue(groupId, out var groupList)) {
            groupList.Add(nextProjectile);
        }
        
        return nextProjectileId;
    }

    public void GetGroupProjectiles(int groupId, List<ProjectileState> projectileStates) {
        projectileStates.Clear();
        var groupProjectiles = groupRegistry[groupId];
        foreach (var projectile in groupProjectiles) {
            if (projectile.Killed)
                continue;
                
            projectileStates.Add(GetState(projectile));
        }
    }

    public void KillProjectile(int projectileId) {
        projectilesRegistry[projectileId].Kill();
        projectileRemovalBuffer.Add(projectileId);
    }

    private ProjectileState GetState(ProjectileEntity entity) {
        return new ProjectileState {
            id = entity.Id,
            position = entity.Position,
            velocity = entity.Velocity,
            isAged = entity.IsAged
        };
    }

#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        foreach (var projectile in projectilesRegistry?.Values) {
            Gizmos.DrawWireSphere(projectile.Position, 0.3f);
        }
    }
#endif

}