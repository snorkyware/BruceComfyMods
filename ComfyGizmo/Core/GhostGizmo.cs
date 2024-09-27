using UnityEngine;

namespace ComfyGizmo
{
    public sealed class GhostGizmo
    {
        private readonly GameObject _prefab;
        private readonly Transform _root;

        public GhostGizmo()
        {
            _prefab = new GameObject("ComfyGizmo");
            _root = _prefab.transform;
        }

        public static GhostGizmo CreateGhostGizmo()
        {
            return new GhostGizmo();
        }

        public void Destroy()
        {
            Object.Destroy(_prefab);
        }

        public void ApplyRotation(Quaternion rotation)
        {
            _root.rotation *= rotation;
        }

        public void ApplyLocalRotation(Quaternion rotation)
        {
            _root.localRotation *= rotation;
        }

        public void SetRotation(Quaternion rotation)
        {
            _root.rotation = rotation;
        }

        public Quaternion GetRotation()
        {
            return _root.rotation;
        }

        public void SetLocalRotation(Quaternion rotation)
        {
            _root.localRotation = rotation;
        }

        public void SetAxisRotation(float angle, Vector3 axis)
        {
            _root.rotation = Quaternion.AngleAxis(angle, axis);
        }

        public void SetLocalAxisRotation(float angle, Vector3 axis)
        {
            _root.localRotation = Quaternion.AngleAxis(angle, axis);
        }
    }
}