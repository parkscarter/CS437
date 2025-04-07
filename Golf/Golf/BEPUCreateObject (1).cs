// Create a BEPU physics object from a convex model using the mesh.

//private (CompoundBody, Vector3) CreateBEPUEntity(Model model)
//{
//    List<CompoundShapeEntry> shapes = new List<CompoundShapeEntry>();

//    // Derived from coding examples from various sources on the Internet, including StackExchange.
//    foreach (ModelMesh mesh in model.Meshes)
//    {
//        List<Vector3> points = new List<Vector3>();
//        List<BEPUutilities.Vector3> bepuPoints = new List<BEPUutilities.Vector3>();
//        foreach (ModelMeshPart part in mesh.MeshParts)
//        {
//            // Extract vertex data
//            VertexBuffer vertexBuffer = part.VertexBuffer;
//            int vertexStride = vertexBuffer.VertexDeclaration.VertexStride;
//            int vertexCount = vertexBuffer.VertexCount;
//            float[] vertexData = new float[vertexCount * vertexStride / sizeof(float)];
//            vertexBuffer.GetData(vertexData);

//            // Iterate through vertices
//            for (int i = 0; i < vertexData.Length; i += vertexStride / sizeof(float))
//            {
//                Vector3 vertexPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
//                vertexPosition = Vector3.Transform(vertexPosition, mesh.ParentBone.ModelTransform);
//                bepuPoints.Add(ConversionHelper.MathConverter.Convert(vertexPosition));
//            }

//        }
//        // for each mesh create a BEPU convexhull shape.
//        ConvexHullShape convexHullShape = new ConvexHullShape(bepuPoints);
//        CompoundShapeEntry cshape =
//            new CompoundShapeEntry(convexHullShape, ConversionHelper.MathConverter.Convert(mesh.ParentBone.ModelTransform.Translation));
//        shapes.Add(cshape);
//    }

//    BEPUutilities.Vector3 center;
//    CompoundShape shape = new CompoundShape(shapes, out center);
//    CompoundBody body = new CompoundBody(shapes);
//    return (body, ConversionHelper.MathConverter.Convert(center));
//}
