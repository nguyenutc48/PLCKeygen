using System;
using System.Collections.Generic;

namespace PLCKeygen
{
    /// <summary>
    /// Represents a single teaching point with X, Y, Z, RI, RO, F coordinates
    /// </summary>
    public class TeachingPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int RI { get; set; }  // Rotation Inner
        public int RO { get; set; }  // Rotation Outer
        public int F { get; set; }   // Focus

        public TeachingPoint()
        {
            X = Y = Z = RI = RO = F = 0;
        }

        public TeachingPoint(int x, int y, int z, int ri, int ro, int f)
        {
            X = x;
            Y = y;
            Z = z;
            RI = ri;
            RO = ro;
            F = f;
        }
    }

    /// <summary>
    /// Represents all teaching points for a single port
    /// </summary>
    public class PortTeachingPoints
    {
        // Tray Input (OK) - 4 points
        public TeachingPoint TrayInputXYStart { get; set; }
        public TeachingPoint TrayInputXEnd { get; set; }
        public TeachingPoint TrayInputYEnd { get; set; }
        public TeachingPoint TrayInputZPosition { get; set; }

        // Tray NG1 - 4 points
        public TeachingPoint TrayNG1XYStart { get; set; }
        public TeachingPoint TrayNG1XEnd { get; set; }
        public TeachingPoint TrayNG1YEnd { get; set; }
        public TeachingPoint TrayNG1ZPosition { get; set; }

        // Tray NG2 - 4 points
        public TeachingPoint TrayNG2XYStart { get; set; }
        public TeachingPoint TrayNG2XEnd { get; set; }
        public TeachingPoint TrayNG2YEnd { get; set; }
        public TeachingPoint TrayNG2ZPosition { get; set; }

        // Socket - 8 points
        public TeachingPoint SocketXYPosition { get; set; }
        public TeachingPoint SocketZLoad { get; set; }
        public TeachingPoint SocketZUnload { get; set; }
        public TeachingPoint SocketZReady { get; set; }
        public TeachingPoint SocketZReadyLoad { get; set; }
        public TeachingPoint SocketZReadyUnload { get; set; }
        public TeachingPoint SocketFOpened { get; set; }
        public TeachingPoint SocketFClosed { get; set; }

        // Camera - 2 points
        public TeachingPoint CameraXYPosition { get; set; }
        public TeachingPoint CameraZPosition { get; set; }

        public PortTeachingPoints()
        {
            // Initialize all teaching points
            TrayInputXYStart = new TeachingPoint();
            TrayInputXEnd = new TeachingPoint();
            TrayInputYEnd = new TeachingPoint();
            TrayInputZPosition = new TeachingPoint();

            TrayNG1XYStart = new TeachingPoint();
            TrayNG1XEnd = new TeachingPoint();
            TrayNG1YEnd = new TeachingPoint();
            TrayNG1ZPosition = new TeachingPoint();

            TrayNG2XYStart = new TeachingPoint();
            TrayNG2XEnd = new TeachingPoint();
            TrayNG2YEnd = new TeachingPoint();
            TrayNG2ZPosition = new TeachingPoint();

            SocketXYPosition = new TeachingPoint();
            SocketZLoad = new TeachingPoint();
            SocketZUnload = new TeachingPoint();
            SocketZReady = new TeachingPoint();
            SocketZReadyLoad = new TeachingPoint();
            SocketZReadyUnload = new TeachingPoint();
            SocketFOpened = new TeachingPoint();
            SocketFClosed = new TeachingPoint();

            CameraXYPosition = new TeachingPoint();
            CameraZPosition = new TeachingPoint();
        }
    }

    /// <summary>
    /// Represents a complete teaching model with all ports
    /// </summary>
    public class TeachingModel
    {
        public string ModelName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string Description { get; set; }

        // Teaching points for all 4 ports
        public PortTeachingPoints Port1 { get; set; }
        public PortTeachingPoints Port2 { get; set; }
        public PortTeachingPoints Port3 { get; set; }
        public PortTeachingPoints Port4 { get; set; }

        public TeachingModel()
        {
            ModelName = string.Empty;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            Description = string.Empty;

            Port1 = new PortTeachingPoints();
            Port2 = new PortTeachingPoints();
            Port3 = new PortTeachingPoints();
            Port4 = new PortTeachingPoints();
        }

        public TeachingModel(string modelName)
        {
            ModelName = modelName;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            Description = string.Empty;

            Port1 = new PortTeachingPoints();
            Port2 = new PortTeachingPoints();
            Port3 = new PortTeachingPoints();
            Port4 = new PortTeachingPoints();
        }

        /// <summary>
        /// Get teaching points for specified port (1-4)
        /// </summary>
        public PortTeachingPoints GetPortTeachingPoints(int portNumber)
        {
            switch (portNumber)
            {
                case 1: return Port1;
                case 2: return Port2;
                case 3: return Port3;
                case 4: return Port4;
                default: throw new ArgumentException($"Invalid port number: {portNumber}. Must be 1-4.");
            }
        }
    }

    /// <summary>
    /// Container for all saved models
    /// </summary>
    public class TeachingModelCollection
    {
        public List<TeachingModel> Models { get; set; }

        public TeachingModelCollection()
        {
            Models = new List<TeachingModel>();
        }

        /// <summary>
        /// Find model by name
        /// </summary>
        public TeachingModel FindModel(string modelName)
        {
            return Models.Find(m => m.ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if model name already exists
        /// </summary>
        public bool ModelExists(string modelName)
        {
            return Models.Exists(m => m.ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Add new model
        /// </summary>
        public void AddModel(TeachingModel model)
        {
            if (ModelExists(model.ModelName))
            {
                throw new InvalidOperationException($"Model '{model.ModelName}' already exists.");
            }
            Models.Add(model);
        }

        /// <summary>
        /// Update existing model
        /// </summary>
        public void UpdateModel(TeachingModel model)
        {
            var existing = FindModel(model.ModelName);
            if (existing == null)
            {
                throw new InvalidOperationException($"Model '{model.ModelName}' not found.");
            }

            model.LastModified = DateTime.Now;
            int index = Models.IndexOf(existing);
            Models[index] = model;
        }

        /// <summary>
        /// Delete model by name
        /// </summary>
        public bool DeleteModel(string modelName)
        {
            var model = FindModel(modelName);
            if (model != null)
            {
                Models.Remove(model);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all model names
        /// </summary>
        public List<string> GetModelNames()
        {
            return Models.ConvertAll(m => m.ModelName);
        }
    }
}
