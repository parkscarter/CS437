//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//using BEPUphysics;
//using System.Collections.Generic;
//using BEPUphysics.CollisionShapes;
//using BEPUphysics.CollisionShapes.ConvexShapes;
//using BEPUphysics.Entities.Prefabs;
//using System.Runtime.CompilerServices;
//using System.Reflection;
////using Utilities;



//namespace MonoBlocks
//{
//    struct BoundingHull
//    {
//        public List<Vector3> Vertices;
//    }
//    public class Block : DrawableGameComponent
//    {
//        private string _modelPath;
//        private Model _model;
//        private CompoundBody _bepu_model;
//        private bool _showPhysicsBoundingBox = true;
//        private Vector3 _centerOffset;
//        private Matrix _centerTransform;
//        private Vector3 _position;
//        private Quaternion _orientation;

//        public CompoundBody PhysicsModel
//        {
//            get
//            {
//                return _bepu_model;
//            }
//        }

//        public Vector3 Position
//        {
//            get
//            {
//                return ConversionHelper.MathConverter.Convert(_bepu_model.Position);
//            }
//        }

//        public Model Model
//        {
//            get { return _model; }
//        }

//        public Quaternion Orientation
//        {
//            get
//            {
//                return ConversionHelper.MathConverter.Convert(_bepu_model.Orientation);
//            }
//        }

//        public bool ShowPhysicsBounds { get { return _showPhysicsBoundingBox; } set { _showPhysicsBoundingBox = value; } }


//        public Block(Game game, string modelPath, Vector3 position) : this(game, modelPath, position, Quaternion.Identity)
//        {
//        }

//        public Block(Game game, string modelPath, Vector3 position, Quaternion orientation) : base(game)
//        {
//            _modelPath = modelPath;
//            _showPhysicsBoundingBox = true;
//            _position = position;
//            _orientation = orientation;
//        }

//        protected override void LoadContent()
//        {
//            _model = Game.Content.Load<Model>(_modelPath);
//            (CompoundBody, Vector3) result = CreateBEPUEntity(_model);
//            _bepu_model = result.Item1;
//            _centerOffset = result.Item2;
//            _bepu_model.Position = ConversionHelper.MathConverter.Convert(_position);
//            _bepu_model.Orientation = ConversionHelper.MathConverter.Convert(_orientation);
//            _centerTransform = Matrix.CreateTranslation(-_centerOffset);
//            Game.Services.GetService<Space>().Add(_bepu_model);
//            base.LoadContent();
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            foreach (ModelMesh mesh in _model.Meshes)
//            {
//                if (!mesh.Name.Contains("position"))
//                {
//                    foreach (BasicEffect effect in mesh.Effects)
//                    {
//                        effect.World = mesh.ParentBone.ModelTransform * _centerTransform * ConversionHelper.MathConverter.Convert(_bepu_model.WorldTransform);
//                        effect.View = Game.Services.GetService<Camera>().View;
//                        effect.Projection = Game.Services.GetService<Camera>().Projection;
//                    }
//                    mesh.Draw();
//                }
//            }

//            if (_showPhysicsBoundingBox)
//            {
//                DrawBEPUModel(_bepu_model);
//            }
//            base.Draw(gameTime);
//        }


//        public Matrix GetInitialCameraPosition()
//        {
//            ModelMesh ICPos = null;

//            foreach (ModelMesh m in _model.Meshes)
//            {
//                if (m.Name == "InitialPosition")
//                {
//                    ICPos = m;
//                    break;
//                }
//            }
//            return ICPos.ParentBone.ModelTransform * _centerTransform * ConversionHelper.MathConverter.Convert(_bepu_model.WorldTransform);
//        }

//        public Vector3 GetTeePosition()
//        {
//            ModelMesh ICPos = null;

