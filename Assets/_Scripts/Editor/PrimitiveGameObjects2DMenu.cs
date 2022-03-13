using UnityEngine;
using UnityEditor;

namespace Arj2D
{
    public class PrimitivesGameObject2DMenu
    {

        [MenuItem("GameObject/2D Object/Cube", false, 21)]
        public static void Cube()
        {
            PrimitiveGameObjects2D.CreatePrimitive2D(PrimitiveGameObjects2D.PrimitiveType2D.Cube).transform.position = GetViewCenter();
        }

        [MenuItem("GameObject/2D Object/Sphere", false, 22)]
        public static void Sphere()
        {
            PrimitiveGameObjects2D.CreatePrimitive2D(PrimitiveGameObjects2D.PrimitiveType2D.Sphere).transform.position = GetViewCenter();
        }

        [MenuItem("GameObject/2D Object/Capsule", false, 23)]
        public static void Capsule()
        {
            PrimitiveGameObjects2D.CreatePrimitive2D(PrimitiveGameObjects2D.PrimitiveType2D.Capsule).transform.position = GetViewCenter();
        }

        [MenuItem("GameObject/2D Object/Cylinder", false, 24)]
        public static void Cylinder()
        {
            PrimitiveGameObjects2D.CreatePrimitive2D(PrimitiveGameObjects2D.PrimitiveType2D.Cylinder).transform.position = GetViewCenter();
        }

        [MenuItem("GameObject/2D Object/Quad", false, 25)]
        public static void Quad()
        {
            PrimitiveGameObjects2D.CreatePrimitive2D(PrimitiveGameObjects2D.PrimitiveType2D.Quad).transform.position = GetViewCenter();
        }

        static Vector3 GetViewCenter()
        {
            Vector3 tmp = SceneView.lastActiveSceneView.camera.transform.position;
            tmp.z = 0.0f;
            return tmp;
        }
    }
}