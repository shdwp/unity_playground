using System.Collections;
using System.Linq;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    /// <summary>
    /// Controller behavior for targets. Will spawn new targets when needed, provides methods to cut current targets and
    /// spawns debris.
    /// </summary>
    public class TargetObjectsController : MonoBehaviour
    {
        /// <summary>
        /// World transform. Will act as a parent transform for targets and debris.
        /// </summary>
        public Transform worldTransform;
        
        /// <summary>
        /// Array of prefabs to use as targets, those should contain mesh filter.
        /// Will spawn each prefab consecutively, one at a time.
        /// </summary>
        public GameObject[] prefabs;
        
        /// <summary>
        /// Delay before spawning new target after current has been cut.
        /// </summary>
        public float spawnDelaySeconds = 3f;

        /// <summary>
        /// Amount of force to apply to debris to get them fly away
        /// </summary>
        public float debrisForceAmount = 1600f;

        /// <summary>
        /// Physics gravity
        /// </summary>
        public Vector3 gravity;

        // index of next prefab to spawn (in `prefabs`)
        private int prefabIndex = 0;
        
        // current target object
        private GameObject _target;
        
        // current target mesh
        private Mesh _mesh;
        
        // material of the current target (debris are set to this material as well)
        private Material _material;

        private void Start()
        {
            // spawn initial target
            SpawnNewTarget();

            // setup physics for debris
            Physics.gravity = gravity;
        }

        /// <summary>
        /// Cut current target with plane defined by `normal` and `point`. Those should be defined in world-space. 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="point"></param>
        public void Cut(Vector3 normal, Vector3 point)
        {
            if (_target == null)
            {
                // currently there is no target therefore cut attempt should be ignored
                return;
            }
            
            // create plane in local-space 
            var plane = new Plane(_target.transform.InverseTransformDirection(normal), _target.transform.InverseTransformPoint(point));
            
            // create mesh builders to be used for debris - one is for above, and another is for below
            var meshBuilders = new []
            {
                new MeshBuilder(),
                new MeshBuilder(),
            };

            // force vectors to apply to freshly spawned debries - one for debris above the cut, and another for below
            var forceVectors = new[]
            {
                normal,
                -normal,
            };
            
            // split mesh populating builders with debris mesh data
            SplitMesh(plane, _mesh, meshBuilders[0], meshBuilders[1]);

            for (var i = 0; i < meshBuilders.Count(); i++)
            {
                // create each individual debris game object
                // currently there's always two - one above the plane and one below
                var debrisObject = new GameObject();
                
                // perform initial setup by DebrisController
                debrisObject.AddComponent<DebrisController>().InitialSetup(
                    worldTransform,
                    _target.transform,
                    meshBuilders[i].CookMesh(),
                    _material,
                    forceVectors[i] * debrisForceAmount
                );
            }

            // destroy current target and start coroutine to spawn next one
            Destroy(_target);
            StartCoroutine(SpawnNewTargetAfterDelayCoroutine());
        }

        /// <summary>
        /// Spawn new target with delay (called after current one has been cut)
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnNewTargetAfterDelayCoroutine()
        {
            yield return new WaitForSeconds(spawnDelaySeconds);
            SpawnNewTarget();
            yield return null;
        }

        /// <summary>
        /// Spawn new target (next one in prefab array, loops-around)
        /// </summary>
        private void SpawnNewTarget()
        {
            if (prefabs.Length > 0)
            {
                // create actual game object with next prefab
                _target = Instantiate(prefabs[prefabIndex++]);
                _target.transform.SetParent(worldTransform, false);

                // get mesh and material to be used for cutting
                _mesh = _target.GetComponent<MeshFilter>().sharedMesh;
                _material = _target.GetComponent<MeshRenderer>().sharedMaterial;

                // loop-around prefabIndex if needed
                prefabIndex = prefabIndex > prefabs.Length - 1 ? 0 : prefabIndex;
            }
        }

        /// <summary>
        /// Split mesh in two by plane. Will populate mesh builders with data, which then can be used to cook
        /// debris mesh.
        /// </summary>
        /// <param name="plane">Plane in local-space</param>
        /// <param name="mesh">Mesh to cut</param>
        /// <param name="aboveMeshBuilder">Everything above the plane will go in this builder</param>
        /// <param name="belowMeshBuilder">Everything below the plane will go in this builder</param>
        private void SplitMesh(Plane plane, Mesh mesh, MeshBuilder aboveMeshBuilder, MeshBuilder belowMeshBuilder)
        {
            var trigs = mesh.GetTriangles(0);
            var verts = mesh.vertices;
            var normals = mesh.normals;

            /*
             * Iterate over each triangle of the mesh. There are three cases:
             * - triangle is completely above the plane - it's not cut, added as-in into respective mesh builder
             * - triangle is completely below the plane - same as previous
             * - triangle intersects plane, making it one vertex on one side and two on the others - triangle is cut in two by
             * generating new vertices, one half of it goes into above builder and other into below builder
             */
            for (var trigIdx = 0; trigIdx < trigs.Length; trigIdx += 3)
            {
                // mask that indicates which edges has already been checked (3 bits total)
                var checkedEdgeMask = 0;
                
                /*
                 * Iterate over each edge of the triangle to determine which edges should actually be cut.
                 * Each edge is processed only once, no matter the order of the vertices.
                 * If it's determined that edge intersects the cutting plane, new vertex is added at the intersection point,
                 * and respective mesh builders receive new and existing vertices based on their position relative to the plane.
                 * After all edges has been processed mesh builder's `CommitExistingSurface` method will be called, which will
                 * triangulate all vertices inserted during this loop to create actual trigs.
                 */
                for (var vertAIdx = 0; vertAIdx < 3; vertAIdx++)
                {
                    // get the vert data
                    var vertA = verts[trigs[trigIdx + vertAIdx]];
                    var vertANormal = normals[trigs[trigIdx + vertAIdx]];
                    
                    // figure out whether vert is below or above the plane and insert it into 
                    // respective mesh builder
                    var vertAAbove = plane.GetSide(vertA);
                    (vertAAbove ? aboveMeshBuilder : belowMeshBuilder).InsertExistingSurfaceVert(vertA, vertANormal);

                    for (var vertBIdx = 0; vertBIdx < 3; vertBIdx++)
                    {
                        // iterate over each edge connected to the vert
                        
                        if (vertAIdx == vertBIdx || (checkedEdgeMask & 1 << (vertAIdx + vertBIdx)) > 0)
                        {
                            // skip edges that were already processed as indicated by `checkedEdgeMask`
                            continue;
                        }
                        
                        // get other vert data
                        var vertB = verts[trigs[trigIdx + vertBIdx]];
                        var vertBAbove = plane.GetSide(vertB);

                        // vert A and vert B are not on the same side of the plane, meaning that 
                        // they intersect with it, the point of intersection being the vert we need to create
                        if (vertAAbove != vertBAbove)
                        {
                            // calculate intersection vert
                            var intersection = LightsaberDemoLib.CalculateEdgePlaneIntersection(
                                plane,
                                vertAAbove ? vertA : vertB,
                                !vertBAbove ? vertB : vertA
                            );

                            // insersection vert will go into both mesh builders since it should be 
                            // present on both above and below meshes
                            
                            // insert new vert into builders to create new edges on the existing surfaces
                            aboveMeshBuilder.InsertExistingSurfaceVert(intersection, vertANormal);
                            belowMeshBuilder.InsertExistingSurfaceVert(intersection, vertANormal);
                            
                            // insert new vert to create intersection area surface in the end (actual area of the cut)
                            aboveMeshBuilder.InsertIntersectionSurfaceVert(intersection, -plane.normal);
                            belowMeshBuilder.InsertIntersectionSurfaceVert(intersection, plane.normal);
                        }
                        
                        // update checked edge bitmask. no matter the order the position of the bit will always be the same,
                        // therefore A-B and B-A will result in single iteration, making a total of 3 for each trig
                        checkedEdgeMask |= 1 << (vertAIdx + vertBIdx);
                    }
                }

                // commit existing surface on both builders
                aboveMeshBuilder.CommitExistingSurface();
                belowMeshBuilder.CommitExistingSurface();
            }

            // commit cut area surface on both builders
            aboveMeshBuilder.CommitIntersectionSurface();
            belowMeshBuilder.CommitIntersectionSurface();
        }
    }
}