//            foreach (ModelMesh m in _model.Meshes)
//            {
//                if (m.Name == "Tee")
//                {
//                    ICPos = m;
//                    break;
//                }
//            }
//            return (ICPos.ParentBone.ModelTransform * _centerTransform * ConversionHelper.MathConverter.Convert(_bepu_model.WorldTransform)).Translation;
//        }


//        // Derived from coding examples from various sources on the Internet, including StackExchange.
//        private void DrawBEPUModel(CompoundBody bmodel)
//        {
//            Matrix wt = ConversionHelper.MathConverter.Convert(bmodel.WorldTransform);

//            BasicEffect effect = new BasicEffect(GraphicsDevice)
//            {
//                VertexColorEnabled = true,
//                View = Game.Services.GetService<Camera>().View,
//                Projection = Game.Services.GetService<Camera>().Projection,
//                World = Matrix.Identity
//            };

//            foreach (CompoundShapeEntry s in bmodel.Shapes)
//            {
//                Vector3[] vertices = new Vector3[(s.Shape as ConvexHullShape).Vertices.Count];
//                for (int i = 0; i < vertices.Length; i++)
//                {
//                    vertices[i] = ConversionHelper.MathConverter.Convert((s.Shape as ConvexHullShape).Vertices[i]);
//                }
//                VertexPositionColor[] vertexData = new VertexPositionColor[vertices.Length];
//                for (int i = 0; i < vertices.Length; i++)
//                {
//                    vertexData[i] = new VertexPositionColor(vertices[i], Color.Red);
//                }

//                List<short> indices = new List<short>();
//                for (int i = 0; i < vertices.Length; i++)
//                {
//                    for (int j = i + 1; j < vertices.Length; j++)
//                    {
//                        indices.Add((short)i);
//                        indices.Add((short)j);
//                    }
//                }

//                effect.World = ConversionHelper.MathConverter.Convert(s.LocalTransform.Matrix) * wt;

//                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//                {
//                    pass.Apply();
//                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertexData, 0, vertexData.Length, indices.ToArray(), 0, indices.Count / 2);
//                }

//            }
//        }

//        // Derived form coding examples from various sources on the Internet, including StackExchange.
//        private (CompoundBody, Vector3) CreateBEPUEntity(Model model)
//        {
//            List<CompoundShapeEntry> shapes = new List<CompoundShapeEntry>();

//            foreach (ModelMesh mesh in model.Meshes)
//            {
//                List<Vector3> points = new List<Vector3>();
//                List<BEPUutilities.Vector3> bepuPoints = new List<BEPUutilities.Vector3>();
//                foreach (ModelMeshPart part in mesh.MeshParts)
//                {
//                    // Extract vertex data
//                    VertexBuffer vertexBuffer = part.VertexBuffer;
//                    int vertexStride = vertexBuffer.VertexDeclaration.VertexStride;
//                    int vertexCount = vertexBuffer.VertexCount;
//                    float[] vertexData = new float[vertexCount * vertexStride / sizeof(float)];
//                    vertexBuffer.GetData(vertexData);

//                    // Iterate through vertices
//                    for (int i = 0; i < vertexData.Length; i += vertexStride / sizeof(float))
//                    {
//                        Vector3 vertexPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
//                        vertexPosition = Vector3.Transform(vertexPosition, mesh.ParentBone.ModelTransform);
//                        bepuPoints.Add(ConversionHelper.MathConverter.Convert(vertexPosition));
//                    }

//                }
//                ConvexHullShape convexHullShape = new ConvexHullShape(bepuPoints);
//                CompoundShapeEntry cshape = new CompoundShapeEntry(convexHullShape, ConversionHelper.MathConverter.Convert(mesh.ParentBone.ModelTransform.Translation));
//                shapes.Add(cshape);
//            }
//            BEPUutilities.Vector3 center;
//            CompoundShape shape = new CompoundShape(shapes, out center);
//            CompoundBody body = new CompoundBody(shapes);
//            return (body, ConversionHelper.MathConverter.Convert(center));
//        }

//    }

//}
