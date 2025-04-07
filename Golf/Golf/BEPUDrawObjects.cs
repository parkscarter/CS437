//// Derived from coding examples from various sources on the Internet, including StackExchange.

//// Example Draw method for drawing BEPU models with optional bounding shape.

//public override void Draw(GameTime gameTime)
//{
//    foreach (ModelMesh mesh in _model.Meshes)
//    {
//        foreach (BasicEffect effect in mesh.Effects)
//        {
//            //effect.EnableDefaultLighting();
//            effect.World = mesh.ParentBone.ModelTransform * ConversionHelper.MathConverter.Convert(_bepu_model.WorldTransform) * _centerTransform;
//            effect.View = Game.Services.GetService<Camera>().View;
//            effect.Projection = Game.Services.GetService<Camera>().Projection;
//        }
//        mesh.Draw();
//    }

//    if (_showPhysicsBoundingBox)
//    {
//        DrawBEPUModel(_bepu_model);
//    }
//    base.Draw(gameTime);
//}


//private void DrawBEPUModel(CompoundBody bmodel)
//{
//    Matrix wt = ConversionHelper.MathConverter.Convert(bmodel.WorldTransform);

//    BasicEffect effect = new BasicEffect(GraphicsDevice)
//    {
//        VertexColorEnabled = true,
//        View = Game.Services.GetService<Camera>().View,
//        Projection = Game.Services.GetService<Camera>().Projection,
//        World = Matrix.Identity
//    };

//    foreach (CompoundShapeEntry s in bmodel.Shapes)
//    {
//        Vector3[] vertices = new Vector3[(s.Shape as ConvexHullShape).Vertices.Count];
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            vertices[i] = ConversionHelper.MathConverter.Convert((s.Shape as ConvexHullShape).Vertices[i]);
//        }
//        VertexPositionColor[] vertexData = new VertexPositionColor[vertices.Length];
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            vertexData[i] = new VertexPositionColor(vertices[i], Color.Red);
//        }

//        List<short> indices = new List<short>();
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            for (int j = i + 1; j < vertices.Length; j++)
//            {
//                indices.Add((short)i);
//                indices.Add((short)j);
//            }
//        }

//        effect.World = wt * ConversionHelper.MathConverter.Convert(s.LocalTransform.Matrix);

//        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//        {
//            pass.Apply();
//            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertexData, 0, vertexData.Length, indices.ToArray(), 0, indices.Count / 2);
//        }

//    }
//}
