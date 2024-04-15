using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    /// <summary>
    /// A concrete subclass of the Unity UI <see cref="Graphic"/> class that just skips drawing.
    /// Useful for providing a raycast target without actually drawing anything.
    /// Useful for blocking touch/click without an image
    ///
    /// Based from - https://gist.github.com/capnslipp/349c18283f2fea316369
    /// </summary>
    public class NonDrawingGraphic : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }

        /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
